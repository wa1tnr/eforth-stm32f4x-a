# eForth for STM32F407 Discovery board

## local studies by wa1tnr
```
Target board:  **Adafruit STM32F405 Express**
Target board:  STM32F407 Discovery
```

### Fri Sep  4 14:48:33 UTC 2020

#### NEWS

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

Code that exists is either in a branch (not master)
or hasn't even been uploaded to github.  Sorry. ;)

That will change at some point; too much work was
done without public commits.  Hard to separate out
private code from public.

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
