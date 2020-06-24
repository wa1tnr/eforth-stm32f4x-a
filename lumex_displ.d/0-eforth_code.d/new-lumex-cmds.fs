\ ." Wed Jun 24 18:44:17 UTC 2020 "

\ scratch pad forth source - no organization
\ some word defs repeated - ignore

\ progress: at81 positioning helps paint
\ multi-color message blocks

\ [ CHAR J ] LITERAL

\ works fine:
: signal-james

  [ CHAR s ] LITERAL
  [ CHAR e ] LITERAL
  [ CHAR m ] LITERAL
  [ CHAR a ] LITERAL
  [ CHAR J ] LITERAL

  5 1 - FOR outc NEXT ;

\ atef=(4) is nice mid green
: msg_koylijj $" at81=(0,0,fine brew) "   DUP C@ 1 - DUP . ;
: line_end 0A outc ;
: msg_koyligg $" at81=(0,10,C0FFEE) "          DUP C@ 1 - DUP . ;
: msg_koyliff $" fine by me. "   DUP C@ 1 - DUP . ;
: msg_c0ffee-green $" atef=(4) " DUP C@ 1 - DUP . ;
: msg_c0ffee-blue  $" atef=(1) " DUP C@ 1 - DUP . ;
: msg_dlocator $" at80=(0,B,x) " DUP C@ 1 - DUP . ;
: intermsg 1 - FOR 1 + dup C@ outc NEXT drop ;
: msg_refresh  $" atd1=() "
  21 delay DUP C@ 1 - DUP . intermsg ;
: idelay 21 delay ;
: fifmsg msg_c0ffee-blue intermsg idelay
  msg_koyligg intermsg idelay msg_refresh ;
: sendmsg clearit idelay msg_c0ffee-green intermsg
  idelay msg_koylijj intermsg idelay msg_refresh idelay
  27 delay fifmsg 22 delay msg_refresh ;

\ - - -  current work is above this line - - -


\ - - -  all below this line somewhat outdated.  - - -

\ 42 kind of stark white a bit of a tinge
: msg_c0ffee-a $" atef=(42) " DUP C@ 1 - DUP . ;
: sendmsg clearit 25 delay msg_c0ffee-a
  1 - FOR 1 + dup C@ outc 2B EMIT NEXT drop ;


\ sendmsg F+++++++++++++++ ok
\          123456789abcdef
: msg_koyliee $" fine by me C0ff " DUP C@ 1 - DUP . ;
: sendmsg clearit 25 delay msg_koyliee
  1 - FOR 1 + dup C@ outc 2B EMIT NEXT drop ;


\ seems to be counting by itself, now:
: msg_koylidd $" fine by me. " DUP C@ 1 - DUP . ;
: sendmsg clearit 25 delay msg_koylidd
  1 - FOR 1 + dup C@ outc 2B EMIT NEXT drop ;


: msg_koylicc $" fine by me. " DUP B ;
: sendmsg clearit 25 delay msg_koylicc
  1 - FOR 1 + dup C@ outc 2B EMIT NEXT drop ;

\ TODO: $" may be counted somehow.

: msg_koyliaa $" fine by me. " B ;
: sendmsg clearit 25 delay msg_koyliaa
  1 - FOR 1 + dup C@ outc 2B EMIT NEXT drop ;

\ log follows good forth words, below
cold
stm32eForth v7.20
: msg_koyliaa $" fine by me. " B ; ok
: sendmsg clearit 25 delay msg_koyliaa
  1 - FOR 1 + dup C@ outc 2B EMIT NEXT drop ; ok
.s   ok
decimal -99 hex ok
.s  FFFFFF9D  ok
sendmsg+++++++++++ ok
.s  FFFFFF9D  ok






: sdely 1 drop 1 drop 1 drop ;

\            123456789AB

\ koyliaa+++++++++++ ok
\        123456789AB
\ .s  FFFFFF9D  ok




: koylexx $" fine by me " 10 1 - FOR 1 + dup C@ outc sdely NEXT ;

: koylepp $" fine by me. " 11 1 - FOR 1 + dup C@ outc NEXT ;

\ really close, very good indeed:
: koylemm $" fine by me " 9 1 - FOR 1 + dup C@ EMIT NEXT ;

\ okay not great:
: koylekk $" fine by me " 9 1 - FOR 1 + dup EMIT NEXT ;

: koylejj $" fine by me " 9 1 - FOR 1 + dup . NEXT ;

: koylehh $" fine by me " 9 1 - FOR over 1 + dup . NEXT ;


: koyleff $" fine by me " 1 - 9 1 - FOR 1 + dup @ NEXT ;

: koyledd $" fine by me " 1 - 9 1 - FOR 1 + dup C@ NEXT ;
: koyleaa ." here to go " PAD ;

\ eForth p.79 EXTRACT
\ 1421 10 EXTRACT EMIT 10 EXTRACT EMIT 10 EXTRACT EMIT 10 EXTRACT EMIT1241 ok

: sigg

  [ CHAR s ] LITERAL
  [ CHAR e ] LITERAL
  [ CHAR m ] LITERAL
  [ CHAR a ] LITERAL
  [ CHAR J ] LITERAL

  5 1 - FOR outc NEXT
;



\ : c0ffee-sign
: line_end 0A outc ;
: s_atd1=()
 \ atd1=()
 29 28 3D 31   64 74 61
 7 1 - FOR outc NEXT
 line_end
;

\ reference:
\ lumex-64x32_RGB_LED_command_list_V17_dot_E_03152019.pdf

\  1 dark blue
\ 32 dark red
\  4 dark green
\ red: 32 64 96
\ blue: 1  2  3
\ green: 4 8 12
\ cyan:  7 11 15 - and 23
\ magenta: 65 66 67, 97 98 99

: atef=(32) \ dark red
\ )  2  3  (    =  f  e  t    a
 29 32 33 28   3D 66 65 74   61
 outc outc outc outc
 outc outc outc outc       outc
 line_end
 s_atd1=()
;

: atef=(1) \ default color, here '1' is a color range 0-255
\ 1 is dark blue - not very dark, though.
\ )  1  (  =    f  e  t  a
 29 31 28 3D   66 65 74 61
 outc outc outc outc outc outc outc outc
 line_end
 \ 8 1 - FOR outc NEXT
 s_atd1=()
;

: at80=(0,0,4)
 29 34 2C 30   2C 30 28 3D   30 38 74 61
 outc outc outc outc outc outc outc outc
 outc outc outc outc
 line_end
 \ C 1 - FOR outc NEXT
 s_atd1=()
;

: at80=(0,1,0)
 29 30 2C 31   2C 30 28 3D   30 38 74 61
 C 1 - FOR outc NEXT
 line_end
 s_atd1=()
;

: at80=(0,2,C)
 29 43 2C 32   2C 30 28 3D   30 38 74 61
 C 1 - FOR outc NEXT
 line_end
 s_atd1=()
;

: sent 14 delay ;

: said
  atef=(1) sent
  at80=(0,0,4) sent
  at80=(0,1,0) sent
  at80=(0,2,C) sent
\ s_atd1=()
;

 ( - - - - - )
