namespace ReportPortal.XUnitReporter.IntegrationTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        Assert.True(true);

        TestContext.Current.TestOutputHelper?.WriteLine("This is a test output message via TestContext.");
        TestContext.Current.TestOutputHelper?.WriteLine("Another log message for the test.");

        Shared.Context.Current.Log.Info("This is a log message from ReportPortal context.");

        using (var scope = Shared.Context.Current.Log.BeginScope("my scope 1"))
        {
            Shared.Context.Current.Log.Info("This is a log message from ReportPortal Scoped context.");
        }
    }

    [Fact]
    public void Test2()
    {
        Assert.True(true);
    }
}
