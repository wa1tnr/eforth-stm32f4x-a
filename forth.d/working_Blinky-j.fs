COLD

: RCC ( -- addr )
  40023800 ;

: RCC_AHB1ENR ( -- addr )
  RCC 30 + ;

: << ( n shifts -- )
  LSHIFT ;
  ( 1 - FOR 2* NEXT )

: GPIOCEN 1 2 << ; ( -- n )
( 6.3.10 p.180 Rev 18 datasheet)

: RCC! ( -- )
  RCC_AHB1ENR @ GPIOCEN OR RCC_AHB1ENR ! ;

: GPIOC ( -- addr )
  40020800 ; ( 2.3 p.65 )

: GPIOC_MODER (  -- addr )
  GPIOC 0 + ; ( explicit alias, )
( offset 0x00 8.4.1 p.281 )

: MODER1 1 2 << ; ( -- n ) ( 4 )
( 8.4.1 p.281 ) ( GPIOC_1 )
( here's how to setup any port pin on PORTC )
(  as OUTPUT: )
( 1 4 << ok ) ( 1 is just the universal binary bit )
( 4 here is the selected port pin - PORTC MODER2 - low bit )
( as in 8.4.1 )
( GPIOC_MODER @ SWAP OR GPIOC_MODER ! ok )
( that was PORTC_2  aka  PORTC MODER2 )

( PORTC_1 is the D13 LED )

( here's PORTC_3 aka PORTC MODER3 )
( 1 6 << ok )
( GPIOC_MODER @ SWAP OR GPIOC_MODER ! ok )

: MODER2 1 4 << ; ( -- n ) ( 16 aka 0x10 )
: MODER3 1 6 << ; ( -- n ) ( 64 aka 0x40 )

( : GPIOC_MODER1! )
( GPIOC_MODER @ MODER1 OR GPIOC_MODER ! ; )

: GPIOC_MODER!
  GPIOC_MODER @ SWAP   OR GPIOC_MODER ! ; ( n -- ) ( wants MODER2 or MODER3 &c )

: OUTPUT ( n -- )
  1 MAX 3 MIN ( kludge don't want pin0 or > pin3 )
  2 * 1 SWAP << GPIOC_MODER! ;

( trial run: )
( LINIT ok )
( 1 ON ok )
( 1 OFF ok )
( 2 OUTPUT ok )
( 2 ON ok )
( 2 OFF ok )
( 3 OUTPUT ok )
( 3 ON ok )
( 3 OFF ok )

( evidently GPIOC_ODR effort is incomplete )
( nothing asks for this word, anymore: )

: GPIOC_ODR ( -- addr )
  GPIOC 14 + ; ( explicit alias, )
( offset 0x14 8.4.6 p.283 )

: GPIOC_BSRR ( -- addr )
  GPIOC 18 + ; ( alias for )
( bit write access - see note 8.4.6 )

( BSRR scheme: 0xRRRRSSSS ) 
( word half-word byte )
( byte 8 bits  half word 16 bits word 32 bits )

: <1? ( n -- BOOL )
  DUP 1 - 0< IF
    DROP -1 EXIT
  THEN
  DROP 0 ;

: 2^ ( n -- )
  DUP <1?  0< IF
    DROP 1 EXIT
  THEN
  1 SWAP << ;

: BSX ( n -- n ) ( BS1 for PORTC_1 )
  2^ ;

: BRX ( n -- n ) ( BR1 for PORTC_1 )
  10 + 2^ ;

: GPIOC_BSRR! ( n -- )
( generic - may setb or clr the port pin )
  GPIOC_BSRR ! ;

: LED ( -- n )
  1 ; ( PORTC_1 )

: LED2 ( -- n )
  2 ; ( PORTC_2 )

: LED3 ( -- n )
  3 ; ( PORTC_3 )

: LED! ( n -- )
  GPIOC_BSRR! ;

: ON ( n -- )
  BSX LED! ;
: OFF ( n -- )
  BRX LED! ;

: SETUPLED ( -- )
  RCC!
  LED OUTPUT ( GPIOC_MODER1! )
  LED OFF ;

( LED GPIOC 14 + 2 )

: DELAY ( n -- )
  DEPTH 1 - 0<
  IF EXIT THEN
  FOR 3 FOR 11 FOR 100
      FOR 1 DROP NEXT
  NEXT NEXT NEXT ;

: BDELAY ( -- )
  3 DELAY ;
: BDKDEL ( -- )
  8 DELAY ;
: LDELAY ( -- )
  188 DELAY ;

: FINISHMSG ( -- )
  ."  done." ;
( 100 blinks per minute )

( blinks isn't facile enough to )
( blink a specified port on command )
( it always blinks PORTC_1 aka D13 )

: BLINKS ( n -- )
  DEPTH 1 - 0<
  IF EXIT THEN
  1 - ( normalize )
  FOR LED ON BDELAY LED OFF BDKDEL
  NEXT ;

: LINIT ( -- n )
  FFFFFF9D SETUPLED 3 BLINKS ;

: VMEMB         0 ; ( base of Forth vmem )
: FLASHB  8000000 ; ( base of on-chip flash - turnkey here )
: RAMB   20000000 ; ( base of physical RAM )

: DDP ( addr count -- addr+count count )
  OVER OVER DUMP OVER OVER + ROT
  DROP SWAP ;

: NOTES.TXT ( -- )
  CR ." notes.txt follows." CR
  ."    flash ......  example:   0x800BD00  ..  seems to be copied over"   CR
  ."    into RAM ...  example:  0x2000BD00  ..  irrespective of if the"    CR
  ."    uploaded program contains it."                                     CR
  CR
  ." Old code from other programs is not only still resident in flash"     CR
  ." memory, but is also copied into RAM, even though it is not currently" CR
  ." in use.  This can be leveraged. ;)" CR ;

: VERS SPACE ." 0.0.0.1.d-" ;

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
    Thu Dec 26 21:22:09 UTC 2019

: BIT1 2 ; ( -- n )
: BIT17 20000 ; ( -- n )
: BS1_NO BIT1 ; ( -- n )
: BR1_NO BIT17 ; ( -- n )

[THEN]
