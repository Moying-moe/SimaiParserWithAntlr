lexer grammar StructureLexer;

BPM_START               : '(' -> pushMode(BPM) ;
RESOLUTION_START        : '{' -> pushMode(RESOLUTION) ;
COMMENT_SYMBOL          : '||' ;

H_SPEED_START           : '<HS*' -> pushMode(H_SPEED) ;
MAML_OPEN_START         : '<-' -> pushMode(MAML_OPEN) ;
MAML_CLOSE_START        : '</' -> pushMode(MAML_CLOSE) ;

NEWLINE                 : '\r\n' | [\r\n] ;
ANY                     : . ;

mode BPM ;
BPM_END                 : ')' -> popMode ;
BPM_NUMBER  			: ([1-9] [0-9]* | '0') ('.' [0-9]+)? ;

mode RESOLUTION ;
RESOLUTION_END          : '}' -> popMode ;
// For compatibility reasons, decimal numbers and 0 are accepted as beat resolutions here.
RESOLUTION_NUMBER		: ([1-9] [0-9]*) ('.' [0-9]+)? ;

mode H_SPEED ;
H_SPEED_END             : '>' -> popMode ;
H_SPEED_NUMBER          : ([1-9] [0-9]* | '0') ('.' [0-9]+)? ;

mode MAML_OPEN ;
MAML_OPEN_END           : '>' -> popMode ;
MAML_NUMBER             : ([1-9] [0-9]* | '0') ('.' [0-9]+)? ;
MAML_EQUAL              : '=' ;
MAML_QUOTE              : '"' -> pushMode(STRING) ;
MAML_WS                 : [ \t\u00A0\r\n\u2028\u2029] ;
MAML_OPEN_WORD          : [a-zA-Z] [a-zA-Z0-9]* ;

mode MAML_CLOSE ;
MAML_CLOSE_END          : '>' -> popMode ;
MAML_CLOSE_WORD         : [a-zA-Z] [a-zA-Z0-9]* ;

mode STRING ;
STRING_END              : '"' -> popMode ;
STRING_ANY              : . ;