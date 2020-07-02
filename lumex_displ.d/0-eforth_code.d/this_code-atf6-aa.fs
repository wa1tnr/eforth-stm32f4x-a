\ atf6=() hex mode
\ 00000000  61 74 66 36 3D 28 29 0A                           |atf6=().|

: atf6_ ( -- )
  61 outc 74 outc 66 outc 36 outc
  \ 3D outc 28 outc 29 outc
  3D outc 28 outc \ 30 outc 29 outc 0A outc
  \ line_end is '0A outc'
;

: dy 8 delay ;

: atf6-0 ( -- ) \ enable hexmode - not working
  atf6_ 30 outc 29 outc 0A outc ;

: atf6-1 ( -- ) \ enable AT commands - seems okay
  atf6_ 31 outc 29 outc 0A outc ;

: atf6-1hx ( -- )
  F6 outc 1 outc ;

: hexmode atf6-0 ;

: testhx77 ( -- )
  \ atf6 dy - retains mode so do this separately
  hexmode dy dy dy
  80 outc
  0 outc
  1 outc
  50 outc \ [ CHAR P ] LITERAL outc dy
  dy dy
  D1 outc ;

: fabb \ enable HEX commands
  61 outc 74 outc 66 outc 36 outc 3D outc 28 outc 30 outc
  29 outc 0A outc ;
: foxx \ re-enable AT commands
\ F6 outc 1 outc 0A outc ;
  F6 outc 1 outc ;

: texthx
  fabb dy
  80 outc 0 outc 0 outc 30 outc dy
  80 outc 0 outc 1 outc 31 outc dy
  80 outc 0 outc 2 outc 32 outc dy
  80 outc 0 outc 3 outc 33 outc dy
  80 outc 0 outc 4 outc 34 outc dy
  80 outc 0 outc 5 outc 35 outc dy
  foxx dy
;

\ this sequence was recoverable!
\ fabb ok
\ 80 outc 30 outc 30 outc 50 outc dy dy D1 outc ok
\ D1 outc ok
\ foxx ok
\ signon

