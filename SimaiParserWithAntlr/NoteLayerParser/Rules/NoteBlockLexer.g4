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

// Although `V` needs 2 slide end position like `1V36`
// But we treat `V` as same as other slide type, and check this rule in the sematic analysis.
// Because if we perform special processing on 'V', it will introduce unnecessary complexity.
SLIDE_TYPE				: [-<>^vVpqw] | 'pp' | 'qq' ;
SLIDE_SAME_HEAD_MARK	: '*' ;

DURATION_START			: '[' -> pushMode(DURATION) ;

WS						: [ \t\n\r\u2028\u2029] -> skip ;


// Because duration contains numbers, and will have ambiguity with the button token.
// So we need this DURATION mode.
mode DURATION ;
DURATION_END			: ']' -> popMode ;
COLON					: ':' ;
HASHTAG					: '#' ;
// As well, instead of handle the differences between float and int in lexer, we handle it in the sematic analysis.
NUMBER					: ([1-9] [0-9]* | '0') ('.' [0-9]+)? ;
