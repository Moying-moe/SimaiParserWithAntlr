namespace SimaiParserWithAntlr.NoteLayerParser.Exceptions
{

    public class UnsupportedPropInCurrentDurationType : Exception
    {
        public UnsupportedPropInCurrentDurationType() : this("Unsupported property in current duration type!")
        {
        }

        public UnsupportedPropInCurrentDurationType(string? message) : base(message)
        {
        }

        public UnsupportedPropInCurrentDurationType(string? message, Exception? innerException) : base(message,
            innerException)
        {
        }
    }
}
