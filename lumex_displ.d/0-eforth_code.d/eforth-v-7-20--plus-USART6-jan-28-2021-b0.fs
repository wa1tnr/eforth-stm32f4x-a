\ eforth-v-7-20--plus-USART6-jan-28-2021-a.fs
\ Thu Jan 28 15:49:22 UTC 2021

\ INCREMENTALLY TURNKEY 'd - vers is old vers

\ incremental has 'd.' word

\ TESTED   - cold works correctly.\
\ UPLOADED - resident on target.

\ IDEA: have it blink when upload is done!

\ current 'picocom' command line
\     to interact with this system is:

\     0-current-PICOCOM-snippet.txt

\ no timestamp or verson change

\    - just these comments.

\ ' signon 1 + 'BOOT ! 0 ERASE_SECTOR TURNKEY

\ That's basically how you automatically start an
\ arbitrary forth word.

( COLD HEX )

VARIABLE speed
55 speed C! \ speed C@ 

\ lower case vocabulary
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
: finishmsg ." done." ; ( -- )

: RCC 40023800 ; ( -- addr )
: RCC_AHB1ENR RCC 30 + ; ( -- addr )
: RCC_APB2ENR RCC 44 + ; ( -- addr )
: GPIODEN 1 3 << ; ( -- n )
: USART1EN 1 4 << ; ( -- n )

: RCC! ( -- )
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

: green C ; : orange D ;
: red E ; : blue F ;

: led 6 ; \ PD6 convenience selection
: led!  GPIOD_BSRR! ; ( n -- )
: on BSX led! ; ( n -- )
: off BRX led! ; ( n -- )

: setupled ( -- )
  RCC! led OUTPUT led off ;

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
  led off 3 blinks ;

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
  AGAIN ;

: plus_sign 2B ; : hash_symb 23 ;
: carr_ret   D ;

: GPIOCEN ( -- n ) 1 2 << ;
( 6.3.10 p.180 Rev 18 datasheet)

\ PC6/TX PC7/RX
: USART6EN ( -- n ) 1 5 << ;

\ PB_6/TX,  USART1

: SIO_RCC! ( -- ) ( verif SED )
  RCC_AHB1ENR @
  GPIOCEN or
  RCC_AHB1ENR !

  RCC_APB2ENR @
  USART6EN or
  RCC_APB2ENR ! ;

: USART1 40011000 ; ( -- addr )
: USART6 40011400 ; ( -- addr )

: USART6_CR1 USART6 C + ; ( -- addr ) \ #12
( 30.6.4 p.1010 )

: USART1_CR1 USART1 C + ; ( -- addr ) \ NO LOOKUP - lazy

: USART6_CR1_UE 1 D << ; \ 0x2000 ( -- n )
: USART1_CR1_UE 1 D << ; \ 0x2000 ( -- n )
\ bit 13 ref page 1010

: USART6_CR1_TE 1 3 << ; \ 0x8 ( -- n )
: USART1_CR1_TE 1 3 << ; \ 0x8 ( -- n )

: USART6_CR1_RE 1 2 << ; \ 0x4 ( -- n )
: USART1_CR1_RE 1 2 << ; \ 0x4 ( -- n )

: USART1_CR1_SETUPS
  USART1_CR1_UE   \ -- 0x2000
  USART1_CR1_TE or   \ 0x2000 -- 0x2008
  USART1_CR1_RE or ; \ 0x2008 -- 0x200C

: USART6_BRR USART6 8 + ; ( -- addr ) \ #8

: SET_USART6_BRR ( -- )
  8B USART6_BRR ! ;

: SETUP_USART6_CR1
  USART6_CR1 @
  USART6_CR1_UE or
  USART6_CR1 ! ;

: SETUP_USART6_CR1_TE
  USART6_CR1 @
  USART6_CR1_TE or \ 0x00 -- 0x08
  USART6_CR1_RE or \ 0x08 -- 0x0C
  USART6_CR1 ! ;

: UNSET_USART1_CR1_UE
  USART1_CR1 @
  USART1_CR1_SETUPS or
  USART1_CR1_UE \ 0x2000 ( -- n )
  not and USART1_CR1 ! ;

: SET_USART1_CR1_UE \ SETUP_USART1_CR1
  USART1_CR1 @
  USART1_CR1_SETUPS or
  USART1_CR1 ! ;

: GPIOC 40020800 ; ( -- addr )
( 2.3 p.65 )

: GPIOC_MODER GPIOC 0 + ; ( -- addr )
( offset 0x00 8.4.1 p.281 )

: GPIOC_MODER! ( n -- )
  GPIOC_MODER @
  or GPIOC_MODER ! ;

\ combi AF_MODE both pins in one go: 0xA000
: PC6,7_AF_MODE A C << ; \ 0xA000
\ 0x2000 for pin 6 by itself
\ 0x8000 for pin 7 by itself
\ 0x2000 0x8000 or \ 0xA000 and there you have it. )

: SET_GPIOC_MODER_PC6_PC7_ALT_B ( -- )
  PC6,7_AF_MODE
  GPIOC_MODER! ;

: AF_MODE ( n -- ) \ modeled on the OUTPUT word
  \ F000 \ mask for PC6/PC7
  6 max 7 min \ restrict to pins 6 and 7
  2 * 1 + 1
  swap <<
  GPIOC_MODER! ;

