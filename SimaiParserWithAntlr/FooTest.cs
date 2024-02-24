namespace SimaiParserWithAntlr
{
    public class FooTest
    {
        public int A { get; set; }
        public int B { get; set; }

        public FooTest()
        {
            A = 0;
            B = 0;
        }

        public FooTest(int a, int b)
        {
            A = a;
            B = b;
        }

        public int Foo()
        {
            return A + B;
        }
    }
}
