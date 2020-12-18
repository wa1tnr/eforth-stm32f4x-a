\ ." Mon Jun 22 16:18:56 UTC 2020 "

( HEX ) ( COLD )

\ lower case vocabulary 24 May 09:07z
: emit EMIT ; : nop NOP ; : drop DROP ;
: dup DUP ; : swap SWAP ; : over OVER ;
: or OR ; : and AND ; : rot ROT ;
: 2drop 2DROP ;
: 2dup 2DUP ; : not NOT ; : negate NEGATE ;
: abs ABS ; : max MAX ; : min MIN ; : base BASE ;
: depth DEPTH ; : here HERE ; : hex HEX ;
: decimal DECIMAL ; : key KEY ; : space SPACE ;
: spaces SPACES ; : type TYPE ; : cr CR ;
: char CHAR ; : preset PRESET ; : dump DUMP ;
: .s .S ; : see SEE ; : words WORDS ; : cold COLD ;

: <1?  ( n -- BOOL )
  dup 1 - 0< IF
    drop -1 EXIT
  THEN
  drop 0 ;

: << ( n shifts -- )
  LSHIFT ;

: 2^ ( n -- )
  dup <1?  0< IF
    drop 1 EXIT
  THEN
  1 swap << ;

: delay ( n -- )
  DEPTH 1 - 0<
  IF EXIT THEN
  FOR 3 FOR 11 FOR 100
      FOR 1 DROP NEXT
  NEXT NEXT NEXT ;

: bdelay 3 delay ; ( -- )
: bdkdel 8 delay ; ( -- )
: ldelay 122 delay ; ( -- )
: finishmsg ."  done." ; ( -- )

: vers space ." 0.0.0.2.kb- "
  ." Mon Jun 22 16:18:56 UTC 2020 "
;

.( 0 ERASE_SECTOR ) ( TURNKEY )

 ( - - - - - )
