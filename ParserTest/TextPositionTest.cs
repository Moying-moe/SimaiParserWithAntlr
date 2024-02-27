using SimaiParserWithAntlr.DataModels;
using Xunit.Abstractions;

namespace ParserTest;

public class TextPositionTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TextPositionTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test1()
    {
        string text = "0123456789\nabcdefghij\nklmnopqrst\nuvwxyz";

        var range1 = new TextPositionRange(
            new TextPosition(1, 4),
            new TextPosition(3, 8)
        );
        var range2 = new TextPositionRange(
            new TextPosition(1, 4),
            new TextPosition(1, 5)
        );
        var range3 = new TextPositionRange(
            new TextPosition(2, 5),
            new TextPosition(4, 1)
        );

        _testOutputHelper.WriteLine(range1.GetPositionedString(text, 3, 3, true, true) + "\n");
        _testOutputHelper.WriteLine(range2.GetPositionedString(text, 3, 3, true, true) + "\n");
        _testOutputHelper.WriteLine(range3.GetPositionedString(text, 3, 3, true, true) + "\n");
    }
}
