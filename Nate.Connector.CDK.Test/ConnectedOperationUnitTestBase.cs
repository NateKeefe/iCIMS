using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scribe.Core.ConnectorApi.Actions;

namespace CDK
{
    public abstract class ConnectedOperationUnitTestBase: ConnectedUnitTestBase
    {
        protected static void ExpectSuccess(OperationResult results)
        {
            Assert.IsNotNull(results);
            Assert.IsNotNull(results.Success);
            Assert.IsNotNull(results.ObjectsAffected);
            Assert.IsNotNull(results.Output);
            Assert.IsNotNull(results.ErrorInfo);
            Assert.AreEqual(1, results.Success.Count());
            Assert.IsTrue(results.Success[0]);
            Assert.AreEqual(1, results.ObjectsAffected.Count());
            Assert.AreEqual(1, results.ObjectsAffected[0]);
            Assert.AreEqual(1, results.Output.Count());
            Assert.AreEqual(1, results.ErrorInfo.Count());
            Assert.IsNull(results.ErrorInfo[0]);
        }

        protected static void ExpectError(OperationResult results, string expectedErrorDescription = null)
        {
            Assert.IsNotNull(results);
            Assert.IsNotNull(results.Success);
            Assert.IsNotNull(results.ObjectsAffected);
            Assert.IsNotNull(results.Output);
            Assert.IsNotNull(results.ErrorInfo);
            Assert.AreEqual(1, results.Success.Count());
            Assert.IsFalse(results.Success[0]);
            Assert.AreEqual(1, results.ObjectsAffected.Count());
            Assert.AreEqual(0, results.ObjectsAffected[0]);
            Assert.AreEqual(1, results.Output.Count());
            Assert.AreEqual(1, results.ErrorInfo.Count());
            Assert.IsNotNull(results.ErrorInfo[0]);
            if (expectedErrorDescription != null)
                Assert.AreEqual(expectedErrorDescription, results.ErrorInfo[0].Description);
        }
    }
}

