\ testing-ac.fs
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
: bdelay 3 delay ; : bdkdel 8 delay ; : ldelay 122 delay ; ( -- )
: finishmsg ."  done." ; ( -- )
: RCC 40023800 ; : RCC_AHB1ENR RCC 30 + ;
: RCC_APB2ENR RCC 44 + ; ( -- addr )
: GPIOC 40020800 ; : GPIOC_MODER GPIOC 0 + ;
: GPIOC_BSRR GPIOC 18 + ; ( -- addr )
: GPIOC_MODER!  GPIOC_MODER @ or GPIOC_MODER ! ; ( n -- )
: GPIOCEN 1 2 << ; ( -- n ) \ verify this
: OUTPUT RCC_AHB1ENR @ GPIOCEN  or RCC_AHB1ENR !
  1 max 1 min 2 * 1 swap << GPIOC_MODER! ; ( n -- )
: PC6,7_AF_MODE A C << ; \ 0xA000
: SET_GPIOC_MODER_PC6_PC7_ALT_B PC6,7_AF_MODE GPIOC_MODER! ; ( -- )
: GPIOC_BSRR!  GPIOC_BSRR ! ; ( n -- )
: BSX 2^ ; : BRX 10 + 2^ ; ( n -- n )
: led 1 ; \ PC1 D13 Adafruit STM32 target
: led!  GPIOC_BSRR! ; : on BSX led! ; : off BRX led! ; ( n -- )
: setupled led OUTPUT led off ; ( -- )
: blinks DEPTH 1 - 0< IF EXIT THEN 1 - FOR
    led on bdelay led off bdkdel NEXT ; ( n -- )
: linit FFFFFF9D setupled led off 3 blinks 2 FOR bdkdel NEXT
  led off 3 blinks ; ( -- n )
