using System;
using System.Collections.Generic;
using Scribe.Core.ConnectorApi;
using Scribe.Core.ConnectorApi.Exceptions;

namespace CDK.Common
{
    public static class ExpressionParser
    {
        public static Dictionary<string, object> GetMatchCriteria(Expression expression)
        {
            var matchCriteria = new Dictionary<string, object>();
            ParseExpression(expression, matchCriteria);

            return matchCriteria;
        }

        private static void ParseExpression(Expression expression, Dictionary<string, object> matchCriteria)
        {
            if (expression != null)
            {
                switch (expression.ExpressionType)
                {
                    case ExpressionType.Comparison:
                        var comparisonExpression = (ComparisonExpression)expression;

                        EnsureValidComparisonExpression(comparisonExpression);

                        var keyValuePair = ParseComparisonExpression(comparisonExpression);

                        if (!matchCriteria.ContainsKey(keyValuePair.Key))
                        {
                            matchCriteria.Add(keyValuePair.Key, keyValuePair.Value);
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format(StringMessages.DuplicateFilterField, keyValuePair.Key));
                        }

                        break;
                    case ExpressionType.Logical:
                        var logicalExpression = (LogicalExpression)expression;

                        EnsureValidLogicalExpression(logicalExpression);

                        ParseExpression(logicalExpression.LeftExpression, matchCriteria);

                        ParseExpression(logicalExpression.RightExpression, matchCriteria);
                        break;
                }
            }
        }

        private static KeyValuePair<string, object> ParseComparisonExpression(ComparisonExpression comparisonExpression)
        {
            var fieldName = GetFieldName(comparisonExpression.LeftValue.Value.ToString());
            var dataValue = comparisonExpression.RightValue.Value;

            var filter = new KeyValuePair<string, object>(fieldName, dataValue);

            return filter;
        }

        private static string GetFieldName(string qualifiedPropertyName)
        {
            var fieldName = qualifiedPropertyName;

            if (qualifiedPropertyName != null && qualifiedPropertyName.Contains("."))
            {
                var names = qualifiedPropertyName.Split('.');
                fieldName = names[1];
            }

            return fieldName;
        }

        private static void EnsureValidLogicalExpression(LogicalExpression logicalExpression)
        {
            if (logicalExpression == null)
            {
                throw new InvalidExecuteQueryException(StringMessages.ErrorInvalidLogicalExpression);
            }

            if (logicalExpression.LeftExpression == null)
            {
                throw new InvalidExecuteQueryException(StringMessages.ErrorNullLeftValue);
            }

            if (logicalExpression.RightExpression == null)
            {
                throw new InvalidExecuteQueryException(StringMessages.ErrorNullRightValue);
            }
        }

        private static void EnsureValidComparisonExpression(ComparisonExpression comparisonExpression)
        {
            if (comparisonExpression == null)
            {
                throw new InvalidExecuteQueryException(StringMessages.ErrorInvalidComparisionExpression);
            }

            if (comparisonExpression.LeftValue?.Value == null)
            {
                throw new InvalidExecuteQueryException(StringMessages.ErrorNullLeftValue);
            }

            if (comparisonExpression.Operator != ComparisonOperator.Equal)
            {
                string operatorString;

                switch (comparisonExpression.Operator)
                {
                    case ComparisonOperator.Greater:
                        operatorString = "Greater Than";
                        break;
                    case ComparisonOperator.Less:
                        operatorString = "Less Than";
                        break;
                    default:
                        operatorString = comparisonExpression.Operator.ToString();
                        break;
                }

                throw new InvalidExecuteQueryException(string.Format(StringMessages.ErrorInvalidLogicalExpression, operatorString, comparisonExpression.LeftValue?.Value));
            }
        }
    }
}
