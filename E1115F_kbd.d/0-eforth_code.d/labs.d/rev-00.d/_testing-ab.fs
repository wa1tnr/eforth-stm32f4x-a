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
