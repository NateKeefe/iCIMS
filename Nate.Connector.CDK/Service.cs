using System;
using System.Web;
using System.Collections.Generic;
using System.Reflection;

using Scribe.Core.ConnectorApi;
using Scribe.Core.ConnectorApi.Actions;
using Scribe.Core.ConnectorApi.Metadata;
using Scribe.Core.ConnectorApi.Query;
using Scribe.Core.ConnectorApi.Exceptions;
using Scribe.Core.ConnectorApi.Logger;
using Scribe.Connector.Common.Reflection.Data;

using CDK.Objects;
using CDK.Common;
using static CDK.ConnectionHelper;

using Newtonsoft.Json;
using System.Linq;
using Scribe.Core.ConnectorApi.Cryptography;

namespace CDK
{
    class ConnectorService
    {
        #region Instaniation 
        private Reflector reflector;
        public RestClient client = new RestClient();
        private ConnectionProperties properties { get; set; }
        public bool IsConnected { get; set; }
        public Guid ConnectorTypeId { get; }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Connection
        public enum SupportedActions
        {
            Query,
            Create,
            CreateWith
        }

        public void Connect(IDictionary<string, string> propDictionary)
        {
            if (propDictionary == null)
                throw new InvalidConnectionException("Connection Properties are NULL");

            //capture properties from form
            var connectorProps = new ConnectionProperties();
            connectorProps.BaseUrl = getRequiredPropertyValue(propDictionary, ConnectionPropertyKeys.BaseUrl, ConnectionPropertyLabels.BaseUrl);
            connectorProps.Username = getRequiredPropertyValue(propDictionary, ConnectionPropertyKeys.Username, ConnectionPropertyLabels.Username);
            connectorProps.Password = getRequiredPropertyValue(propDictionary, ConnectionPropertyKeys.Password, ConnectionPropertyLabels.Password);
            connectorProps.CustomerId = getRequiredPropertyValue(propDictionary, ConnectionPropertyKeys.CustomerId, ConnectionPropertyLabels.CustomderId);
            connectorProps.HMAC = getRequiredPropertyValue(propDictionary, ConnectionPropertyKeys.HMAC, ConnectionPropertyLabels.HMAC);

            //remove extra slash for consistency
            if (connectorProps.BaseUrl.ToString().EndsWith("/"))
            { connectorProps.BaseUrl = connectorProps.BaseUrl.Remove(connectorProps.BaseUrl.Length - 1); }

            //decrypt password value for later
            connectorProps.Password = Decryptor.Decrypt_AesManaged(connectorProps.Password, Connector.CryptoKey);

            // re-check unencrypted password
            if (string.IsNullOrEmpty(connectorProps.Password))
                throw new InvalidConnectionException(string.Format("A value is required for '{0}'", ConnectionPropertyLabels.Password));

            properties = connectorProps;
            reflector = new Reflector(Assembly.GetExecutingAssembly());
            IsConnected = true;
        }
        private static string getRequiredPropertyValue(IDictionary<string, string> properties, string key, string label)
        {
            var value = getPropertyValue(properties, key);
            if (string.IsNullOrEmpty(value))
                throw new InvalidConnectionException(string.Format("A value is required for '{0}'", label));

            return value;
        }

        private static string getPropertyValue(IDictionary<string, string> properties, string key)
        {
            var value = "";
            properties.TryGetValue(key, out value);
            return value;
        }

        public void Disconnect()
        {
            IsConnected = false;
        }
        #endregion

        #region Operations
        public OperationResult Create(DataEntity dataEntity)
        {
            var entityName = dataEntity.ObjectDefinitionFullName;
            var operationResult = new OperationResult();
            var output = new DataEntity(entityName);

            switch (entityName)
            {
                case EntityNames.Person:
                    var person = ToScribeModel<Entities.Person.Rootobject>(dataEntity);
                    output.Properties["location"] = PostApi(entityName, person, properties);
                    operationResult.Success = new[] { true };
                    operationResult.ObjectsAffected = new[] { 1 };
                    operationResult.Output = new[] { output };
                    break;
                default:
                    throw new ArgumentException($"{entityName} is not supported for Create.");
            }
            return operationResult;
        }

