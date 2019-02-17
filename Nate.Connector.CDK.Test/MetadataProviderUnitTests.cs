using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Scribe.Core.ConnectorApi;
using Scribe.Core.ConnectorApi.Exceptions;

namespace CDK
{
    [TestClass]
    public class MetadataProviderUnitTests : ConnectedUnitTestBase
    {
        #region setup and tear down of test class

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            setup(context);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            cleanup();
        }

        #endregion

        [TestMethod]
        public void MP_ResetMetadata()
        {
            connector.GetMetadataProvider().ResetMetadata();
        }

        [TestMethod]
        public void MP_RetrieveActionDefinitions()
        {
            var actionDefs = connector.GetMetadataProvider().RetrieveActionDefinitions();

            Assert.IsNotNull(actionDefs);

            var actionDefList = actionDefs.ToList();
            Assert.IsTrue(actionDefList.Count == 2);
        }

        [TestMethod]
        public void MP_RetrieveObjectDefinitionsAndProps()
        {
            var objDefs = connector.GetMetadataProvider().RetrieveObjectDefinitions(true);

            Assert.IsNotNull(objDefs);

            var objDefsList = objDefs.ToList();
            Assert.IsTrue(objDefsList.Count == 1);
        }

        [TestMethod]
        public void MP_RetrieveObjectDefinitionsNoProps()
        {
            var objDefs = connector.GetMetadataProvider().RetrieveObjectDefinitions(false);

            Assert.IsNotNull(objDefs);

            var objDefsList = objDefs.ToList();
            Assert.IsTrue(objDefsList.Count == 1);
        }

        [TestMethod]
        public void MP_RetrieveObjectDefinitionAndProps()
        {
            var objDef = connector.GetMetadataProvider().RetrieveObjectDefinition("Folder", true);

            Assert.IsNotNull(objDef);
        }

        [TestMethod]
        public void MP_RetrieveObjectDefinitionNoProps()
        {
            var objDef = connector.GetMetadataProvider().RetrieveObjectDefinition("Folder", false);

            Assert.IsNotNull(objDef);
        }
    }
}
