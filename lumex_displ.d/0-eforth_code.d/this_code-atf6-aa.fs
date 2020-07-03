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
  29 outc ;
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

: msgbb
  [ char ! ] LITERAL
  [ char y ] LITERAL
  [ char l ] LITERAL
  [ char l ] LITERAL
  [ char o ] LITERAL
  [ char P ] LITERAL
  BL
  [ char p ] LITERAL
  [ char u ] LITERAL
  BL
  [ char e ] LITERAL
  [ char k ] LITERAL
  [ char a ] LITERAL
  [ char W ] LITERAL
;

: textcc-works \ doesn't crash
  msgbb
  fabb
  81 outc 0 outc 0 outc
  outc outc outc outc
  outc outc outc outc
  outc outc outc outc
  0A outc
  foxx
;

: textcc
  msgbb
  fabb
  81 outc 0 outc 0 outc
  outc outc outc outc
  outc outc outc outc
  outc outc outc outc
  0 outc \ null terminated string
  \ 0A outc
  dy dy
  D1 outc
  dy dy
  foxx
;

: drwsqr ( -- )
  fabb dy 93 outc 48 outc 1 outc 4 outc 2 outc dy foxx dy ;

: drwstr ( -- )
  fabb dy 81 outc 0 outc 1 outc 50 outc 52 outc dy foxx dy ;

: drwp ( -- )
  fabb dy
  80 outc 0 outc 0 outc 50 outc dy
  80 outc 0 outc 1 outc 52 outc dy
  80 outc 0 outc 2 outc 54 outc dy
  80 outc 0 outc 3 outc 56 outc dy
  80 outc 0 outc 4 outc 58 outc
  dy foxx ;

\ LESSON LEARNED:

\ Just write the bytes to the display normally, and
\ pad with spaces (BL) as required, to do string output.
\ To do placed output (specific locations) use the other
\ method (fabb/foxx delimited stuff).
\ so far, fabb/foxx only works well with at80=(row,col,char)
\ as the construct.  'drwp' (just above) is typical usage.
\ It's probably most useful after a color change, for
\ mixed color messages.


\ this sequence was recoverable!
\ fabb ok
\ 80 outc 30 outc 30 outc 50 outc dy dy D1 outc ok
\ D1 outc ok
\ foxx ok
\ signon

