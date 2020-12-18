\ Sat Jun 20 08:10:45 UTC 2020

\ remove some PORTC stuff in prep for USART6.

( HEX )
( COLD )

\ lower case vocabulary 24 May 09:07z
: emit EMIT ; : nop NOP ; : drop DROP ;
: dup DUP ; : swap SWAP ; : over OVER ;
: or OR ; : rot ROT ; : 2drop 2DROP ;
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

: RCC 40023800 ; ( -- addr )
  ( page 65 )

: RCC_AHB1ENR RCC 30 + ; ( -- addr )

: GPIODEN 1 3 << ; ( -- n )
( 6.3.10 p.180 Rev 18 datasheet)

: RCC! ( -- ) ( verif SED )
  RCC_AHB1ENR @
  GPIODEN or
  RCC_AHB1ENR ! ;

\ : GPIOC ( -- addr ) 40020800 ;
: GPIOD 40020C00 ; ( -- addr )
( 2.3 p.65 )

: GPIOD_MODER GPIOD 0 + ; ( -- addr )
( explicit alias, )
( offset 0x00 8.4.1 p.281 )

\ : MODER1 1 2 << ; ( -- n ) ( 4 )
( 8.4.1 p.281 ) ( GPIOC_1 )
\ : MODER2 1 4 << ; ( -- n ) ( 16 aka 0x10 )
\ : MODER3 1 6 << ; ( -- n ) ( 64 aka 0x40 )

: GPIOD_MODER! ( want SED )
  GPIOD_MODER @ swap
  or GPIOD_MODER ! ;

: OUTPUT ( n -- )
  C max F min
  2 * 1 swap <<
  GPIOD_MODER! ;

( nothing asks for this word, anymore: )
: GPIOD_ODR ( -- addr )
  GPIOD 14 + ;

( explicit alias, )
( offset 0x14 8.4.6 p.283 )

: GPIOD_BSRR ( -- addr )
  GPIOD 18 + ;

( alias for )
( bit write access - see note 8.4.6 )

( BSRR scheme: 0xRRRRSSSS ) 
( word half-word byte )
( byte 8 bits  half word 16 bits word 32 bits )


: BSX 2^ ; ( n -- n )
: BRX 10 + 2^ ; ( n -- n )

( generic - foo_BSSR! setb or clr the port pin )
: GPIOD_BSRR!  ( n -- )
  GPIOD_BSRR ! ;

\ green orange red blue
\ PORT D pins 12-15:
: green C ; ( -- n )
: orange D ; ( -- n )
: red E ; ( -- n )
: blue F ; ( -- n )

: led blue ; \ alias
: led!  GPIOD_BSRR! ; ( n -- )
: on BSX led! ; ( n -- )
: off BRX led! ; ( n -- )

: setupled ( -- )
  RCC! led OUTPUT
  led off ;

: DELAY ( n -- )
  DEPTH 1 - 0<
  IF EXIT THEN
  FOR 3 FOR 11 FOR 100
      FOR 1 DROP NEXT
  NEXT NEXT NEXT ;

: BDELAY 3 DELAY ; ( -- )
: BDKDEL 8 DELAY ; ( -- )
: LDELAY 188 DELAY ; ( -- )
: FINISHMSG ."  done." ; ( -- )

( 100 blinks per minute )

: BLINKS ( n -- )
  DEPTH 1 - 0<
  IF EXIT THEN
  1 - ( normalize )
  FOR
    green on orange on red on blue on
    BDELAY
    green off orange off red off blue off
    BDKDEL
  NEXT ;

: blinks BLINKS ; \ alias

: LINIT ( -- n )
  FFFFFF9D setupled
  3 BLINKS ;

\ untested: key, type
\ : base-ten 7 3 + base ! ;
\ : base-sixteen 7 7 2 + + base ! ;

: nullemit 0 emit ;
: readkeyc
  BEGIN
    ?KEY ( c T | F )
    IF EXIT THEN
  AGAIN ;
: pollc
  nullemit
  BEGIN
    readkeyc
    \ look for ascii 27 ESC
    dup 1B = IF drop EXIT THEN
    dup  D = IF A emit THEN
    emit
  AGAIN
  ;

: vers space
  ." 0.0.0.1.v- "
  ." Sat Jun 20 08:10:45 UTC 2020 "
; \ green orange red blue
 ( trial run: ) ( LINIT ) ( green OUTPUT )
 ( green on ) ( green off )
 ( 5 blinks ) ( blue on LDELAY blue off )

.( 0 ERASE_SECTOR ) ( TURNKEY )

 ( - - - - - )
