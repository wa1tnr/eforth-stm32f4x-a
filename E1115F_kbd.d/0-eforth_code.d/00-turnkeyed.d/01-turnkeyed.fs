\ : newchar> 2E 900 ! ; \ location 900 has the underscore for the tochar word
\ upstream used 5F not 2E there.
\ >CHAR is that word and is stored near location 900 in RAM

: vers01  ." stm32eForth v7.20 " 3 spaces
  ." Sat Sep 19 00:37:26 UTC 2020 " cr
  ." target: STM32F405 Express " ;
: signon01  33 delay 3 spaces vers01 go00 ;

\ ' signon01 'BOOT ! 0 ERASE_SECTOR TURNKEY

\ That's basically how you automatically start an
\ arbitrary forth word.

.( 0 ERASE_SECTOR ) ( TURNKEY )

\ END.
