[![CI](https://github.com/reportportal/agent-dotnet-xunit.v3/actions/workflows/ci.yml/badge.svg)](https://github.com/reportportal/agent-dotnet-xunit.v3/actions/workflows/ci.yml)

# ReportPortal integration for xUnit v3

## Important Note about xUnit v3 Custom Reporters

According to xUnit v3 documentation, custom runner reporters are only supported by the in-process console runner. This means:

- Custom reporters work only when directly running the test project (by directly invoking the test project .exe or when using `dotnet run`)
- Custom reporters are **not supported** by multi-assembly runners like:
  - xunit.v3.runner.console
  - xunit.v3.runner.msbuild
  - xunit.runner.visualstudio (which means they don't work with `dotnet test` or Test Explorer)

This is a limitation of xUnit v3's design, where test projects are stand-alone executables.

## Installation
Install `ReportPortal.XUnit.V3` NuGet package in your xUnit v3 test project.

[![NuGet Badge](https://buildstats.info/nuget/reportportal.xunit.v3)](https://www.nuget.org/packages/reportportal.xunit.v3)

## Configuration
Add `ReportPortal.json` file to the test project.

```json
{
  "$schema": "https://raw.githubusercontent.com/reportportal/agent-dotnet-xunit.v3/main/ReportPortal.config.schema",
  "enabled": true,
  "server": {
    "url": "https://rp.epam.com/api/v1/",
    "project": "default_project",
    "apiKey": "<your_rp_api_key_here>"
  },
  "launch": {
    "name": "XUnit Demo Launch",
    "description": "this is description",
    "debugMode": false,
    "attributes": [ "t1", "os:win10" ]
  }
}
```

Read [more](https://github.com/reportportal/commons-net/blob/master/docs/Configuration.md) about configuration of other available options and alternative ways how to provide options.


### Important Limitations

⚠️ The following methods of running tests **will not work** with the ReportPortal reporter due to xUnit v3 limitations:

- `dotnet test`
- `dotnet vstest`
- `vstest.console.exe`
- Visual Studio Test Explorer
- xunit.v3.runner.console.exe

This is because these runners do not support custom reporters in xUnit v3.


## Additional Resources

- [xUnit v3 Documentation](https://xunit.net/docs/v3-alpha)
- [ReportPortal Documentation](https://reportportal.io/docs)


## Integrating Logger Frameworks with xUnit v3

You can integrate various logging frameworks with ReportPortal:

- [NLog](https://github.com/reportportal/logger-net-nlog)
- [log4net](https://github.com/reportportal/logger-net-log4net)
- [Serilog](https://github.com/reportportal/logger-net-serilog)
- [System.Diagnostics.TraceListener](https://github.com/reportportal/logger-net-tracelistener)

See [here](https://github.com/reportportal/commons-net/blob/master/docs/Logging.md) for more information on how to improve your logging experience with attachments or nested steps.


## Useful Extensions
- [Skippable](https://github.com/nvborisenko/reportportal-extensions-skippable) marks skipped tests as `No Defect` automatically
- [SourceBack](https://github.com/nvborisenko/reportportal-extensions-sourceback) adds piece of test code where test was failed
- [Insider](https://github.com/nvborisenko/reportportal-extensions-insider) brings more reporting capabilities without coding like methods invocation as nested steps


# License
ReportPortal is licensed under [Apache 2.0](./LICENSE)

We use Google Analytics for sending anonymous usage information as library's name/version and the agent's name/version when starting launch. This information might help us to improve integration with ReportPortal. Used by the ReportPortal team only and not for sharing with 3rd parties. You are able to [turn off](https://github.com/reportportal/commons-net/blob/master/docs/Configuration.md#analytics) it if needed.
