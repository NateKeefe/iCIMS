using System;
using System.Collections.Generic;

using Scribe.Core.ConnectorApi.Exceptions;
using Scribe.Core.ConnectorApi.ConnectionUI;
using Scribe.Core.ConnectorApi.Cryptography;

namespace CDK
{
    public static class ConnectionHelper
    {
        #region Constants

        public class ConnectionProperties
        {
            public string BaseUrl { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string CustomerId { get; set; }
            public string HMAC { get; set; }
        }

        public static class ConnectionPropertyKeys
        {
            public const string BaseUrl = "BaseUrl";
            public const string Username = "Username";
            public const string Password = "Password";
            public const string CustomerId = "CustomerId";
            public const string HMAC = "HMAC";
        }

        public static class ConnectionPropertyLabels
        {
            public const string BaseUrl = "Base Url";
            public const string Username = "Username";
            public const string Password = "Password";
            public const string CustomderId = "Customer Id";
            public const string HMAC = "HMAC";
        }

        private const string HelpLink = "https://success.scribesoft.com/s/";

        #endregion

        public static FormDefinition GetConnectionFormDefintion()
        {

            var formDefinition = new FormDefinition
            {
                CompanyName = Connector.CompanyName,
                CryptoKey = Connector.CryptoKey,
                HelpUri = new Uri(HelpLink)
            };

            //Add fields and order to Connection UI
            formDefinition.Add(BuildUrlDefinition(0));
            formDefinition.Add(BuildUserDefinition(1));
            formDefinition.Add(BuildPasswordDefinition(2));
            formDefinition.Add(BuildCustomerIdDefinition(3));
            formDefinition.Add(BuildHMACDefinition(4));

            return formDefinition;
        }

        private static EntryDefinition BuildUrlDefinition(int order)
        {
            var entryDefinition = new EntryDefinition
            {
                InputType = InputType.Text,
                IsRequired = true,
                Label = ConnectionPropertyLabels.BaseUrl,
                PropertyName = ConnectionPropertyKeys.BaseUrl,
                Order = order,
            };

            return entryDefinition;
        }

        private static EntryDefinition BuildUserDefinition(int order)
        {
            var entryDefinition = new EntryDefinition
            {
                InputType = InputType.Text,
                IsRequired = true,
                Label = ConnectionPropertyLabels.Username,
                PropertyName = ConnectionPropertyKeys.Username,
                Order = order,
            };

            return entryDefinition;
        }

        private static EntryDefinition BuildPasswordDefinition(int order)
        {
            var entryDefinition = new EntryDefinition
            {
                InputType = InputType.Password,
                IsRequired = true,
                Label = ConnectionPropertyLabels.Password,
                PropertyName = ConnectionPropertyKeys.Password,
                Order = order,
            };

            return entryDefinition;
        }

        private static EntryDefinition BuildCustomerIdDefinition(int order)
        {
            var entryDefinition = new EntryDefinition
            {
                InputType = InputType.Text,
                IsRequired = true,
                Label = ConnectionPropertyLabels.CustomderId,
                PropertyName = ConnectionPropertyKeys.CustomerId,
                Order = order,
            };

            return entryDefinition;
        }

        private static EntryDefinition BuildHMACDefinition(int order)
        {
            var entryDefinition = new EntryDefinition
            {
                InputType = InputType.Text,
                IsRequired = true,
                Label = ConnectionPropertyLabels.HMAC,
                PropertyName = ConnectionPropertyKeys.HMAC,
                Order = order,
            };

            entryDefinition.Options.Add("Enabled", "Enabled");
            entryDefinition.Options.Add("Disabled", "Disabled");

            return entryDefinition;
        }
    }
}