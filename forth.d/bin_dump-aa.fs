: HELP_BIN_DUMP SPACE ." no help yet." CR ;

HEX

( want < 16kb binary dump )

: SIZED ( -- n )
  3FFF 1 + ; ( 16384 )
  ( comment okay after the sem )

: BDUMP ( -- )
  SIZED .
;

: NOTES_B.TXT ( -- )
  CR ." notes_b.txt follows." CR
  ."    say something"   CR
  ."    say something else"   CR
  CR
  ."    say something again"   CR ;

: VERS_BD SPACE ." 0.0.0.1.a-" ;

: FREN FOR 1 DROP NEXT 2B EMIT 2B EMIT 2B EMIT 20 EMIT ;

 ( - - - - - )

0 [IF]

  comment

[THEN]
