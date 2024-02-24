using SimaiParserWithAntlr;

namespace ParserTest
{
    public class UnitTest1
    {
        [Fact]
        public void FooTester()
        {
            var obj = new FooTest();

            obj.A = 5;
            obj.B = 12;

            int result = obj.Foo();

            Assert.Equal(17, result);
        }
    }
}
