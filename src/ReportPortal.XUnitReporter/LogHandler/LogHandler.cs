using ReportPortal.Shared;
using ReportPortal.Shared.Execution.Logging;
using ReportPortal.Shared.Extensibility;
using ReportPortal.Shared.Extensibility.Commands;
using ReportPortal.Shared.Internal.Logging;
using ReportPortal.XUnitReporter.LogHandler.Messages;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Xunit;
using Xunit.Runner.Common;

namespace ReportPortal.XUnitReporter.LogHandler;

class LogHandler : ICommandsListener
{
    private static readonly ITraceLogger TraceLogger;

    public void Initialize(ICommandsSource commandsSource)
    {
        commandsSource.OnBeginLogScopeCommand += CommandsSource_OnBeginLogScopeCommand;
        commandsSource.OnEndLogScopeCommand += CommandsSource_OnEndLogScopeCommand;
        commandsSource.OnLogMessageCommand += CommandsSource_OnLogMessageCommand;
    }

    static LogHandler()
    {
        var currentDirectory = Path.Combine(Path.GetDirectoryName(new Uri(typeof(ReportPortalReporter).Assembly.Location).LocalPath));

        TraceLogger = TraceLogManager.Instance.WithBaseDir(currentDirectory).GetLogger<LogHandler>();
    }

    private void CommandsSource_OnLogMessageCommand(Shared.Execution.ILogContext logContext, Shared.Extensibility.Commands.CommandArgs.LogMessageCommandArgs args)
    {
        var rootScope = Context.Current.Log.Root;

        TraceLogger.Verbose($"Handling log message for {rootScope.GetHashCode()} root scope...");

        var logScope = args.LogScope;

        var outputHelper = TestContext.Current.TestOutputHelper;

        if (outputHelper != null)
        {
            var logRequest = args.LogMessage.ConvertToRequest();

            var sharedLogMessage = new AddLogCommunicationMessage
            {
                ParentScopeId = logScope?.Id,
                Level = logRequest.LevelString,
                Time = logRequest.Time,
                Text = logRequest.Text
            };

            if (logRequest.Attach != null)
            {
                sharedLogMessage.Attach = new Attach(logRequest.Attach.MimeType, logRequest.Attach.Data);
            }

            NotifyAgent(outputHelper, JsonSerializer.Serialize(sharedLogMessage));
        }
    }

    private void CommandsSource_OnEndLogScopeCommand(Shared.Execution.ILogContext logContext, Shared.Extensibility.Commands.CommandArgs.LogScopeCommandArgs args)
    {
        var logScope = args.LogScope;

        var outputHelper = TestContext.Current.TestOutputHelper;

        if (outputHelper != null)
        {
            var communicationMessage = new EndScopeCommunicationMessage
            {
                Id = logScope.Id,
                EndTime = logScope.EndTime.Value,
                Status = logScope.Status
            };

            NotifyAgent(outputHelper, JsonSerializer.Serialize(communicationMessage));
        }
    }

    private void CommandsSource_OnBeginLogScopeCommand(Shared.Execution.ILogContext logContext, Shared.Extensibility.Commands.CommandArgs.LogScopeCommandArgs args)
    {
        var logScope = args.LogScope;

        var outputHelper = TestContext.Current.TestOutputHelper;

        if (outputHelper != null)
        {
            var communicationMessage = new BeginScopeCommunicationMessage
            {
                Id = logScope.Id,
                ParentScopeId = logScope.Parent?.Id,
                Name = logScope.Name,
                BeginTime = logScope.BeginTime
            };

            NotifyAgent(outputHelper, JsonSerializer.Serialize(communicationMessage));
        }
    }

    private static void NotifyAgent(ITestOutputHelper outputHelper, string serializedMessage)
    {
        var type = outputHelper.GetType();

        var stateMember = type.GetField("state", BindingFlags.Instance | BindingFlags.NonPublic);
        var state = stateMember.GetValue(outputHelper);
        var stateType = state.GetType();

        var messageBusMember = stateType.GetField("messageBus", BindingFlags.Instance | BindingFlags.NonPublic);
        var messageBusValue = messageBusMember.GetValue(state);

        var messageBusType = messageBusValue.GetType();
        var queueMessageMethod = messageBusType.GetMethod("QueueMessage", BindingFlags.Instance | BindingFlags.Public);

        var iTest = TestContext.Current.Test;

        var testOutput = new TestOutput
        {
            TestUniqueID = iTest.UniqueID,
            TestCaseUniqueID = iTest.TestCase.UniqueID,
            TestMethodUniqueID = iTest.TestCase.TestMethod.UniqueID,
            TestClassUniqueID = iTest.TestCase.TestMethod.TestClass.UniqueID,
            TestCollectionUniqueID = iTest.TestCase.TestMethod.TestClass.TestCollection.UniqueID,
            AssemblyUniqueID = iTest.TestCase.TestMethod.TestClass.TestCollection.TestAssembly.UniqueID,
            Output = serializedMessage
        };

        queueMessageMethod.Invoke(messageBusValue, [testOutput]);
    }
}
