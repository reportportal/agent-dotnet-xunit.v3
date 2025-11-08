namespace ReportPortal.XUnitReporter.IntegrationTests;

public class UnitTest1
{
    private ITestOutputHelper _testOutputHelper;

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper.WithReportPortal();
    }

    [Fact]
    public void Test1()
    {
        Assert.True(true);

        Shared.Context.Current.Log.Info("This is a log message from ReportPortal context.");

        _testOutputHelper.WriteLine("This is a test log message.");
    }

    [Fact]
    public void Test2()
    {
        Assert.True(true);
    }
}
