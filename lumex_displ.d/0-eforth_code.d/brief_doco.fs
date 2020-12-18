\ Sat Jun 20 08:10:45 UTC 2020

: vers space
  ." 0.0.0.1.u- "
  ." Sat Jun 20 08:10:45 UTC 2020 "
;

.( 0 ERASE_SECTOR )

\ green orange red blue
 ( trial run: )
 ( LINIT )
 ( green OUTPUT )
 ( green on )
 ( green off )
 ( orange OUTPUT )
 ( orange on )
 ( orange off )
 ( red OUTPUT )
 ( red on )
 ( red off )

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

 ( - - - - - )
