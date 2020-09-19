\ eforth v7.x 18 Sep 2020
\ target: STM32F405 Express, Adafruit
\ (STM Discovery inconvenient)

\ Write a program that blinks a known GPIO pin - at all

\ Write a second program that blinks that pin in morse code,
\ Using no more than four symbols (three is good; even two
\ is sufficient - even one is acceptable for first effort).

\ Initially, this is all done via CP2104 link to the STM32F405 Express,
\ in a terminal running on Debian AMD 64 host PC.

\ Later: will read the E1115F module using the STM32F405 Express,
\ and immediately annunciate a few selected keys in Morse Code
\ (light blinks, not a CW sidetone).

\ PURPOSE: to validate E1115F module, before investing
\ further development time into it.

\ REUSE:

\ Likely, some port setups from existing work can be
\ repurposed here, to save time for this 'quick' test. ;)

\ Look for, first, simple GPIO availability without
\ added work.
