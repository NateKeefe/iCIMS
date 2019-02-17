using Microsoft.VisualStudio.TestTools.UnitTesting;

using Scribe.Core.ConnectorApi;
using Scribe.Core.ConnectorApi.Exceptions;

namespace CDK
{
    [TestClass]
    [DeploymentItem(@"..\..\..\..\..\..\ConnectorDependencies\Test")]
    public abstract class ConnectedUnitTestBase
    {
        protected static readonly IConnector connector = new Connector();


        #region setup and tear down of test class

        protected static void setup(TestContext context)
        {

            connector.Connect(ConnectorUnitTests.GetTestConnectionDictionary());

            if (connector.IsConnected == false)
            {
                throw new InvalidConnectionException("Unable to connect to Connector during ClassSetup");
            }

            var metadataProvider = connector.GetMetadataProvider();
            if (metadataProvider == null)
            {
                throw new InvalidConnectionException("Null MetadataProvider during ClassSetup");
            }
        }

        protected static void cleanup()
        {
            connector.Disconnect();
            if (connector.IsConnected)
            {
                throw new InvalidConnectionException("Unable to disconnect from Connector during ClassCleanup");
            }
        }

        #endregion
    }
}
