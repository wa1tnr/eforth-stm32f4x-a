\ 03-turnkeyed.fs    from:  testing-af-b.fs
\ Sat Sep 19 21:43:56 UTC 2020

\ was: testing-af-b.fs
\ new work 19 September.

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
  USART1_CR1_UE \ -- 0x2000
  USART1_CR1_TE or \ 0x2000 -- 0x2008
  USART1_CR1_RE or \ 0x2008 -- 0x200C
;
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
  USART1_CR1 !
;
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
: GPIOC_AFRL! ( n -- )
  GPIOC_AFRL @ swap
  or GPIOC_AFRL ! ;
\ USART6 needs AF8 not AF7
: AF8_BITS ( n -- ) \ as OUTPUT word is structured
  6 max 7 min
  4 *
  nAF8 1 swap << \ 0x88foo
;
: SET_AF8_BITS_GPIOC_AFRL
  6 AF8_BITS    \ 0x0800 0000
  7 AF8_BITS    \ 0x8000 0000
  or            \ 0x8800 0000
  GPIOC_AFRL! ;
: USART6_DR USART6 4 + ; ( -- addr )
: USART6_SR USART6 0 + ; ( -- addr ) \ explicit 0x0
: TXE 80 ; \ USART_SR
: TC 40 ; \ USART_SR
: USART6_SR_TXE?
  TXE NOT
  USART6_SR @
  AND 0 = IF -1 EXIT THEN
  0 ;
: OLD_A_USART6_SR_TC?
  TC NOT USART6_SR @
  AND 0 = IF -1 EXIT THEN
  0 ;
: USART6_SR_TC?
  USART6_SR @ NOT
  TC AND \ stopped abruptly to solve this word's definition
  0 = IF -1 EXIT THEN
  0 ;
: boutsent?  BEGIN USART6_SR_TXE?
  2B EMIT UNTIL -1 ;
: outsent_TC?
  BEGIN
    USART6_SR_TC?
  UNTIL -1 ;
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
: plus_sign 2B ;
: hash_symb 23 ;
: carr_ret   D ;
: goneff
  SETUP_USART6
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
: line_end 0A outc ;
\ : ralfc 15 BEGIN 1 - DUP 0 = 2B EMIT UNTIL ;
\ END.
