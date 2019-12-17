# eForth for STM32F407 Discovery board

## local studies by wa1tnr

Target board:  Adafruit STM32F405 Express


### Tue Dec 17 22:00:36 UTC 2019

from: doc/notes.txt

Tue Dec 17 22:55:21 UTC 2019

Dr. C.H. Ting's eForth for the STM32F407 Discovery board:

    http://forth.org/OffeteStore/OffeteStore.html

    STM32 eForth - 2014

        http://forth.org/OffeteStore/2165_stm32eForth720.zip


Target board:  Adafruit STM32F405 Express

  http://adafru.it/4382

Dr. Ting's eForth runs (unmodified) on the Adafruit board.
GPIO support is on four unavailable pins, however.

TX/RX is brought out on (STM32F405 Express) SDA and SCL (or vice-versa).


rename repository:

  eforth-stm4x-a > eforth-stm32f4x-a


Tue Dec 17 22:03:02 UTC 2019

** initial-dev
  master

 $ git checkout -b initial-dev
Switched to a new branch 'initial-dev'
 $ mkdir doc
 $ cd doc
 $ rvim -n notes.txt
 $ git branch >> notes.txt

 and the rest of what's seen here (now).

END.
