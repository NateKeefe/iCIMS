using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Scribe.Core.ConnectorApi;
using Scribe.Core.ConnectorApi.Query;
using Scribe.Core.ConnectorApi.Actions;

namespace CDK
{
    [TestClass]
    public class OperationUnitTests : ConnectedOperationUnitTestBase
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

    //    [TestMethod]
    //    public void OP_Execute_Sleep_NULL()
    //    {
    //        var operationInput = new OperationInput
    //        {
    //            Name = "Execute",
    //            Input = new DataEntity[]
    //            {
    //                new DataEntity(ConnectorService.ObjectNames.Sleep)
    //                {
    //                    Properties =
    //                    {
    //                        {ConnectorService.SleepObjPropNames.Sleeptime, null},
    //                    }
    //                }
    //            }
    //        };

    //        var results = connector.ExecuteOperation(operationInput);

    //        ExpectSuccess(results);
    //    }

    //    [TestMethod]
    //    public void OP_Execute_Sleep_Empty()
    //    {
    //        var operationInput = new OperationInput
    //        {
    //            Name = "Execute",
    //            Input = new DataEntity[]
    //            {
    //                new DataEntity(ConnectorService.ObjectNames.Sleep)
    //                {
    //                    Properties =
    //                    {
    //                        {ConnectorService.SleepObjPropNames.Sleeptime, ""},
    //                    }
    //                }
    //            }
    //        };

    //        var results = connector.ExecuteOperation(operationInput);

    //        ExpectSuccess(results);
    //    }

    //    [TestMethod]
    //    public void OP_Execute_Sleep_0()
    //    {
    //        var operationInput = new OperationInput
    //        {
    //            Name = "Execute",
    //            Input = new DataEntity[]
    //            {
    //                new DataEntity(ConnectorService.ObjectNames.Sleep)
    //                {
    //                    Properties =
    //                    {
    //                        {ConnectorService.SleepObjPropNames.Sleeptime, 0},
    //                    }
    //                }
    //            }
    //        };

    //        var results = connector.ExecuteOperation(operationInput);

    //        ExpectSuccess(results);
    //    }

    //    [TestMethod]
    //    public void OP_Execute_Sleep_Neg1()
    //    {
    //        var operationInput = new OperationInput
    //        {
    //            Name = "Execute",
    //            Input = new DataEntity[]
    //            {
    //                new DataEntity(ConnectorService.ObjectNames.Sleep)
    //                {
    //                    Properties =
    //                    {
    //                        {ConnectorService.SleepObjPropNames.Sleeptime, -1},
    //                    }
    //                }
    //            }
    //        };

    //        var results = connector.ExecuteOperation(operationInput);

    //        ExpectError(results);
    //    }

    //    [TestMethod]
    //    public void OP_Execute_Sleep_OverMax()
    //    {
    //        var operationInput = new OperationInput
    //        {
    //            Name = "Execute",
    //            Input = new DataEntity[]
    //            {
    //                new DataEntity(ConnectorService.ObjectNames.Sleep)
    //                {
    //                    Properties =
    //                    {
    //                        {ConnectorService.SleepObjPropNames.Sleeptime, 60001},
    //                    }
    //                }
    //            }
    //        };

    //        var results = connector.ExecuteOperation(operationInput);

    //        ExpectError(results);
    //    }

    //    [TestMethod]
    //    public void OP_Execute_Sleep_500()
    //    {
    //        testValidSleepTime(500);
    //    }

    //    [TestMethod]
    //    public void OP_Execute_Sleep_1000()
    //    {
    //        testValidSleepTime(1000);
    //    }

    //    [TestMethod]
    //    public void OP_Execute_Sleep_2500()
    //    {
    //        testValidSleepTime(2500);
    //    }

    //    [TestMethod]
    //    public void OP_Execute_Sleep_5000()
    //    {
    //        testValidSleepTime(5000);
    //    }

    //    private void testValidSleepTime(object sleepTime)
    //    {
    //        var operationInput = new OperationInput
    //        {
    //            Name = "Execute",
    //            Input = new DataEntity[]
    //                            {
    //                new DataEntity(ConnectorService.ObjectNames.Sleep)
    //                {
    //                    Properties =
    //                    {
    //                        {ConnectorService.SleepObjPropNames.Sleeptime, sleepTime},
    //                    }
    //                }
    //                            }
    //        };

    //        var start = DateTime.Now;

    //        var results = connector.ExecuteOperation(operationInput);

    //        ExpectSuccess(results);

    //        var sleepTimeMS = int.Parse(sleepTime.ToString());

    //        var durration = DateTime.Now - start;
    //        Assert.IsTrue(durration.TotalMilliseconds - sleepTimeMS > 0);
    //        Assert.IsTrue(durration.TotalMilliseconds - sleepTimeMS < 200);
    //    }
    }
}
