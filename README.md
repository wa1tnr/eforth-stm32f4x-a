# eForth for STM32F407 Discovery board

## local studies by wa1tnr
`Target board:` **STM32F407 Discovery**

`Target board:` **Adafruit STM32F405 Express**

### Sun Dec 20 14:24:03 UTC 2020

#### NEWS

##### TURNKEY with eForth logo in blue - working

As of 20 December, the turnkey code seems to be working
well - displays the eForth logo on the Lumex 96x8 RGB
display (it uses TTL serial to talk, and a Hayes-like
'AT' command set, similar (roughly) to an older modem.

##### STM32F407 Discovery - primary target

The STM32F407 Discovery was received in the post, from
DigiKey, earlier in the year, and became the primary
target for the program, as a result (needed to have
the extra GPIO pins not available on the Adafruit
target, which is otherwise suitable).

##### Lumex 96x8 RGB matrix, 3mm pitch, supported.

The Lumex display uses Hayes style 'AT' commands.

The trick to messaging to the display is to do so
continuously; if there's much of a delay, then the
display will not accumulate the message - it will
start over again at the left margin, after blanking
the entire display.

The Lumex display support was written entirely in Forth. ;)

Including the setup for the second USART pin pair.

### older: Tue Dec 17 22:00:36 UTC 2019

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


**REFERENCES**

   **Markdown**:

   [https://guides.github.com/features/mastering-markdown/]

END.
