using Xunit.Abstractions;

namespace ParserTest.I18NTest;

public class I18NTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public I18NTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void EnUsTest()
    {
        // Assert.Equal("tt1", I18N.Instance.Get(I18NKeyEnum.TestKey));
    }
}
