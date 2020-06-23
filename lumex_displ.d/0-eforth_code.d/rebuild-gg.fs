\ ." Tue Jun 23 10:50:22 UTC 2020 "

\  Not very sure that TC gets validated;
\  may be writing blind (without feedback
\ from USART6_SR bit 6: TC, to guide).  

\  Working program - talks to Lumex 96x8 successfully.
\  Bugs very much expected - initial effort complete.

\ found AF8 as an error - renamed nAF8

( HEX ) ( COLD )

: GPIOCEN 1 2 << ; ( -- n )
( 6.3.10 p.180 Rev 18 datasheet)

\ PC6/TX PC7/RX
: USART6EN 1 5 << ; ( -- n )

\ PB_6/TX,  USART1

: SIO_RCC! ( -- ) ( verif SED )
  RCC_AHB1ENR @
  GPIOCEN or
  RCC_AHB1ENR !

  \ RCC_AHB1ENR @
  \ GPIODEN or
  \ RCC_AHB1ENR !

  RCC_APB2ENR @
  USART6EN or
  RCC_APB2ENR !

\ try to break it a little by setting USART1EN, again.
  \ RCC_APB2ENR @
  \ USART1EN or
  \ RCC_APB2ENR !
;

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

: USART6_CR1_SETUPS
  \ USART6_CR1_UE \ -- 0x2000
  \ USART6_CR1_TE or \ 0x2000 -- 0x2008
  \ USART6_CR1_RE or \ 0x2008 -- 0x200C
;

: USART1_CR1_SETUPS
  USART1_CR1_UE \ -- 0x2000
  USART1_CR1_TE or \ 0x2000 -- 0x2008
  USART1_CR1_RE or \ 0x2008 -- 0x200C
;

\ 0x23 hash symbol, #35 not seen so not sure what Ting is saying.
: USART6_BRR USART6 8 + ; ( -- addr ) \ #8
\ Ting had USART_BR not USART_BRR not sure why
\ 0x8B  or #139

: SET_USART6_BRR ( -- )
  \ USART6_BRR @
  8B \ or
  USART6_BRR !
;

: SETUP_USART6_CR1
  USART6_CR1 @
  USART6_CR1_UE or
  USART6_CR1 ! \ old end of word
;

: SETUP_USART6_CR1_TE
  USART6_CR1 @
  USART6_CR1_TE or \ 0x00 -- 0x08
  USART6_CR1_RE or \ 0x08 -- 0x0C
  USART6_CR1 !
;

  \ USART6_CR1_UE \ -- 0x2000
  \ USART6_CR1_TE or \ 0x2000 -- 0x2008
  \ USART6_CR1_RE or \ 0x2008 -- 0x200C

: UNSET_USART1_CR1_UE
  USART1_CR1 @
  USART1_CR1_SETUPS or
  USART1_CR1_UE \ 0x2000 ( -- n )
  not and USART1_CR1 !
  \ FFFF0FFF
  \ FFFFDFFF and \ mask bit 13 to unset it
;

: SET_USART1_CR1_UE \ SETUP_USART1_CR1
  USART1_CR1 @
  USART1_CR1_SETUPS or
  \ USART1_CR1_UE \ 0x2000 ( -- n )
  USART1_CR1 !
;

: GPIOC 40020800 ; ( -- addr )
( 2.3 p.65 )

: GPIOC_MODER GPIOC 0 + ; ( -- addr )
( explicit alias, )
( offset 0x00 8.4.1 p.281 )

\ : MODER1 1 2 << ; ( -- n ) ( 4 )
( 8.4.1 p.281 ) ( GPIOC_1 )
\ : MODER2 1 4 << ; ( -- n ) ( 16 aka 0x10 )
\ : MODER3 1 6 << ; ( -- n ) ( 64 aka 0x40 )

: GPIOC_MODER! ( n -- )
  GPIOC_MODER @
  or GPIOC_MODER ! ;

\ combi AF_MODE both pins in one go: 0xA000
: PC6,7_AF_MODE A C << ; \ 0xA000
\ 0x2000 for pin 6 by itself
\ 0x8000 for pin 7 by itself
\ 0x2000 0x8000 or \ 0xA000 and there you have it. ;)

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

: PIN_MASK_PC6_PC7 7 1C << 7 18 << or ; \ want comment

: nAF8 3 + ; ( n -- n+3 )

: GPIOC_AFRL GPIOC 20 + ; ( -- addr )
\ USART6 needs AF8 not AF7

: GPIOC_AFRL! ( n -- )
  GPIOC_AFRL @ swap
  or GPIOC_AFRL ! ;