        private T ToScribeModel<T>(DataEntity input) where T : new()
        {
            T scribeModel;
            try
            {
                scribeModel = reflector.To<T>(input);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Error translating from DataEntity to ScribeModel: " + e.Message, e);
            }
            return scribeModel;
        }

        private string PostApi<T>(string entityName, T data, ConnectionProperties props)
        {
            client.Accept = "application/json";
            client.ContentType = "application/json";
            client.Method = HttpVerb.POST;
            //serializer settings
            var settings = new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTH:mm:ss.fffZ", NullValueHandling = NullValueHandling.Ignore};
            //auth settings
            if (props.HMAC == "Disabled") { client.AuthType = "Basic"; client.UserName = props.Username; client.Password = props.Password; } else { client.AuthType = "HMAC"; }

            switch (entityName)
            {
                case EntityNames.Person:
                    try
                    {
                        client.EndPoint = props.BaseUrl + "/customers/" + props.CustomerId + "/people";
                        client.PostData = JsonConvert.SerializeObject(data, Formatting.Indented, settings);
                        var response = client.MakeRequest("");
                        return response;
                    }
                    catch (RESTRequestException ex)
                    {
                        Logger.Write(Logger.Severity.Error,
                            $"Error on update for entity: {entityName}.", ex.InnerException.Message);
                        throw new InvalidExecuteOperationException($"Error on update for {entityName}: " + ex.Message);
                    }
            }

            return null;
        }
        #endregion

        #region Query
        public IEnumerable<DataEntity> ExecuteQuery(Query query)
        {
            var entityName = query.RootEntity.ObjectDefinitionFullName;
            var constraints = BuildConstraintDictionary(query.Constraints);

            switch (entityName)
            {
                case EntityNames.Person:
                    return QueryApi<Entities.Person.Rootobject>(query, reflector, constraints, entityName, client, properties);

                default:
                    throw new InvalidExecuteQueryException(
                        $"The {entityName} entity is not supported for query.");
            }
        }

        private static Dictionary<string, object> BuildConstraintDictionary(Expression queryExpression)
        {
            var constraints = new Dictionary<string, object>();

            if (queryExpression == null)
                return constraints;

            if (queryExpression.ExpressionType == ExpressionType.Comparison)
            {
                // only 1 filter
                addCompEprToConstraints(queryExpression as ComparisonExpression, ref constraints);
            }
            else if (queryExpression.ExpressionType == ExpressionType.Logical)
            {
                // Multiple filters
                addLogicalEprToConstraints(queryExpression as LogicalExpression, ref constraints);
            }
            else
                throw new InvalidExecuteQueryException("Unsupported filter type: " + queryExpression.ExpressionType.ToString());

            return constraints;
        }

        private static void addLogicalEprToConstraints(LogicalExpression exp, ref Dictionary<string, object> constraints)
        {
            if (exp.Operator != LogicalOperator.And)
                throw new InvalidExecuteQueryException("Unsupported operator in filter: " + exp.Operator.ToString());

            if (exp.LeftExpression.ExpressionType == ExpressionType.Comparison)
                addCompEprToConstraints(exp.LeftExpression as ComparisonExpression, ref constraints);
            else if (exp.LeftExpression.ExpressionType == ExpressionType.Logical)
                addLogicalEprToConstraints(exp.LeftExpression as LogicalExpression, ref constraints);
            else
                throw new InvalidExecuteQueryException("Unsupported filter type: " + exp.LeftExpression.ExpressionType.ToString());

            if (exp.RightExpression.ExpressionType == ExpressionType.Comparison)
                addCompEprToConstraints(exp.RightExpression as ComparisonExpression, ref constraints);
            else if (exp.RightExpression.ExpressionType == ExpressionType.Logical)
                addLogicalEprToConstraints(exp.RightExpression as LogicalExpression, ref constraints);
            else
                throw new InvalidExecuteQueryException("Unsupported filter type: " + exp.RightExpression.ExpressionType.ToString());
        }

