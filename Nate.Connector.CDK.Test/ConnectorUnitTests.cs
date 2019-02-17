using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Scribe.Core.ConnectorApi;
using Scribe.Core.ConnectorApi.Exceptions;
using Scribe.Core.ConnectorApi.Cryptography;

namespace CDK
{
    [TestClass]
    public class ConnectorUnitTests
    {
        public static Dictionary<string, string> GetTestConnectionDictionary()
        {
            var connectionDictionary = new Dictionary<string, string>();

            return connectionDictionary;
        }

        [TestMethod]
        public void CON_Construct()
        {
            IConnector connector = new Connector();

            Assert.IsNotNull(connector);
        }

        [TestMethod]
        public void CON_Preconnect()
        {
            IConnector connector = new Connector();

            var result = connector.PreConnect(null);

            Assert.IsFalse(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void CON_Connect()
        {
            IConnector connector = new Connector();

            var props = GetTestConnectionDictionary();

            connector.Connect(props);

            Assert.IsTrue(connector.IsConnected);

            if (connector.IsConnected)
                connector.Disconnect();
        }

        [TestMethod]
        public void CON_Disconnect()
        {
            IConnector connector = new Connector();

            connector.Disconnect();

        }

        [TestMethod]
        public void CON_GetMetadataProvider()
        {
            IConnector connector = new Connector();

            var props = GetTestConnectionDictionary();

            connector.Connect(props);

            Assert.IsTrue(connector.IsConnected);

            var metadataProvider = connector.GetMetadataProvider();

            Assert.IsNotNull(metadataProvider);

            if (connector.IsConnected)
                connector.Disconnect();
        }

    }
}
