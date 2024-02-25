parser grammar NoteBlockParser;

options {tokenVocab=NoteBlockLexer;}

// NOTE:
// When analyzing a rule and encountering a naked token,
// such as `tap  :  BUTTON tap_mark`
//                  ^^^^^^
// We need to assign a name to that token like this:
// `tap  :  pos=BUTTON tap_mark`
// Additionally, we do not give names to rules like `tap_mark` in the example.


note_block					: (note_group? COMMA)* EOF ;

// We first separate each group, for example: `` 1h[8:1]`2 ``
// Or simple each tap group like `15`.
note_group					: each_tap | each_group (FAKE_EACH_SEPARATOR each_group)* ;
// Then we separate each group, for example: `1/5`.
each_group					: note (EACH_SEPARATOR note)* ;
note						: tap | hold | touch | touch_hold | slide ;

// Simple each tap represented like `15`.
each_tap					: pos1=BUTTON pos2=BUTTON ;

// tap note
tap							: pos=BUTTON tap_mark ;
// Duplicate tap marks are accepted, for instance, `1bbxbx`, and a warning is stored during semantic analysis.
// (The same applies below)
tap_mark					: (BREAK_MARK | EX_MARK)* ;

// hold note
hold						: pos=BUTTON hold_mark duration? ;
// Hold marks can be positioned anywhere and can be duplicated as well (indicated by `+` after `HOLD_MARK`).
// For example: `1xxhbhbx[8:1]`.
// This may trigger a warning.
hold_mark					: (BREAK_MARK | EX_MARK | HOLD_MARK)* HOLD_MARK+ (BREAK_MARK | EX_MARK | HOLD_MARK)* ;

// touch note
touch						: pos=AREA touch_mark ;
// This is for the sake of consistency considerations :(
touch_mark					: FIREWORK_MARK* ;

// touch hold note
// We assume that touch holds can appear in any touch area,
// and an error or warning is stored to handle this during semantic analysis.
touch_hold					: pos=AREA touch_hold_mark duration? ;
touch_hold_mark				: (FIREWORK_MARK | HOLD_MARK)* HOLD_MARK+ (FIREWORK_MARK | HOLD_MARK)* ;

// slide note
// Initially, we separate the same head slide like `1-5[8:1]*-6[8:1]`.
slide						: startPos=BUTTON tap_mark slide_body (SLIDE_SAME_HEAD_MARK slide_body)* ;
// Then we handle slide chains like `1-5-2[4:1]` or `1-5[8:1]-2[8:1]`.
// Additionally, if we specify the duration of a paragraph, then we must specify the duration of all paragraphs,
// but this is not handled in this stage.
slide_body					: slide_part* slide_tail bsMark1=BREAK_MARK? duration bsMark2=BREAK_MARK? ;
slide_part                  : slide_tail duration? ;
// We assume that all slides contain 2 endpoints and handle the `V` slide type in semantic analysis.
slide_tail					: type=SLIDE_TYPE turnBtn=BUTTON? stopBtn=BUTTON ;


// duration
duration					: DURATION_START (frac_duration | bpm_frac_duration | time_duration | bpm_time_duration | delay_frac_duration | delay_time_duration | delay_bpm_frac_duration) DURATION_END ;
// Holds and slides cannot accept certain types of duration formats,
// but we will handle this in semantic analysis,
// as well as the float and int problem.

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
