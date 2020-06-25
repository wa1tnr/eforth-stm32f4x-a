\ ." Wed Jun 24 18:44:17 UTC 2020 "

\ load after everything else 'all_code-*'.

\ scratch pad forth source - no organization
\ some word defs repeated - ignore

\ progress: at81 positioning helps paint
\ multi-color message blocks

\ To specify one individual ASCII code, you
\ can do it this way:

\ [ CHAR J ] LITERAL

\ works fine:
: signal-james

  [ CHAR s ] LITERAL
  [ CHAR e ] LITERAL
  [ CHAR m ] LITERAL
  [ CHAR a ] LITERAL
  [ CHAR J ] LITERAL

  5 1 - FOR outc NEXT ;

\ output is:
\ sendmsg 8 14 7 8 12 7 7 ok

\ TODO: verify that output (stack)
\ then remove generation of stack effect.

\ atef=(4) is nice mid green
: msg_koylijj $" at81=(0,0,fine brew) "   DUP C@ 1 - DUP . ;
\ : line_end 0A outc ;
: msg_koyligg $" at81=(0,10,C0FFEE) "          DUP C@ 1 - DUP . ;
: msg_koyliff $" fine by me. "   DUP C@ 1 - DUP . ;
: msg_c0ffee-green $" atef=(4) " DUP C@ 1 - DUP drop ;
\ eight?              12345678  YES.
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
: sendmsgb clearit 25 delay msg_c0ffee-a
  1 - FOR 1 + dup C@ outc 2B EMIT NEXT drop ;


\          123456789abcdef
: msg_koyliee $" fine by me C0ff " DUP C@ 1 - DUP . ;
: sendmsgc clearit 25 delay msg_koyliee
  1 - FOR 1 + dup C@ outc 2B EMIT NEXT drop ;


\ seems to be counting by itself, now:
: msg_koylidd $" fine by me. " DUP C@ 1 - DUP . ;
: sendmsgd clearit 25 delay msg_koylidd
  1 - FOR 1 + dup C@ outc 2B EMIT NEXT drop ;


: msg_koylicc $" fine by me. " DUP B ;
: sendmsge clearit 25 delay msg_koylicc
  1 - FOR 1 + dup C@ outc 2B EMIT NEXT drop ;

\ TODO: $" may be counted somehow.

: msg_koyliaa $" fine by me. " B ;
: sendmsgf clearit 25 delay msg_koyliaa
  1 - FOR 1 + dup C@ outc 2B EMIT NEXT drop ;

\ log follows good forth words, below
\ cold
\ stm32eForth v7.20
\ : msg_koyliaa $" fine by me. " B ; ok
\ : sendmsg clearit 25 delay msg_koyliaa
\   1 - FOR 1 + dup C@ outc 2B EMIT NEXT drop ; ok
\ .s   ok
\ decimal -99 hex ok
\ .s  FFFFFF9D  ok
\ sendmsg+++++++++++ ok
\ .s  FFFFFF9D  ok


: sdely 1 drop 1 drop 1 drop ;

: koylexx $" fine by me " 10 1 - FOR 1 + dup C@ outc sdely NEXT ;

: koylepp $" fine by me. " 11 1 - FOR 1 + dup C@ outc NEXT ;

\ really close, very good indeed:
: koylemm $" fine by me " 9 1 - FOR 1 + dup C@ EMIT NEXT ;

\ okay not great:
: koylekk $" fine by me " 9 1 - FOR 1 + dup EMIT NEXT ;

 ( - - - - - )
