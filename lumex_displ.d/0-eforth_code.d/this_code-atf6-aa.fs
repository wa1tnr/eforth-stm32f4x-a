\ Fri Jul  3 14:05:26 UTC 2020

\ atf6=() hex mode
\ 00000000  61 74 66 36 3D 28 29 0A                           |atf6=().|

\ : koylexx $" fine by me " 10 1 - FOR 1 + dup C@ outc sdely NEXT ;

: qtesta $" fine_by_me_too " E 1 - FOR 1 + dup C@ emit NEXT ;
: qtestp $" thats_fine_for_you " E 1 - FOR 1 + dup C@ emit NEXT ;
: qtestq $" thats_fine_for_them " dup C@ 1 - FOR 1 + dup C@ emit NEXT ;
: qtestr $" thats_fine_for_them " dup dup C@ 1 - FOR 1 + dup C@ emit NEXT ;

: qtests $" thats_fine_for_them " dup dup
  C@ 1 - FOR 1 + dup C@ emit NEXT drop drop ;

: qtestt $" thats_fine_for_them " dup dup C@ 1 - FOR
  1 + dup C@ emit NEXT 1B emit drop drop ;

: qtestu $" thats_fine_for_them " dup dup C@ 1 - FOR
  1 + dup C@ emit NEXT 2B emit 2B emit 2B emit drop drop ;

: qtestv $" thats_fine_for_them " dup dup C@ 2 - FOR
  1 + dup C@ emit NEXT 2B emit 2B emit 2B emit drop drop ;

: qtestw $" thats_fine_for_them"  dup dup C@ 1 - FOR
  1 + dup C@ emit NEXT 2B emit 2B emit 2B emit drop drop ;

\ qtestw shows that the quote delimiter ending the string,
\ can be flush against the string.  If it is not, the
\ padding counts towards the composition of the string.

\ the two dup's preserve the string's address (which is
\ where count of the strings elements is) and the second
\ dup of this pair gets incremented, such that it points
\ near (or at) the end of the string, when the for/next
\ loop finishes iterating.

\ thus dup dup .. statements .. drop drop  is a good way
\ to preserve all available intel, but hide it from the
\ stack effect (by dropping the two addresses, just before
\ exiting this word).

\ does not look like the for/next loop's internal counter
\ is referenced in any way; the '1 +' idiom increments
\ the end-user supplied address (the second 'dup') inside
\ this for/next loop.  The 'dup' immediately following
\ gives a fresh copy of the incremented address (user
\ supplied loop memory-index address) and the C@ destroys
\ that copy (returning just the value of the byte stored
\ at that address).

\ You could prove this by not incrementing inside the
\ for/next loop: it should terminate at the correct
\ length, but would just echo the first char over and
\ over, to the same length as the string:


: qtestxdummy $" thats_fine_for_them"  dup dup
  C@ 1 - FOR
  1 + dup C@ emit NEXT 2B emit 2B emit 2B emit drop drop ;

: qtestxproof $" thats_fine_for_them"  dup dup
  C@ 1 -
  swap 1 + swap
         FOR
      dup C@ emit NEXT 2B emit 2B emit 2B emit drop drop ;


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
: hayes nop ;

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

