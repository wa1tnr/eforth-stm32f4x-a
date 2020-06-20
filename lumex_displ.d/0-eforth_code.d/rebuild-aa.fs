\ Sat Jun 20 06:59:07 UTC 2020
\ add colored LEDs 0x0C D E and F
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

: RCC 40023800 ; ( -- addr )

  ( page 65 )

: RCC_AHB1ENR RCC 30 + ; ( -- addr )

: << ( n shifts -- )

  LSHIFT ;

  ( RESCIND comment: 1 - FOR 2* NEXT )

: GPIODEN 1 3 << ; ( -- n )

: GPIOCEN 1 2 << ; ( -- n )

( 6.3.10 p.180 Rev 18 datasheet)

: RCC! ( -- ) ( verif SED )

  RCC_AHB1ENR @
  GPIOCEN or
  RCC_AHB1ENR !

  RCC_AHB1ENR @
  GPIODEN or
  RCC_AHB1ENR ! ;

: GPIOD 40020C00 ; ( -- addr )
( 2.3 p.65 )

: GPIOC ( -- addr ) 40020800 ;
( 2.3 p.65 )

: GPIOD_MODER GPIOD 0 + ; ( -- addr )

: GPIOC_MODER GPIOC 0 + ; (  -- addr )

( explicit alias, )

( offset 0x00 8.4.1 p.281 )

: MODER1 1 2 << ;

( -- n ) ( 4 )

( 8.4.1 p.281 ) ( GPIOC_1 )

( here's how to setup any port pin on PORTC )

(  as OUTPUT: )

( 1 4 << ok ) ( 1 is just the universal binary bit )

( 4 here is the selected port pin - PORTC MODER2 - low bit )

( as in 8.4.1 )

( GPIOC_MODER @ SWAP OR GPIOC_MODER ! ok )

( that was PORTC_2  aka  PORTC MODER2 )

( PORTC_1 is the D13 LED )

( here's PORTC_3 aka PORTC MODER3 )

( 1 6 << ok )

( GPIOC_MODER @ SWAP OR GPIOC_MODER ! ok )

: MODER2 1 4 << ; ( -- n ) ( 16 aka 0x10 )

: MODER3 1 6 << ; ( -- n ) ( 64 aka 0x40 )

: MODER14 1 1C << ; ( want SED )

( : GPIOC_MODER1! )

( GPIOC_MODER @ MODER1 OR GPIOC_MODER ! ; )

: GPIOD_MODER! ( want SED )
  GPIOD_MODER @ swap
  or GPIOD_MODER ! ;

: GPIOC_MODER! ( want SED )
  GPIOC_MODER @ swap
  or GPIOC_MODER ! ;

( n -- ) ( wants MODER2 or MODER3 &c )

: OUTPUT ( n -- )

  1 max 3 min

  ( kludge don't want pin0 or > pin3 )

  2 * 1

  swap <<

  GPIOC_MODER! ;

: DOUTPUT ( n -- )
  C max F min

  2 * 1

  swap <<

  GPIOD_MODER! ;

( trial run: )
( LINIT ok )
( 1 on ok )
( 1 off ok )
( 2 OUTPUT ok )
( 2 on ok )
( 2 off ok )
( 3 OUTPUT ok )
( 3 on ok )
( 3 off ok )

( evidently GPIOC_ODR effort is incomplete )
( nothing asks for this word, anymore: )

: GPIOD_ODR ( -- addr )
  GPIOD 14 + ;

: GPIOC_ODR ( -- addr )
  GPIOC 14 + ;

( explicit alias, )
( offset 0x14 8.4.6 p.283 )

: GPIOD_BSRR ( -- addr )
  GPIOD 18 + ;

: GPIOC_BSRR ( -- addr )
  GPIOC 18 + ;

( alias for )
( bit write access - see note 8.4.6 )

( BSRR scheme: 0xRRRRSSSS ) 
( word half-word byte )
( byte 8 bits  half word 16 bits word 32 bits )

: <1?  ( n -- BOOL )
  dup 1 - 0< IF
    drop -1 EXIT
  THEN
  drop 0 ;

: 2^ ( n -- )
  dup <1?  0< IF
    drop 1 EXIT
  THEN
  1 swap << ;

: BSX 2^ ; ( n -- n )
( BS1 for PORTC_1 )

: BRX 10 + 2^ ; ( n -- n )
( BR1 for PORTC_1 )

: GPIOD_BSRR!  ( n -- )
  GPIOD_BSRR ! ;

: GPIOC_BSRR!  ( n -- )
( generic - may setb or clr the port pin )
  GPIOC_BSRR ! ;

: led 1 ; ( -- n )

( PORTC_1 )

\ PORT D pins 12-15:
: green C ; ( -- n )
: orange D ; ( -- n )
: red E ; ( -- n )
: blue F ; ( -- n )

: led2 2 ; ( -- n )
( PORTC_2 )

: led3 3 ; ( -- n )
( PORTC_3 )

: led!  GPIOC_BSRR! ; ( n -- )

: dled!  GPIOD_BSRR! ; ( n -- )

: on BSX led! ; ( n -- )

: don BSX dled! ; ( n -- )

: off BRX led! ; ( n -- )

: doff BRX dled! ; ( n -- )

: setupled ( -- )
  RCC!
  led OUTPUT
  1C DOUTPUT
  led off
  1C doff
  ;

( led GPIOC 14 + 2 )

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

( blinks isn't facile enough to )
( blink a specified port on command )
( it always blinks PORTC_1 aka D13 )

: BLINKS ( n -- )
  DEPTH 1 - 0<
  IF EXIT THEN

  1 - ( normalize )

  FOR led on
  C don D don E don F don
  BDELAY
  led off
  C doff D doff E doff F doff
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
  ." 0.0.0.1.t- "
  ." Sat Jun 20 07:06:35 UTC 2020 "
;

.( 0 ERASE_SECTOR )

 ( - - - - - )