: AF8_BITS ( n -- ) \ as OUTPUT word is structured
  6 max 7 min
  4 *
  nAF8 1 swap << \ 0x88foo
  \ GPIOC_AFRL!
;

: SET_AF8_BITS_GPIOC_AFRL
  6 AF8_BITS    \ 0x0800 0000
  7 AF8_BITS    \ 0x8000 0000
  or            \ 0x8800 0000
  GPIOC_AFRL! ;

: USART6_DR USART6 4 + ; ( -- addr )

: ralfc 15 BEGIN 1 - DUP 0 = 2B EMIT UNTIL ;
\ ralfc+++++++++++++++++++++ ok
\ ralfc+++++++++++++++++++++ ok
\      123456789012345678901
\ so this 'ralfc' word emits 21 copies of the plus sign,
\ even factoring it a '1 -' right at the top of the loop.

\ when UNTIL is encountered, it looks for a BOOL and acts on that.
\ MAYBE.

: USART6_SR USART6 0 + ; ( -- addr ) \ explicit 0x0

: TXE 80 ; \ USART_SR
: TC 40 ; \ USART_SR

: USART6_SR_TXE?
  TXE NOT
  USART6_SR @
  AND 0 = IF -1 EXIT THEN
  0
;

: OLD_A_USART6_SR_TC?
  TC NOT
  USART6_SR @
  AND 0 = IF -1 EXIT THEN
  0
;

: USART6_SR_TC?
  USART6_SR @ NOT
  TC AND \ stopped abruptly to solve this word's definition
  0 = IF -1 EXIT THEN
  0
;

: boutsent?  BEGIN USART6_SR_TXE?  2B EMIT UNTIL -1 ;

: outsent_TC?
  BEGIN
    \ USART6_SR_TXE? is a status register fetch, with
    \  artificial true or false results, determined
    \  by forth, locally.
    USART6_SR_TC?
    \ DROP 0 NOT \ force very first iteration to TRUE
    \ 2B EMIT
    \ correct, but never returns:
    \ NOT
  UNTIL
   \ 1 blinks
  -1
  \ ." outsent_TC? " space
;

: outc ( n -- ) \ functions as-is.
  FF AND USART6_DR !  outsent_TC? IF EXIT THEN
  ." ouch " space ;

: SETUP_USART6 \ main entry into Lumex support
  SIO_RCC!
  SET_GPIOC_MODER_PC6_PC7_ALT_A \ fine and the improved one
  SET_AF8_BITS_GPIOC_AFRL \ sets AF8 for PC6, PC7
  SETUP_USART6_CR1 \ UE set
  SET_USART6_BRR \ setup USART6 baud rate mantissa and fraction
  SETUP_USART6_CR1_TE \ setup USART6_CR1 TE and RE - UE done previously
;

: goneff
  SETUP_USART6
  \ ." USART6  was setup in goneff. " cr
  \ output 12 hash symbols per Ting
  hash_symb outc hash_symb outc hash_symb outc hash_symb outc
  \ first real data output:
  plus_sign outc plus_sign outc plus_sign outc carr_ret outc
  5 blinks 33 delay
  hash_symb outc hash_symb outc hash_symb outc carr_ret outc
  5 blinks 33 delay
  \ SET_USART1_CR1_UE \ absolutely required following UNSET_USART1_CR1_UE
                    \ or USART1, the command line interpreter, never returns.
;

\ ++++++++++++++++++++++++++++++++++++
\ 123456789012345678901234567890123456 36 total

\ REDEFINE intended:
: go
  led OUTPUT
  2 blinks 43 delay
  \ UNSET_USART1_CR1_UE
  goneff
  \ SET_USART1_CR1_UE

  7 blinks 33 delay
  30 spaces 2B emit 2B emit 2B emit CR
;

\ REDEFINE intended:
: vers space ." 0.0.0.4.c- "
  ." Tue Jun 23 10:50:22 UTC 2020 "
; \ green orange red blue
 ( trial run: ) ( LINIT ) ( green OUTPUT )
 ( green on ) ( green off )
 ( 5 blinks ) ( led on ldelay led off )

.( 0 ERASE_SECTOR ) ( TURNKEY )

: crufta
vers
cr
led OUTPUT
2 blinks 43 delay
SETUP_USART6
." setup of usart6 complete. " cr
hex
\ this sent 'ABCDE' five concurrent chars to Lumex:
41 outc 42 outc 43 outc 44 outc 45 outc
;

