lexer grammar NoteBlockLexer;

COMMA					: ',' ;
EACH_SEPARATOR			: '/' ;
FAKE_EACH_SEPARATOR		: '`' ;

BUTTON					: [1-8] ;
AREA					: [ABDE] [1-8] | 'C' | 'C1' | 'C2' ;

BREAK_MARK				: 'b' ;
EX_MARK					: 'x' ;
HOLD_MARK				: 'h' ;
FIREWORK_MARK			: 'f' ;

// Although `V` requires 2 slide end positions like `1V36`,
// we treat `V` the same as other slide types and check this rule during semantic analysis.
// Introducing special processing for 'V' would unnecessarily increase complexity.
SLIDE_TYPE				: [-<>^vVpqw] | 'pp' | 'qq' ;
SLIDE_SAME_HEAD_MARK	: '*' ;

DURATION_START			: '[' -> pushMode(DURATION) ;

WS						: [ \t\n\r\u2028\u2029] -> skip ;


// Since duration contains numbers and could be ambiguous with the button token,
// we need this DURATION mode.
mode DURATION ;
DURATION_END			: ']' -> popMode ;
COLON					: ':' ;
HASHTAG					: '#' ;
// Additionally, rather than handling the differences between floats and ints in the lexer,
// we handle them in the semantic analysis.
NUMBER					: ([1-9] [0-9]* | '0') ('.' [0-9]+)? ;
