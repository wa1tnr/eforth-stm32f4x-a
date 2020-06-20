# eForth for STM32F407 Discovery board

## local studies by wa1tnr

Primary Target board:  STM32F407VGT6 Discovery
other Target board:  Adafruit STM32F405 Express


### Sat Jun 20 13:09:01 UTC 2020

Current effort:

get a second USART working, on PC6/TX, PC7/RX pair.

Intent is to send messages to a Lumex 96x8 display,
while retaining full dialogue with the target, in
the eForth environment.

The remainder of this README.md file has not been
reviewed for current relevance (June 20, 2020).

# - - - -

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

TX/RX (for USART1) is brought out on (STM32F405 Express) SCL/SDA.


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
