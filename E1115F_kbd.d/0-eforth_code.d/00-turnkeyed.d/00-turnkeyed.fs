\ 00-turnkeyed.fs    from:  testing-af.fs
\ Sat Sep 19 00:16:50 UTC 2020
\ Fairly robust now.  Question about GPIOCEN resolved.
: emit EMIT ; : nop NOP ; : drop DROP ; : dup DUP ; : swap SWAP ;
: over OVER ; : or OR ; : and AND ; : rot ROT ; : 2drop 2DROP ;
: 2dup 2DUP ; : not NOT ; : negate NEGATE ; : abs ABS ; : max MAX ;
: min MIN ; : base BASE ; : depth DEPTH ; : here HERE ; : hex HEX ;
: decimal DECIMAL ; : key KEY ; : space SPACE ; : spaces SPACES ;
: type TYPE ; : cr CR ; : char CHAR ; : preset PRESET ; : dump DUMP ;
: .s .S ; : see SEE ; : words WORDS ; : cold COLD ;
: << LSHIFT ; ( n shifts -- )
: <1?  dup 1 - 0< IF drop -1 EXIT THEN drop 0 ; ( n -- BOOL )
: 2^ dup <1?  0< IF drop 1 EXIT THEN 1 swap << ; ( n -- )
: delay DEPTH 1 - 0< IF EXIT THEN FOR 3 FOR 11 FOR 100 FOR ( n -- )
  1 DROP NEXT NEXT NEXT NEXT ;
: bdelay 3 delay ; : bdkdel 8 delay ; : ldelay 122 delay ; ( -- )
: finishmsg ."  done." ; ( -- )
: RCC 40023800 ; : RCC_AHB1ENR RCC 30 + ; : RCC_APB2ENR RCC 44 + ; ( -- addr )
: GPIOC 40020800 ; : GPIOC_MODER GPIOC 0 + ;
: GPIOC_BSRR GPIOC 18 + ; ( -- addr )
: GPIOC_MODER! GPIOC_MODER @ or GPIOC_MODER ! ; ( n -- )
: GPIOCEN 1 2 << ; ( -- n )
: GPIOC_RCC!  RCC_AHB1ENR @ GPIOCEN or RCC_AHB1ENR ! ; ( -- )
: OUTPUT GPIOC_RCC!  1 max 1 min 2 * 1 swap << GPIOC_MODER! ; ( n -- )
: PC6,7_AF_MODE A C << ; \ 0xA000
: SET_GPIOC_MODER_PC6_PC7_ALT_B PC6,7_AF_MODE GPIOC_MODER! ; ( -- )
: GPIOC_BSRR! GPIOC_BSRR ! ; ( n -- ) : BSX 2^ ; : BRX 10 + 2^ ; ( n -- n )
: led 1 ; \ PC1 D13 STM32F405
: led!  GPIOC_BSRR! ; : on BSX led! ; : off BRX led! ; ( n -- )
: setupled led OUTPUT led off ; ( -- )
: blinks DEPTH 1 - 0< IF EXIT THEN 1 -
  FOR led on bdelay led off bdkdel NEXT ; ( n -- )
: linit FFFFFF9D setupled led off 3 blinks 5 FOR bdkdel NEXT
  led off 3 blinks ; ( -- n )
: USART6EN 1 5 << ; ( -- n ) \ PC6/TX PC7/RX
: SIO_RCC! ( -- )
  GPIOC_RCC!  RCC_APB2ENR @ USART6EN or RCC_APB2ENR ! ;

\ this DOES blink if the target isn't power cycled:
: bypass-a led 1 max 1 min 2 * 1 swap << GPIOC_MODER! led off 3 blinks ;
\    bypass-a ok   \ no blink
\    bypass-a ok   \ no blink
\    GPIOC_RCC! bypass-a ok   \ NOW, it does blink

: vers00  ." stm32eForth v7.20 " 3 spaces
  ." Sat Sep 19 00:16:50 UTC 2020 " cr
  ." target: STM32F405 Express " ;
: go00 linit ;   : signon00 3 spaces vers00 go00 ;

\ ' signon00 'BOOT ! 0 ERASE_SECTOR TURNKEY

\ : signon go ; \ alias
\ ' signon 1 + 'BOOT ! 0 ERASE_SECTOR TURNKEY

\ That's basically how you automatically start an
\ arbitrary forth word.

\ The '1 +' utterance is superfluous, there.

( HEX ) ( COLD )

.( 0 ERASE_SECTOR ) ( TURNKEY )

 ( - - - - - )
\ END.
