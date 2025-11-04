using ReportPortal.Client.Abstractions.Models;
using ReportPortal.Client.Abstractions.Requests;
using ReportPortal.Shared.Reporter;
using System;
using Xunit.Runner.Common;
using Xunit.Sdk;

namespace ReportPortal.XUnitReporter.V3
{
    public partial class ReportPortalReporterMessageHandler
    {
        /// <summary>
        /// Starting test suite in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected new virtual void HandleTestCollectionStarting(MessageHandlerArgs<ITestCollectionStarting> args)
        {
            try
            {
                var testCollection = args.Message;
                string key = testCollection.TestCollectionUniqueID;

                ITestReporter testReporter = _launchReporter.StartChildTestReporter(
                    new StartTestItemRequest()
                    {
                        Name = testCollection.TestCollectionDisplayName,
                        StartTime = DateTime.UtcNow,
                        Type = TestItemType.Suite
                    });

                TestReporterDictionary[key] = testReporter;
            }
            catch (Exception exp)
            {
                Logger.LogError(exp.ToString());
            }
        }

        /// <summary>
        /// Finishing test suite in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected new virtual void HandleTestCollectionFinished(MessageHandlerArgs<ITestCollectionFinished> args)
        {
            try
            {
                var testCollection = args.Message;
                string key = testCollection.TestCollectionUniqueID;

                TestReporterDictionary[key].Finish(new FinishTestItemRequest()
                {
                    EndTime = DateTime.UtcNow,
                    Status = testCollection.TestsFailed > 0 ? Status.Failed : Status.Passed
                });
            }
            catch (Exception exp)
            {
                Logger.LogError(exp.ToString());
            }
        }
    }
}
