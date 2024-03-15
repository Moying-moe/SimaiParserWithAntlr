using System;
namespace SimaiParserWithAntlr.NoteLayerParser.Exceptions
{

    public class UnsupportedPropInCurrentSlideType : Exception
    {
        public UnsupportedPropInCurrentSlideType() : this("Unsupported property in current slide type!")
        {
        }

        public UnsupportedPropInCurrentSlideType(string? message) : base(message)
        {
        }

        public UnsupportedPropInCurrentSlideType(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}