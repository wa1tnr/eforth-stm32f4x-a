: vers02  2 spaces ." stm32eForth v7.20 " 3 spaces
  ." Sat Sep 19 00:47:42 UTC 2020 " cr
  ." target: STM32F405 Express " ;
: signon02  33 delay 3 spaces vers02 go00 ;

\ ' signon02 'BOOT ! 0 ERASE_SECTOR TURNKEY

\ END.
