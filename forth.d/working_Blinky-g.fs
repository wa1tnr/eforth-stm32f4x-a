COLD 

: RCC 40023800 ;
: RCC_AHB1ENR RCC 30 + ;
: GPIOCEN 4 ; ( 6.3.10 p.180 Rev 18 datasheet)
: RCC! RCC_AHB1ENR @ GPIOCEN OR RCC_AHB1ENR ! ;

: GPIOC 40020800 ; ( 2.3 p.65 )
: GPIOC_MODER GPIOC 0 + ; ( explicit alias, )
( offset 0x00 8.4.1 p.281 )
: MODER1 4 ; ( 8.4.1 p.281 ) ( GPIOC_1 )

: GPIOC_MODER!
  GPIOC_MODER @ MODER1 OR GPIOC_MODER ! ;

: GPIOC_ODR  GPIOC 14 + ; ( explicit alias, )
( offset 0x14 8.4.6 p.283 )
: GPIOC_BSRR GPIOC 18 + ; ( alias for )
( bit write access - see note 8.4.6 )

( BSRR scheme: 0xRRRRSSSS ) 
( word half-word byte )
( byte 8 bits  half word 16 bits word 32 bits )

: BIT17 2000 ;
: BIT1 2 ;
: BR1 BIT17 ;
: BS1 BIT1 ;

: GPIOC_BSRR_SETB! ( set PORTC_1 - turn )
( on the LED on D13, Adafruit STM32F405 )
  BS1 GPIOC_BSRR ! ;

: GPIOC_BSRR_CLR! ( clear PORTC_1 )
  BR1 GPIOC_BSRR ! ;

: SETUPLED RCC!
  GPIOC_MODER!
  GPIOC_BSRR_CLR! ;

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
: VMEMB         0 ; ( base of Forth vmem )
: FLASHB  8000000 ; ( base of on-chip flash - turnkey here )
: RAMB   20000000 ; ( base of physical RAM )
: DDP OVER OVER DUMP OVER OVER + ROT
  DROP SWAP ;
: NOTES.TXT CR ." notes.txt follows." CR
  ."    flash ......  example:   0x800BD00  ..  seems to be copied over"   CR
  ."    into RAM ...  example:  0x2000BD00  ..  irrespective of if the"    CR
  ."    uploaded program contains it."                                     CR
  CR
  ." Old code from other programs is not only still resident in flash"     CR
  ." memory, but is also copied into RAM, even though it is not currently" CR
  ." in use.  This can be leveraged. ;)" CR ;

 ( - - - - - )

: DOME FOR 1 DROP NEXT 2B EMIT 2B EMIT 2B EMIT 20 EMIT ;

0 [IF]

    special note: the '0 if' block comment - here - is
    gforth specific and was used - here - for the benefit
    of vim text editor, rather than of eforth.

    Additional Notes
    ~~~~~~~~~~~~~~~~

    newline 50 char 10 - delays in ms in minicom used 23 December
    control A T accesses the Terminal settings in minicom

    December 26 2019, 14:24 UTC:


    TURNKEY - PRELIMINARY INTRUX - tested just once, seems
    to be free of obvious error:

    Updating flash (Turnkey):

        ( cold boot - cycle power; then: )

        ( define new Forth words: ascii upload your code, )
        ( or paste to the serial terminal, using the mouse, )
        ( or type in by hand )

        0 ERASE_SECTOR
        TURNKEY


    Remove your old code (requires .bin upload using dfu-util):

        ( cold boot - cycle power; then: )

        COLD
        0 ERASE_SECTOR

    Cycle power again. TARGET NO LONGER RESPONSIVE (has no progam!)

    Restore eForth (pristine copy) with:

        Connect B0 to Vcc 3.3 VDC; press RESET.

        Then:

        $ cd ../0-Distribution.d/eForth
        $ dfu-util -a 0 --dfuse-address 0x08000000 -D ./stm32F4eForth720.bin-p

        Remove jumper from B0 to Vcc, press RESET.

    Old (original) eForth image will be loaded where it
    belongs, and the target is then ready for your code
    update, and another turnkey.

    This is the only way to remove a colon definition from the
    turnkey image stored on-device, in flashROM.


    (Untested) erase all eForth flashROM space:

        COLD
        0 ERASE_SECTOR ok
        1 ERASE_SECTOR ok
        2 ERASE_SECTOR ok
        3 ERASE_SECTOR ok

    This will erase (the lowest) 64 kB of on-device flashROM.

    That is most likely the limit for a turnkey'd system;
    unclear what is allowed above this 64 kB boundary,
    within the eForth context.

    Quite clear that legacy code still exists there, from
    other uploads of other development systems (such as
    Arduino IDE based development, as well as raw .bin
    uploads (mecrisp, for example).

    That'd (possibly) be dfu-util dependent; the Cube programmer
    sometimes will erase everything, seemingly (dfu-util seems
    more friendly in this regard).


    Dec 26  13:56 UTC:

    Dr. Ting's documentation says exactly what the three
    memory spaces are, and are for.


    An address of '0' (zero) is virtual memory, if local
    understanding is correct.

    VMEMB         0   ( base of Forth vmem )


    An address beginning with 0x20000000 is real RAM space.

    It is machine dependent and is consistent across several
    ARM Cortex M4 chips (possibly all of them?)

    RAMB   20000000   ( base of physical RAM )


    The third address space is physical flashROM, on-chip
    (not an SPI connected flashROM, which is also present,
    but unused, here).

    FLASHB  8000000   ( base of on-chip flash - turnkey here )

    This is the address passed to dfu-util to tell it where
    to <foo verb> what is uploaded as a .bin file to the
    target -- which appears to store it, either beginning at
    this location, or at a location related to this location
    (possibly offset).

    updated:
    Thu Dec 26 14:41:06 UTC 2019
[THEN]
