\ e1_mono-lab-a.fs
\ $ cp -p mono-lab-c.fs e1_mono-lab-a.fs
\ originally:  mono-lab-c.fs
\ ." Tue Jun 23 18:09:07 UTC 2020 "
( HEX ) ( COLD ) \ lower case vocabulary 24 May 09:07z
: emit EMIT ; : nop NOP ; : drop DROP ; : dup DUP ; : swap SWAP ;
: over OVER ; : or OR ; : and AND ; : rot ROT ; : 2drop 2DROP ;
: 2dup 2DUP ; : not NOT ; : negate NEGATE ; : abs ABS ; : max MAX ;
: min MIN ; : base BASE ; : depth DEPTH ; : here HERE ; : hex HEX ;
: decimal DECIMAL ; : key KEY ; : space SPACE ; : spaces SPACES ;
: type TYPE ; : cr CR ; : char CHAR ; : preset PRESET ; : dump DUMP ;
: .s .S ; : see SEE ; : words WORDS ; : cold COLD ;
: <1?  dup 1 - 0< IF drop -1 EXIT THEN drop 0 ; ( n -- BOOL )
: << LSHIFT ; ( n shifts -- )
: 2^ dup <1?  0< IF drop 1 EXIT THEN 1 swap << ; ( n -- )
: delay DEPTH 1 - 0< IF EXIT THEN FOR 3 FOR 11 FOR 100 FOR ( n -- )
  1 DROP NEXT NEXT NEXT NEXT ;
: bdelay 3 delay ; ( -- )
: bdkdel 8 delay ; ( -- )
: ldelay 122 delay ; ( -- )
: finishmsg ."  done." ; ( -- )
 ( - - - - - )
: RCC 40023800 ; ( -- addr )
: RCC_AHB1ENR RCC 30 + ; ( -- addr )
: RCC_APB2ENR RCC 44 + ; ( -- addr )
( - - - - - )
: GPIOC 40020800 ; ( -- addr ) ( 2.3 p.65 )
: GPIOC_MODER  GPIOC   0 + ; ( -- addr ) ( offset 0x00 8.4.1 p.281 )
: GPIOC_BSRR   GPIOC  18 + ; ( -- addr )
\ TODO: VERIFY 18 SEP - may not be same on GPIOC as it was on GPIOd
: GPIOC_MODER!  GPIOC_MODER @ or GPIOC_MODER ! ; ( n -- )
: GPIOCEN 1 2 << ; ( 6.3.10 p.180 Rev 18 datasheet) ( -- n )
: OUTPUT RCC_AHB1ENR @ GPIOCEN  or RCC_AHB1ENR !
  1 max 1 min 2 * 1 swap << GPIOC_MODER! ; ( n -- )
\ max min pin number range
\   1 1 << GPIOC_MODER! 1 2 << GPIOC_MODER! ; ( n -- )
\ : xxOUTPUT 1 max 1 min 2 * 1 swap << GPIOC_MODER! ; ( n -- )
: PC6,7_AF_MODE A C << ; \ 0xA000
: SET_GPIOC_MODER_PC6_PC7_ALT_B PC6,7_AF_MODE GPIOC_MODER! ; ( -- )
: GPIOC_BSRR!  GPIOC_BSRR ! ; ( n -- )
: BSX 2^ ; ( n -- n )
: BRX 10 + 2^ ; ( n -- n )
: led 1 ; \ PC1 D13 Adafruit STM32 target
: led!  GPIOC_BSRR! ; ( n -- )
: on BSX led! ; ( n -- )
: off BRX led! ; ( n -- )
: setupled led OUTPUT led off ; ( -- ) \ RCC! factored out
: blinks DEPTH 1 - 0< IF EXIT THEN 1 - FOR
    led on bdelay led off bdkdel NEXT ; ( n -- )
: linit FFFFFF9D setupled led off 3 blinks
  2 FOR bdkdel NEXT
  led off 3 blinks ; ( -- n )
: USART6EN 1 5 << ; ( -- n ) \ PC6/TX PC7/RX
: SIO_RCC!  \ SIO means Serial i/o
  RCC_AHB1ENR @ GPIOCEN  or RCC_AHB1ENR !
  RCC_APB2ENR @ USART6EN or RCC_APB2ENR ! ; ( -- )
