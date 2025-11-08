using ReportPortal.Client.Abstractions.Models;
using ReportPortal.Client.Abstractions.Requests;
using ReportPortal.Shared.Configuration;
using ReportPortal.Shared.Reporter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Xunit.Runner.Common;
using Xunit.Sdk;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        /// <summary>
        /// Starting connect to report portal. Create launcher and start it.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void TestAssemblyExecutionStarting(MessageHandlerArgs<ITestAssemblyStarting> args)
        {
            try
            {
                LaunchMode launchMode = _config.GetValue(ConfigurationPath.LaunchDebugMode, false) ? LaunchMode.Debug : LaunchMode.Default;

                var startLaunchRequest = new StartLaunchRequest
                {
                    Name = _config.GetValue(ConfigurationPath.LaunchName, args.Message.AssemblyName),
                    StartTime = DateTime.UtcNow,
                    Mode = launchMode,
                    Attributes = _config.GetKeyValues("Launch:Attributes", new List<KeyValuePair<string, string>>()).Select(a => new ItemAttribute { Key = a.Key, Value = a.Value }).ToList(),
                    Description = _config.GetValue(ConfigurationPath.LaunchDescription, "")
                };

                Shared.Extensibility.Embedded.Analytics.AnalyticsReportEventsObserver.DefineConsumer("agent-dotnet-xunit.v3");

                _launchReporter = new LaunchReporter(_service, _config, null, Shared.Extensibility.ExtensionManager.Instance);
                _launchReporter.Start(startLaunchRequest);

                Logger.LogMessage("[Report Portal Agent] Start sending messages to Report Portal server");
                Logger.LogMessage("[Report Portal Agent] URL: " + _config.GetValue(ConfigurationPath.ServerUrl, "") + " - Project: " + _config.GetValue(ConfigurationPath.ServerProject, ""));

            }
            catch (Exception exp)
            {
                Logger.LogError(exp.ToString());
            }
        }


        protected virtual void TestAssemblyExecutionFinished(MessageHandlerArgs<ITestAssemblyFinished> args)
        {
            try
            {
                _launchReporter.Finish(new FinishLaunchRequest { EndTime = DateTime.UtcNow });

                Logger.LogMessage("[Report Portal Agent] Waiting to finish sending all results to Report Portal server.");

                var stopWatch = Stopwatch.StartNew();

                //log a message saying "we're still doing stuff", to avoid the appearance of hung builds 
                using (new Timer(
                           _ => Logger.LogMessage($"[Report Portal Agent] Still sending results to Report Portal server..."),
                           null,
                           TimeSpan.FromMinutes(1),
                           Timeout.InfiniteTimeSpan))
                {
                    _launchReporter.Sync();
                }

                Logger.LogMessage($"[Report Portal Agent] Results are sent to Report Portal server. Sync duration: {stopWatch.Elapsed}");
                Logger.LogMessage(_launchReporter.StatisticsCounter.ToString());
            }
            catch (Exception exp)
            {
                Logger.LogWarning(exp.ToString());
            }
        }
    }
}
