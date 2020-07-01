\ Wed Jul  1 16:26:01 UTC 2020

\ load after all turnkey stuff, for now (1 July 2020)

\ c0ffee-sign derivatives aa

\ : inchar [ CHAR P ] LITERAL ;
: inchar nop ; \ placeholder
: incol  nop ; \ placeholder

: at80 ( col char -- )
 swap
 \ 'at80'
 61 outc 74 outc 38 outc 30 outc
 \ 'at80=('
 3D outc 28 outc
 \ 'at80=0,'
 30 outc 2C outc
 \ 'at80=(0,col,'
 incol  outc 2C outc
 \ 'at80=(0,col,char'
 inchar outc \ 2C outc
 \ 'at80=(0,col,char)\n'
 29 outc line_end s_atd1=() ; \ at80=(0,col,char)

\ sample run
\ emit 'PQR' on Lumex 96x8 display:

\ decimal -99 hex .s  FFFFFF9D
\ [ char 4 ] ; [ char P ] ; at80
\ [ char 5 ] ; [ char Q ] ; at80
\ [ char 6 ] ; [ char R ] ; at80

\ this construct works:
\ [ char 4 ] [ char P ] ; at80 8 delay [ char 5 ] [ char Q ] ; at80
 ( - - - - - )