: USART1 40011000 ; : USART6 40011400 ; ( -- addr )
: USART6_CR1 USART6 C + ; ( -- addr ) \ #12 ( 30.6.4 p.1010 )
: USART1_CR1 USART1 C + ; ( -- addr ) \ NO LOOKUP - lazy
: USART6_CR1_UE 1 D << ; : USART1_CR1_UE 1 D << ; \ 0x2000 ( -- n )
\ bit 13 ref page 1010
: USART6_CR1_TE 1 3 << ; : USART1_CR1_TE 1 3 << ; \ 0x8 ( -- n )
: USART6_CR1_RE 1 2 << ; : USART1_CR1_RE 1 2 << ; \ 0x4 ( -- n )
: USART1_CR1_SETUPS
  USART1_CR1_UE \ -- 0x2000
  USART1_CR1_TE or \ 0x2000 -- 0x2008
  USART1_CR1_RE or ; \ 0x2008 -- 0x200C
: USART6_BRR USART6 8 + ; ( -- addr ) \ #8
: SET_USART6_BRR 8B USART6_BRR ! ; ( -- )
: SETUP_USART6_CR1 USART6_CR1 @ USART6_CR1_UE or USART6_CR1 ! ;
: SETUP_USART6_CR1_TE USART6_CR1 @
  USART6_CR1_TE or \ 0x00 -- 0x08
  USART6_CR1_RE or \ 0x08 -- 0x0C
  USART6_CR1 ! ;
: UNSET_USART1_CR1_UE
  USART1_CR1 @ USART1_CR1_SETUPS or
  USART1_CR1_UE \ 0x2000 ( -- n )
  not and USART1_CR1 ! ;
: SET_USART1_CR1_UE \ SETUP_USART1_CR1
  USART1_CR1 @ USART1_CR1_SETUPS or USART1_CR1 ! ;
: AF_MODE ( n -- ) \ modeled on the OUTPUT word
  6 max 7 min 2 * 1 + 1 swap << GPIOC_MODER! ; \ PC6/PC7
: SET_GPIOC_MODER_PC6_PC7_ALT_A 6 AF_MODE 7 AF_MODE ; ( -- )
: AF_MASK_PC6_PC7 F 1C << F 18 << or ; \ 0xFF000000 ( -- mask )
: PIN_MASK_PC6_PC7 7 1C << 7 18 << or ; \ want comment
: nAF8 3 + ; ( n -- n+3 )
: GPIOC_AFRL GPIOC 20 + ; ( -- addr )
: GPIOC_AFRL!  GPIOC_AFRL @ swap or GPIOC_AFRL ! ; ( n -- )
: AF8_BITS ( n -- ) \ as OUTPUT word is structured
  6 max 7 min 4 * nAF8 1 swap << ; \ 0x88foo
: SET_AF8_BITS_GPIOC_AFRL
  6 AF8_BITS    \ 0x0800 0000
  7 AF8_BITS    \ 0x8000 0000
  or            \ 0x8800 0000
  GPIOC_AFRL! ;
: USART6_DR USART6 4 + ; ( -- addr )
: ralfc 15 BEGIN 1 - DUP 0 = 2B EMIT UNTIL ;
: USART6_SR USART6 0 + ; ( -- addr ) \ explicit 0x0
: TXE 80 ; \ USART_SR
: TC 40 ; \ USART_SR
: USART6_SR_TXE?  TXE NOT USART6_SR @ AND 0 = IF -1 EXIT THEN 0 ;
: OLD_A_USART6_SR_TC?  TC NOT USART6_SR @ AND 0 = IF -1 EXIT THEN 0 ;
: USART6_SR_TC?  USART6_SR @ NOT TC AND 0 = IF -1 EXIT THEN 0 ;
: boutsent?  BEGIN USART6_SR_TXE?  2B EMIT UNTIL -1 ;
: outsent_TC?  BEGIN USART6_SR_TC?  UNTIL -1 ;
: outc FF AND USART6_DR !  outsent_TC? IF EXIT THEN
  ." ouch " space ; ( n -- )
: SETUP_USART6 \ main entry into Lumex support
  SIO_RCC!  SET_GPIOC_MODER_PC6_PC7_ALT_A
  SET_AF8_BITS_GPIOC_AFRL \ sets AF8 for PC6, PC7
  SETUP_USART6_CR1 \ UE set
  SET_USART6_BRR \ setup USART6 baud rate mantissa and fraction
  SETUP_USART6_CR1_TE \ setup USART6_CR1 TE and RE - UE done previously
;
: vers space ." 0.0.0.0.a- "
  ." Fri Sep 18 19:49:47 UTC 2020 "
;
: gro SETUP_USART6 ;
: go cr cr cr vers cr cr ;

\ END.
