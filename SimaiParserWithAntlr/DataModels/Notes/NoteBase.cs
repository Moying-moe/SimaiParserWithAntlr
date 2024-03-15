using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SimaiParserWithAntlr.Enums;

namespace SimaiParserWithAntlr.DataModels.Notes
{

    /**
     * Why do we need a new `NoteBase` class even though we already have `ParserNoteBase`?
     * This is because `ParserNoteBase` is specific to the parser of Note Block Layer,
     * and it is related to the simai syntax and the implementation of the parser.
     * 
     * However, `NoteBase` here is not related to simai and the syntax parser.
     * It is a language-agnostic data class, rather than being specific to any particular chart description syntax
     * (like simai).
     * Therefore, we need to use two classes that may seem somewhat similar.
     * This helps to isolate the specific details of the syntax parser.
     * 
     * If there are changes to the implementation details of the syntax parser,
     * `ParserNoteBase` is likely to change as well, but `NoteBase` will not change, achieving isolation.
     */
    public abstract class NoteBase
    {
        protected NoteBase(NoteTypeEnum type, double hiSpeed)
        {
            Type = type;
            HiSpeed = hiSpeed;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public NoteTypeEnum Type { get; set; }

        public double HiSpeed { get; set; }
    }
}