: c0ffee-sign
 \ atef=(1)
 0A 29 31 28     3D 66 65 74     61
 9 FOR outc NEXT

 \ atd1=()
 0A 29 28 3D     31 64 74 61
 8 FOR outc NEXT

 \ at80=(0,0,4)
 0A 29 34 2C   30 2C 30 28   3D 30 38 74  61
 D FOR outc NEXT
;

 ( - - - - - )

\ c0ffee sign hexdump:

\ 00000000  61 74 65 66 3d 28 31 29  0a 61 74 64 31 3d 28 29  |atef=(1).atd1=()|
\ 00000010  0a 61 74 38 30 3d 28 30  2c 30 2c 34 29 0a 61 74  |.at80=(0,0,4).at|
\ 00000020  64 31 3d 28 29 0a 61 74  38 30 3d 28 30 2c 31 2c  |d1=().at80=(0,1,|
\ 00000030  30 29 0a 61 74 64 31 3d  28 29 0a 61 74 38 30 3d  |0).atd1=().at80=|
\ 00000040  28 30 2c 32 2c 43 29 0a  61 74 64 31 3d 28 29 0a  |(0,2,C).atd1=().|
\ 00000050  61 74 38 30 3d 28 30 2c  33 2c 30 29 0a 61 74 64  |at80=(0,3,0).atd|
\ 00000060  31 3d 28 29 0a 61 74 38  30 3d 28 30 2c 34 2c 46  |1=().at80=(0,4,F|
\ 00000070  29 0a 61 74 64 31 3d 28  29 0a 61 74 38 30 3d 28  |).atd1=().at80=(|
\ 00000080  30 2c 35 2c 46 29 0a 61  74 64 31 3d 28 29 0a 61  |0,5,F).atd1=().a|
\ 00000090  74 38 30 3d 28 30 2c 36  2c 45 29 0a 61 74 64 31  |t80=(0,6,E).atd1|
\ 000000a0  3d 28 29 0a 61 74 38 30  3d 28 30 2c 37 2c 45 29  |=().at80=(0,7,E)|
\ 000000b0  0a 61 74 64 31 3d 28 29  0a 61 74 65 66 3d 28 34  |.atd1=().atef=(4|
\ 000000c0  29 0a 61 74 64 31 3d 28  29 0a 61 74 38 30 3d 28  |).atd1=().at80=(|
\ 000000d0  30 2c 38 2c 46 29 0a 61  74 64 31 3d 28 29 0a 61  |0,8,F).atd1=().a|
\ 000000e0  74 38 30 3d 28 30 2c 39  2c 45 29 0a 61 74 64 31  |t80=(0,9,E).atd1|
\ 000000f0  3d 28 29 0a 61 74 38 30  3d 28 30 2c 31 30 2c 43  |=().at80=(0,10,C|
\ 00000100  29 0a 61 74 64 31 3d 28  29 0a 61 74 38 30 3d 28  |).atd1=().at80=(|
\ 00000110  30 2c 31 31 2c 37 29 0a  61 74 64 31 3d 28 29 0a  |0,11,7).atd1=().|
\ 00000120  61 74 38 30 3d 28 30 2c  31 32 2c 44 29 0a 61 74  |at80=(0,12,D).at|
\ 00000130  64 31 3d 28 29 0a 61 74  38 30 3d 28 30 2c 31 33  |d1=().at80=(0,13|
\ 00000140  2c 34 29 0a 61 74 64 31  3d 28 29 0a 61 74 38 30  |,4).atd1=().at80|
\ 00000150  3d 28 30 2c 31 34 2c 43  29 0a 61 74 64 31 3d 28  |=(0,14,C).atd1=(|
\ 00000160  29 0a 61 74 38 30 3d 28  30 2c 31 35 2c 39 29 0a  |).at80=(0,15,9).|
\ 00000170  61 74 64 31 3d 28 29 0a  61 74 65 66 3d 28 33 32  |atd1=().atef=(32|
\ 00000180  29 0a 61 74 64 31 3d 28  29 0a 61 74 39 65 3d 28  |).atd1=().at9e=(|
\ 00000190  39 35 2c 30 29 0a 61 74  64 31 3d 28 29 0a 61 74  |95,0).atd1=().at|
\ 000001a0  65 66 3d 28 31 29 0a 61  74 64 31 3d 28 29 0a 61  |ef=(1).atd1=().a|
\ 000001b0  74 39 65 3d 28 39 35 2c  32 29 0a 61 74 64 31 3d  |t9e=(95,2).atd1=|
\ 000001c0  28 29 0a                                          |().|
\ 000001c3
