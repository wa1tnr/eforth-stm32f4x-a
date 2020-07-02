\ Wed Jul  1 16:26:01 UTC 2020

\ load after all turnkey stuff, for now (1 July 2020)

\ c0ffee-sign derivatives aa

: chA [ char A ] LITERAL ;
: chB [ char B ] LITERAL ;
: chC [ char C ] LITERAL ;
: chD [ char D ] LITERAL ;
: chE [ char E ] LITERAL ;
: chF [ char F ] LITERAL ;
: chG [ char G ] LITERAL ;
: chH [ char H ] LITERAL ;
: chI [ char I ] LITERAL ;
: chJ [ char J ] LITERAL ;
: chK [ char K ] LITERAL ;
: chL [ char L ] LITERAL ;
: chM [ char M ] LITERAL ;
: chN [ char N ] LITERAL ;
: chO [ char O ] LITERAL ;
: chP [ char P ] LITERAL ;
: chQ [ char Q ] LITERAL ;
: chR [ char R ] LITERAL ;
: chS [ char S ] LITERAL ;
: chT [ char T ] LITERAL ;
: chU [ char U ] LITERAL ;
: chV [ char V ] LITERAL ;
: chW [ char W ] LITERAL ;
: chX [ char X ] LITERAL ;
: chY [ char Y ] LITERAL ;
: chZ [ char Z ] LITERAL ;
: ch0 [ char 0 ] LITERAL ;
: ch1 [ char 1 ] LITERAL ;
: ch2 [ char 2 ] LITERAL ;
: ch3 [ char 3 ] LITERAL ;
: ch4 [ char 4 ] LITERAL ;
: ch5 [ char 5 ] LITERAL ;
: ch6 [ char 6 ] LITERAL ;
: ch7 [ char 7 ] LITERAL ;
: ch8 [ char 8 ] LITERAL ;
: ch9 [ char 9 ] LITERAL ;

\ : inchar [ CHAR P ] LITERAL ;
\ placeholders:
: inchar nop ;
: incol  nop ;

: _twocol
  incol outc incol outc 2C outc \ two places for column as with 10 decimal
  inchar outc
  29 outc line_end s_atd1=() ;

: at80= ( char col0 col10 -- )
  61 outc 74 outc 38 outc 30 outc \ at80
  3D outc 28 outc \ at80=(
  30 outc 2C outc \ at80=(0,
  _twocol
;
\ sample run
\ clearit ok
\ : dy 8 delay ; ok
\ ch0 chA at80 dy  ch1 chB at80 dy  ch2 chC at80 ok
\ ch3 chD at80 dy  ch4 chE at80 dy  ch5 chF at80 ok
\ ch6 chG at80 dy  ch7 chH at80 dy ok
\ ch8 chI at80 dy  ch9 chJ at80 ok
\ chK ch0 ch1 at80=    dy   chL ch1 ch1 at80=   dy   chM ch2 ch1 at80= ok
\ chN ch3 ch1 at80=  dy  chO ch4 ch1 at80=  dy  chP ch5 ch1 at80= ok

\ display 'ABCDEFGHIJKLMNOP'

: at80 ( col char -- )
 swap
 \ 'at80'                of 'at80=(0,col,char)'
 61 outc 74 outc 38 outc 30 outc
 \     '=('              of 'at80=(0,col,char)'
 3D outc 28 outc
 \       '0,'            of 'at80=(0,col,char)'
 30 outc 2C outc
 \         'col,'        of 'at80=(0,col,char)'
 incol  outc 2C outc
 \             'char'    of 'at80=(0,col,char)'
 inchar outc
 \                 ')\n' of 'at80=(0,col,char)'
 29 outc line_end s_atd1=() ;

\ sample run
\ emit 'PQR' on Lumex 96x8 display:

\ decimal -99 hex .s  FFFFFF9D
\ [ char 4 ] ; [ char P ] ; at80
\ [ char 5 ] ; [ char Q ] ; at80
\ [ char 6 ] ; [ char R ] ; at80

\ this construct works:
\ [ char 4 ] [ char P ] ; at80 8 delay [ char 5 ] [ char Q ] ; at80
 ( - - - - - )