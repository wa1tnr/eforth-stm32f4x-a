\ ." Wed Jun 24 10:22:19 UTC 2020 "
\ scratch pad forth source - no organization
\ some word defs repeated - ignore

\ : c0ffee-sign
: line_end 0A outc ;
: s_atd1=()
 \ atd1=()
 29 28 3D 31   64 74 61
 7 1 - FOR outc NEXT
 line_end
;

\ reference:
\ lumex-64x32_RGB_LED_command_list_V17_dot_E_03152019.pdf

\  1 dark blue
\ 32 dark red
\  4 dark green
\ red: 32 64 96
\ blue: 1  2  3
\ green: 4 8 12
\ cyan:  7 11 15 - and 23
\ magenta: 65 66 67, 97 98 99

: atef=(32) \ dark red
\ )  2  3  (    =  f  e  t    a
 29 32 33 28   3D 66 65 74   61
 outc outc outc outc
 outc outc outc outc       outc
 line_end
 s_atd1=()
;

: atef=(1) \ default color, here '1' is a color range 0-255
\ 1 is dark blue - not very dark, though.
\ )  1  (  =    f  e  t  a
 29 31 28 3D   66 65 74 61
 outc outc outc outc outc outc outc outc
 line_end
 \ 8 1 - FOR outc NEXT
 s_atd1=()
;

: at80=(0,0,4)
 29 34 2C 30   2C 30 28 3D   30 38 74 61
 outc outc outc outc outc outc outc outc
 outc outc outc outc
 line_end
 \ C 1 - FOR outc NEXT
 s_atd1=()
;

: at80=(0,1,0)
 29 30 2C 31   2C 30 28 3D   30 38 74 61
 C 1 - FOR outc NEXT
 line_end
 s_atd1=()
;

: at80=(0,2,C)
 29 43 2C 32   2C 30 28 3D   30 38 74 61
 C 1 - FOR outc NEXT
 line_end
 s_atd1=()
;

: sent 14 delay ;

: said
  atef=(1) sent
  at80=(0,0,4) sent
  at80=(0,1,0) sent
  at80=(0,2,C) sent
\ s_atd1=()
;

 ( - - - - - )
