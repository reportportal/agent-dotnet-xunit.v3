

namespace Xunit
{
    public static class OutputHelperExtensions
    {
        public static ITestOutputHelper WithReportPortal(this ITestOutputHelper outputHelper)
        {
            ReportPortal.XUnitReporter.V3.LogHandler.LogHandler.XunitTestOutputHelper = outputHelper;

            return outputHelper;
        }
    }
}