        private static void addCompEprToConstraints(ComparisonExpression exp, ref Dictionary<string, object> constraints)
        {
            if (exp.Operator != ComparisonOperator.Equal)
                throw new InvalidExecuteQueryException(string.Format(StringMessages.ErrorInvalidLogicalExpression, exp.Operator.ToString(), exp.LeftValue.Value));

            var constraintKey = exp.LeftValue.Value.ToString();
            if (constraintKey.LastIndexOf(".") > -1)
            {
                // need to remove "objectname." if present
                constraintKey = constraintKey.Substring(constraintKey.LastIndexOf(".") + 1);
            }
            constraints.Add(constraintKey, exp.RightValue.Value.ToString());
        }

        public static IEnumerable<DataEntity> QueryApi<T>(Query query, Reflector r, Dictionary<string, object> filters, string entityName, RestClient client, ConnectionProperties props)
        {
            client.Method = HttpVerb.GET;
            client.EndPoint = buildRequestUrl(filters, entityName, props);
            client.Accept = "application/json";
            client.PostData = "";
            if (props.HMAC == "Disabled")
                { client.AuthType = "Basic"; client.UserName = props.Username; client.Password = props.Password; }
                else { client.AuthType = "HMAC"; }

            switch (entityName)
            {
                case EntityNames.Person:
                    try
                    {
                        var response = client.MakeRequest("");
                        var data = JsonConvert.DeserializeObject<T>(response);
                        return r.ToDataEntities(new[] { data }, query.RootEntity);
                    }
                    catch (RESTRequestException ex)
                    {
                        Logger.Write(Logger.Severity.Error,
                            $"Error on query for entity: {entityName}.", ex.InnerException.Message);
                        throw new InvalidExecuteQueryException($"Error on query for {entityName}: " + ex.Message);
                    }
                default:
                    throw new InvalidExecuteQueryException($"The {entityName} entity is not supported for query.");
            }
        }

        private static string buildRequestUrl(Dictionary<string, object> filters, string entityName, ConnectionProperties props)
        {
            var uri = new UriBuilder(props.BaseUrl);
            var queryBuilder = HttpUtility.ParseQueryString(string.Empty);
            foreach (var kvp in filters.ToDictionary(k => k.Key, k => k.Value.ToString()).ToArray())
                queryBuilder.Add(kvp.Key, kvp.Value);

            switch (entityName)
            {
                case EntityNames.Person:
                    uri.Path = "/customers/" + props.CustomerId + "/people/" + queryBuilder.Get("peopleId");
                    queryBuilder.Remove("peopleId"); //removes URI param from query string
                    uri.Query = queryBuilder.ToString();
                    return uri.ToString();

                case "something else":
                    uri.Path = props.BaseUrl + "/something/" + props.CustomerId + "/else";
                    uri.Query = queryBuilder.ToString();
                    return uri.ToString();
                default:
                    throw new InvalidExecuteQueryException($"The {entityName} entity is not supported for query.");
            }
        }
        #endregion

        #region Metadata
    public IMetadataProvider GetMetadataProvider()
        {
            return reflector.GetMetadataProvider();
        }

        public IEnumerable<IActionDefinition> RetrieveActionDefinitions()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IObjectDefinition> RetrieveObjectDefinitions(bool shouldGetProperties = false, bool shouldGetRelations = false)
        {
            throw new NotImplementedException();
        }

        public IObjectDefinition RetrieveObjectDefinition(string objectName, bool shouldGetProperties = false,
            bool shouldGetRelations = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMethodDefinition> RetrieveMethodDefinitions(bool shouldGetParameters = false)
        {
            throw new NotImplementedException();
        }

        public IMethodDefinition RetrieveMethodDefinition(string objectName, bool shouldGetParameters = false)
        {
            throw new NotImplementedException();
        }

        public void ResetMetadata()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}