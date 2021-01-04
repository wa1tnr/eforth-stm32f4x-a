\ 04 January 2021 05:02 UTC
\ cold
\ stm32eForth v7.20
\ preset ok
\ C0FFEE .s  C0FFEE  ok

\ use 'char' interactively to generate the desired message.

\ char E dup char F dup char 0 ok
\ char C 20 ok

\ .s  C0FFEE 45 45 46 46 30 43 20  ok

\ The dot-S stack report generates the bytes needed for 'outc'
\ (which writes characters in ASCII, to the Lumex 96x8 display).

\ collated backwards, of course:

\ : c0ffeesign 45 45 46 46 30 43 20 6 FOR outc NEXT ; ok

\ preset C0FFEE .s  C0FFEE  ok
\ c0ffeesign ok
\ .s  C0FFEE  ok
: c0ffeesign 45 45 46 46 30 43 20 6 FOR outc NEXT ;
\ end.