: SET_GPIOC_MODER_PC6_PC7_ALT_A ( -- )
  6 AF_MODE
  7 AF_MODE ;

: AF_MASK_PC6_PC7 ( -- mask )
  F 1C << F 18 << or ; \ 0xFF000000

: PIN_MASK_PC6_PC7 7 1C << 7 18 << or ;
  \ want comment prev line

: nAF8 3 + ; ( n -- n+3 )

: GPIOC_AFRL GPIOC 20 + ; ( -- addr )

: GPIOC_AFRL! ( n -- )
  GPIOC_AFRL @ swap
  or GPIOC_AFRL ! ;

\ USART6 needs AF8 not AF7
: AF8_BITS ( n -- )
  \ as OUTPUT word is structured
  6 max 7 min
  4 *
  nAF8 1 swap << ; \ 0x88foo

: SET_AF8_BITS_GPIOC_AFRL
  6 AF8_BITS    \ 0x0800 0000
  7 AF8_BITS    \ 0x8000 0000
  or            \ 0x8800 0000
  GPIOC_AFRL! ;

: USART6_DR USART6 4 + ; ( -- addr )

: USART6_SR USART6 0 + ; ( -- addr )
  \ explicit 0x0
: TXE 80 ; \ USART_SR
: TC 40 ; \ USART_SR

: USART6_SR_TXE?
  TXE NOT
  USART6_SR @
  AND 0 = IF -1 EXIT THEN
  0 ;

: USART6_SR_TC?
  USART6_SR @ NOT
  TC AND
  0 = IF -1 EXIT THEN
  0 ;

: outsent_TC?
  BEGIN
    USART6_SR_TC?
  UNTIL -1 ;

: outc ( n -- ) \ functions as-is.
  FF AND USART6_DR !  outsent_TC? IF EXIT THEN
  ." ouch " ; \ ouch for 'never reaches here'

: SETUP_USART6 \ main entry into Lumex support
  SIO_RCC!
  SET_GPIOC_MODER_PC6_PC7_ALT_A
  SET_AF8_BITS_GPIOC_AFRL \ sets AF8 for PC6, PC7
  SETUP_USART6_CR1 \ UE set
  SET_USART6_BRR
  \ setup USART6 baud rate mantissa and fraction
  SETUP_USART6_CR1_TE ;
  \ setup USART6_CR1 TE and RE - UE done previously

: line_end 0A outc ;
: s_atd1=() \ explain this
  29 28 3D 31   64 74 61
  7 1 - FOR outc NEXT
  line_end ;

: atef=(1) \ Blue text for Lumex - want this almost always
  29 31 28 3D   66 65 74 61
  8 1 - FOR outc NEXT
  line_end s_atd1=() ;

: clearit 16 FOR 20 outc NEXT ;

: eflogo ( -- ) \ send eFort v7.20 to Lumex 96x8 RGB LED matrix
  clearit 3 delay

\        e  F  o  r  t  h     v  7  .  2  0
\ 20 20 65 46 6F 72 74 68 20 76 37 2E 32 30
  30 32 2E 37 76 20 68 74 72 6F 46 65 20 20

  E 1 - FOR outc NEXT ;

: signonx ( -- )
  setupled 3 blinks 22 delay
  SETUP_USART6 22 delay
  atef=(1) 18 delay \ make it blue - default is white
  9 delay eflogo HI ;

: signon ( -- )
  setupled 3 blinks 3 delay
  SETUP_USART6 3 delay
  clearit \ confirmation it got rewritten was weak
  atef=(1) 3 delay \ make it blue - default is white
  3 delay eflogo HI ;

: noutc ( .. n2 n1 n0 count -- )
  1 - FOR outc NEXT ;

: coffees 45 dup 46 dup
  30 43 20 dup 8 noutc ;

: show ( .. n2 n1 n0 count -- )
  noutc 9 delay clearit
  7 delay coffees ;

: paint ( n -- ) \ n is a color
  29 swap 28 3D 66 65 74 61 \ atef=(n)
  8 show ;

: slower ( -- ) speed C@ delay ;

: paint++ ( n -- ) \ display n colors sequentially
  0 swap 1 -
  FOR
    1 + dup paint slower
  NEXT drop ;

: low-red 2C ; : black-nocolor 40 ;
: low-blue 41 ; : low-green 44 ;
: low-cyan 45 ; : low-red 50 ;
: low-magenta 51 ; : low-yellow 54 ;
: low-white 55 ; : yellowish-green 58 ;
: brite-yellow-green DC ;
: brite-cyan-white DF ;

: plusses 15 BEGIN 1 - DUP 0 = 2B EMIT UNTIL ;

\ deedot - Thu Jan 28 10:31:02 EST 2021

VARIABLE oldbase

\ generate integer '4' in any base:
: gen4 ( -- 4 ) 1 dup + dup + ;

: d. ( n -- ) \ dot in decimal always
  BASE C@ oldbase C! gen4 dup + 1 + 1 + BASE
  C! . oldbase C@ BASE C! ;

: vers space ." 0.0.0.5.b2d-b3- "
  ." Thu Jan 28 15:49:22 UTC 2021 "
;

\  ' signon 1 + 'BOOT !
\  0 ERASE_SECTOR 33 delay
\  1 ERASE_SECTOR 33 delay
\  TURNKEY
