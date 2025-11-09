using ReportPortal.Client.Abstractions;
using ReportPortal.Shared.Configuration;
using ReportPortal.Shared.Reporter;
using System.Collections.Concurrent;
using Xunit.Runner.Common;
using Xunit.Sdk;

namespace ReportPortal.XUnitReporter;

partial class ReportPortalReporterMessageHandler : DefaultRunnerReporterMessageHandler, IRunnerReporterMessageHandler
{
    private readonly IConfiguration _config;
    private readonly IMessageSink _diagnosticMessageSink;

    private readonly IClientService _service;

    private LaunchReporter _launchReporter;

    protected readonly ConcurrentDictionary<string, ITestReporter> TestReporterDictionary = new();

    public ReportPortalReporterMessageHandler(IRunnerLogger logger, IConfiguration configuration, IMessageSink diagnosticMessageSink = null) : base(logger)
    {
        Logger = logger;
        _config = configuration;
        _diagnosticMessageSink = diagnosticMessageSink;

        _service = new Shared.Reporter.Http.ClientServiceBuilder(configuration).Build();

        Execution.TestAssemblyStartingEvent += TestAssemblyExecutionStarting;
        Execution.TestAssemblyFinishedEvent += TestAssemblyExecutionFinished;

        Execution.TestCollectionStartingEvent += HandleTestCollectionStarting;
        Execution.TestCollectionFinishedEvent += HandleTestCollectionFinished;

        Execution.TestStartingEvent += HandleTestStarting;
        Execution.TestPassedEvent += HandlePassed;
        Execution.TestSkippedEvent += HandleSkipped;
        Execution.TestFailedEvent += HandleFailed;

        Execution.TestOutputEvent += Execution_TestOutputEvent;
    }

    protected new IRunnerLogger Logger { get; }
}
