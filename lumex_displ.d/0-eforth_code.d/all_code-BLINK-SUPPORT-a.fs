\ ." Mon Jun 22 16:35:18 UTC 2020 "

( HEX ) ( COLD )

: RCC 40023800 ; ( -- addr )
: RCC_AHB1ENR RCC 30 + ; ( -- addr )
: RCC_APB2ENR RCC 44 + ; ( -- addr )
: GPIODEN 1 3 << ; ( -- n )
: USART1EN 1 4 << ; ( -- n )

: RCC! ( -- ) ( verif SED )
  RCC_AHB1ENR @
  GPIODEN or
  RCC_AHB1ENR !
;

: GPIOD 40020C00 ; ( -- addr )
: GPIOD_MODER GPIOD 0 + ; ( -- addr )
: GPIOD_MODER! ( n -- )
  GPIOD_MODER @ swap
  or GPIOD_MODER ! ;

: OUTPUT ( n -- )
  6 max F min \ was C not 6
  2 * 1 swap <<
  GPIOD_MODER! ;

: GPIOD_BSRR ( -- addr )
  GPIOD 18 + ;

: BSX 2^ ; ( n -- n )
: BRX 10 + 2^ ; ( n -- n )

: GPIOD_BSRR!  ( n -- )
  GPIOD_BSRR ! ;

: green C ; ( -- n )
: orange D ; ( -- n )
: red E ; ( -- n )
: blue F ; ( -- n )

: led 6 ; \ PD6 convenience selection
: led!  GPIOD_BSRR! ; ( n -- )
: on BSX led! ; ( n -- )
: off BRX led! ; ( n -- )

: setupled ( -- )
  RCC! led OUTPUT
  led off ;

: blinks ( n -- )
  DEPTH 1 - 0<
  IF EXIT THEN
  1 - ( normalize )
  FOR
    led on green on orange on red on blue on
    bdelay
    led off green off orange off red off blue off
    bdkdel
  NEXT ;

: linit ( -- n )
  FFFFFF9D setupled
  3 blinks

  7 FOR bdkdel NEXT
  7 FOR bdkdel NEXT
  led off
  3 blinks
;

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

: plus_sign 2B ;
: hash_symb 23 ;
: carr_ret   D ;

: go
  led OUTPUT
  2 blinks 43 delay
  7 blinks 33 delay
  30 spaces 2B emit 2B emit 2B emit CR
;

: vers space ." 0.0.0.2.kc- "
  ." Mon Jun 22 16:35:18 UTC 2020 "
; \ green orange red blue
 ( trial run: ) ( LINIT ) ( green OUTPUT )
 ( green on ) ( green off )
 ( 5 blinks ) ( led on ldelay led off )

.( 0 ERASE_SECTOR ) ( TURNKEY )

 ( - - - - - )
