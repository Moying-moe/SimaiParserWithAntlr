parser grammar NoteBlockParser;

options {tokenVocab=NoteBlockLexer;}

// NOTE:
// If we gonna analyze a rule, and there is a naked token in it
// For example: `tap  :  BUTTON tap_mark`
//                       ^^^^^^
// We need to give that token a name like this:
// `tap  :  pos=BUTTON tap_mark`
// And we do not give a name to those rules like `tap_mark` in the example.


note_block					: (note_group? COMMA)* EOF ;

// We seperate fake each group firstly, example: `` 1h[8:1]`2 ``
// Or just a simple each tap group like `15`
note_group					: each_tap | each_group (FAKE_EACH_SEPARATOR each_group)* ;
// Then we seperate each group, example: `1/5`
each_group					: note (EACH_SEPARATOR note)* ;
note						: tap | hold | touch | touch_hold | slide ;

// simple each tap. example: `15`
each_tap					: pos1=BUTTON pos2=BUTTON ;

// tap note
tap							: pos=BUTTON tap_mark ;
// We accept duplicate tap mark like `1bbxbx` and throw a warning in the sematic analysis.
// (The same below)
tap_mark					: (BREAK_MARK | EX_MARK)* ;

// hold note
hold						: pos=BUTTON hold_mark duration? ;
// Hold mark can be any position and it can be duplicate as well. (So there is a `+` after `HOLD_MARK`)
// For example: `1xxhbhbx[8:1]`.
// And it will trigger a warning (if so).
hold_mark					: (BREAK_MARK | EX_MARK | HOLD_MARK)* HOLD_MARK+ (BREAK_MARK | EX_MARK | HOLD_MARK)* ;

// touch note
touch						: pos=AREA touch_mark ;
// This is for some sort of consistency considerations :(
touch_mark					: FIREWORK_MARK* ;

// touch hold note
// We assume that touch hold can appear in any touch area.
// And throw an exception or warning to handle this in the sematic analysis.
touch_hold					: pos=AREA touch_hold_mark duration? ;
touch_hold_mark				: (FIREWORK_MARK | HOLD_MARK)* HOLD_MARK+ (FIREWORK_MARK | HOLD_MARK)* ;

// slide note
// First, we seperate the same head slide like `1-5[8:1]*-6[8:1]`
slide						: startPos=BUTTON tap_mark slide_body (SLIDE_SAME_HEAD_MARK slide_body)* ;
// Then, we handle the slide chain like `1-5-2[4:1]` or `1-5[8:1]-2[8:1]`
// Also, if we specify the duration of a paragraph, then we must specify the duration of all paragraphs.
// But we do not handle it in this stage.
slide_body					: (slide_tail duration?)* slide_tail bsMark1=BREAK_MARK? duration bsMark2=BREAK_MARK? ;
// We assume that all slides contain 2 end. And handle `V` slide type in the sematic analysis.
slide_tail					: SLIDE_TYPE endPos1=BUTTON? endPos2=BUTTON ;


// duration
duration					: DURATION_START (frac_duration | bpm_frac_duration | time_duration | bpm_time_duration | delay_frac_duration | delay_time_duration | delay_bpm_frac_duration) DURATION_END ;
// Hold and slide can not accept some type of these duration formats.
// But we will handle it in the sematic analysis.
// As well the float and int problem.

// example: `8:1`
frac_duration				: den=NUMBER COLON num=NUMBER ;
// example: `120#8:1`
bpm_frac_duration			: bpm=NUMBER HASHTAG den=NUMBER COLON num=NUMBER ;
// example: '#1.5'		hold only
time_duration				: HASHTAG dur=NUMBER ;
// example: '120#1.5`   slide only
bpm_time_duration			: bpm=NUMBER HASHTAG dur=NUMBER ;
// example: `2.5##8:1`	slide only
delay_frac_duration			: delay=NUMBER HASHTAG HASHTAG den=NUMBER COLON num=NUMBER ;
// example: `2.5##1.5`	slide only
delay_time_duration			: delay=NUMBER HASHTAG HASHTAG dur=NUMBER ;
// example: `2.5##120#8:1`	slide only
delay_bpm_frac_duration		: delay=NUMBER HASHTAG HASHTAG bpm=NUMBER HASHTAG den=NUMBER COLON num=NUMBER ;
