using ReportPortal.Shared.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit.Runner.Common;
using Xunit.Sdk;

namespace ReportPortal.XUnitReporter;

public class ReportPortalReporter : IRunnerReporter
{
    private readonly IConfiguration _config;

    public ReportPortalReporter()
    {
        var currentDirectory = Path.Combine(Path.GetDirectoryName(new Uri(typeof(ReportPortalReporter).Assembly.Location).LocalPath));

        _config = new ConfigurationBuilder().AddDefaults(currentDirectory).Build();
    }

    public ValueTask<IRunnerReporterMessageHandler> CreateMessageHandler(IRunnerLogger logger, IMessageSink diagnosticMessageSink)
    {
        return new ValueTask<IRunnerReporterMessageHandler>(new ReportPortalReporterMessageHandler(logger, _config, diagnosticMessageSink));
    }

    public bool CanBeEnvironmentallyEnabled => true;

    public string Description => "Reporting tests results to Report Portal";

    public bool ForceNoLogo => false;

    public bool IsEnvironmentallyEnabled => _config.GetValue("enabled", true);

    public string RunnerSwitch => "reportportal";
}
