COLD 

: RCC 40023800 ;
: RCC_AHB1ENR RCC 30 + ;
: GPIOC 40020800 ;
: SETUPLED RCC @
  4 OR RCC_AHB1ENR ! ( GPIOCEN )
  4 GPIOC ! 0 GPIOC 14 + ! ;
: LED GPIOC 14 + 2 ;
: ON SWAP ! ;
: OFF DROP 0 SWAP ! ;
: DELAY DEPTH 1 - 0<
  IF EXIT THEN
  FOR 3 FOR 11 FOR 100
      FOR 1 DROP NEXT
  NEXT NEXT NEXT ;
: BDELAY 3 DELAY ;
: BDKDEL 8 DELAY ;
: LDELAY 188 DELAY ;
: FINISHMSG ."  done." ;
( 100 blinks per minute )
: BLINKS DEPTH 1 - 0<
  IF EXIT THEN
  1 - ( normalize )
  FOR LED ON BDELAY LED OFF BDKDEL
  NEXT ;
: LINIT  FFFFFF9D SETUPLED 3 BLINKS ;
: RAMB          0 ; ( base of Forth vmem? )
: FLASHB  8000000 ; ( base of flash - turnkey here )
: MEMB   20000000 ; ( base of RAM )
: DDP OVER OVER DUMP OVER OVER + ROT
  DROP SWAP ;

 ( - - - - - )

: DOME FOR 1 DROP NEXT 2B EMIT 2B EMIT 2B EMIT 20 EMIT ;

( newline 50 char 10 - delays in ms in minicom used 23 December )
( control A T accesses the Terminal settings in minicom )
