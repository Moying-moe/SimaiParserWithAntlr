parser grammar StructureParser;

options {tokenVocab=StructureLexer;}

chart                       : element* EOF ;
element                     : bpm | resolution | h_speed | maml_open | maml_close | note_block | comment ;

bpm                         : BPM_START value=BPM_NUMBER BPM_END ;
resolution                  : RESOLUTION_START value=RESOLUTION_NUMBER RESOLUTION_END ;

note_block                  : (ANY | NEWLINE)+ ;

comment                     : COMMENT_SYMBOL ANY* NEWLINE? ;

h_speed                     : H_SPEED_START rate=H_SPEED_NUMBER H_SPEED_END ;

maml_open                   : MAML_OPEN_START name=MAML_OPEN_WORD (MAML_WS+ props+=maml_prop)* MAML_WS* MAML_OPEN_END ;
maml_prop                   : name=MAML_OPEN_WORD (MAML_EQUAL MAML_QUOTE STRING_ANY* STRING_END)? ;

maml_close                  : MAML_CLOSE_START name=MAML_CLOSE_WORD MAML_CLOSE_END ;