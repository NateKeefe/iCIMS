using System;
using System.Collections.Generic;

using Scribe.Core.ConnectorApi;
using Scribe.Core.ConnectorApi.Actions;
using Scribe.Core.ConnectorApi.Query;
using Scribe.Core.ConnectorApi.Exceptions;
using Scribe.Core.ConnectorApi.Logger;

using CDK.Common;
using MethodInfo = Scribe.Connector.Common.MethodInfo;

namespace CDK
{
    #region Connector Attributes
    [ScribeConnector(
    ConnectorTypeIdAsString,
    ConnectorTypeName,
    ConnectorTypeDescription,
    typeof(Connector),
    "",
    "",
    ConnectionUITypeName,
    ConnectionUIVersion,
    "",
    new[] { "Scribe.IS2.Target", "Scribe.MS2.Target", "Scribe.IS2.Source", "Scribe.MS2.Source" }, //"Scribe.IS2.Message"
    SupportsCloud,
    ConnectorVersion)]
    #endregion

    public class Connector : IConnector //, IDisposable, ISupportProcessNotifications, ISupportOAuth, ISupportMessage
    {
        #region Constants
        internal const string ConnectorTypeName = "Scribe Labs - iCIMS";
        internal const string ConnectorTypeDescription = "Scribe Labs connector that works with iCIMS REST API.";
        internal const string ConnectorVersion = "1.0.0";
        internal const string ConnectorTypeIdAsString = "{3621509B-7A27-4C51-8F8F-61FD69313708}";
        internal const string CryptoKey = "{61770D86-4ADA-456A-BEE5-BF042AA2C771}";
        internal const string CompanyName = "Scribe Labs";
        internal const bool SupportsCloud = true;
        internal const string ConnectionUITypeName = "ScribeOnline.GenericConnectionUI";
        internal const string ConnectionUIVersion = "1.0";
        private MethodInfo methodInfo;
        private ConnectorService service;
        private readonly Guid connectionId;

        public Connector()
        {
            clearLocals();
            connectionId = Guid.NewGuid();
            methodInfo = new MethodInfo(GetType().Name);
        }
        private void clearLocals()
        {
            if (service != null)
            {
                service.Disconnect();
                service = null;
            }
        }
        internal static void unhandledExecptionHandler(string methodName, Exception exception)
        {
            var msg = string.Format("Unhandled exception caught in {0}: {1}\n\n", methodName, exception.Message);
            var details = string.Format("Details: {0}", exception.ToString());
            Logger.Write(Logger.Severity.Error, ConnectorTypeDescription, msg + details);
            throw new ApplicationException(msg, exception);
        }
        #endregion

        #region IConnector
        public Guid ConnectorTypeId => Guid.Parse(ConnectorTypeIdAsString);

        public bool IsConnected
        {
            get
            {
                if (service == null) return false;
                return service.IsConnected;
            }
        }

        public string PreConnect(IDictionary<string, string> properties)
        {
            using (new LogMethodExecution(ConnectorTypeDescription, methodInfo.GetCurrentMethodName()))
            {
                try
                {
                    var uiDef = ConnectionHelper.GetConnectionFormDefintion();
                    return uiDef.Serialize();
                }
                catch (Exception exception)
                {
                    unhandledExecptionHandler(methodInfo.GetCurrentMethodName(), exception);
                }
            }
            return "";
        }

        public void Connect(IDictionary<string, string> properties)
        {
            using (new LogMethodExecution(ConnectorTypeDescription + connectionId.ToString(), methodInfo.GetCurrentMethodName()))
            {
                try
                {
                    if (service == null)
                        service = new ConnectorService();
                    service.Connect(properties);
                }
                catch (InvalidConnectionException)
                {
                    clearLocals();
                    throw;
                }
                catch (Exception exception)
                {
                    clearLocals();
                    unhandledExecptionHandler(methodInfo.GetCurrentMethodName(), exception);
                }
            }
        }

        public void Disconnect()
        {
            using (new LogMethodExecution(ConnectorTypeDescription, methodInfo.GetCurrentMethodName()))
            {
                try
                {
                    clearLocals();
                }
                catch (Exception exception)
                {
                    unhandledExecptionHandler(methodInfo.GetCurrentMethodName(), exception);
                }
            }
        }

        public IMetadataProvider GetMetadataProvider()
        {
            using (new LogMethodExecution(ConnectorTypeDescription, methodInfo.GetCurrentMethodName()))
            {
                if (service.GetMetadataProvider() == null)
                    throw new ApplicationException("Must connect before calling " + methodInfo.GetCurrentMethodName());

                return service.GetMetadataProvider();
            }
        }

        public IEnumerable<DataEntity> ExecuteQuery(Query query)
        {
            using (new LogMethodExecution(ConnectorTypeDescription, methodInfo.GetCurrentMethodName()))
            {
                try
                {
                    if (service == null || service.IsConnected == false)
                        throw new ApplicationException("Must connect before calling " + methodInfo.GetCurrentMethodName());

                    if (query == null)
                        throw new ArgumentNullException(nameof(query));

                    return service.ExecuteQuery(query);
                }
                catch (InvalidExecuteQueryException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    unhandledExecptionHandler(methodInfo.GetCurrentMethodName(), exception);
                }
            }

            return null;
        }

        public OperationResult ExecuteOperation(OperationInput input)
        {
            using (new LogMethodExecution(ConnectorTypeDescription, methodInfo.GetCurrentMethodName()))
            {
                try
                {
                    if (service == null || service.IsConnected == false)
                        throw new ApplicationException("Must connect before calling " +
                                                       methodInfo.GetCurrentMethodName());
                    if (input == null)
                        throw new ArgumentNullException(nameof(input));

                    if (input.Input == null)
                        throw new ArgumentException(StringMessages.ErrorInvalidComparisionExpression, nameof(input));

                    if (!Enum.TryParse(input.Name, out ConnectorService.SupportedActions action))
                        throw new InvalidExecuteOperationException("Unsupported operation: " + input.Name);

                    if (input.Input.Length < 1)
                        throw new ArgumentException(StringMessages.DuplicateFilterField, nameof(input));

                    switch (action)
                    {
                        case ConnectorService.SupportedActions.CreateWith:
                            return service.Create(input.Input[0]);
                        //case ConnectorService.SupportedActions.Execute:
                        //    return service.Execute(input.Input[0]);
                        //case ConnectorService.SupportedActions.Remove:
                        //    return service.Remove(input.Input[0],
                        //        ExpressionParser.GetMatchCriteria(input.LookupCondition[0]));
                        //case ConnectorService.SupportedActions.UpdateWith:
                        //case ConnectorService.SupportedActions.Update:
                        //    return service.Update(input.Input[0],
                        //        ExpressionParser.GetMatchCriteria(input.LookupCondition[0]));
                        default:
                            throw new InvalidExecuteOperationException("Unsupported operation: " + input.Name);
                    }
                }
                catch (InvalidExecuteOperationException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    unhandledExecptionHandler(methodInfo.GetCurrentMethodName(), exception);
                }
            }
            return null;
        }

        public IEnumerable<DataEntity> ProcessMessage(string entityName, string message)
        {
            throw new NotImplementedException();
        }

        public MethodResult ExecuteMethod(MethodInput input)
        {
            throw new NotImplementedException();
        }
    }
        #endregion 
}
