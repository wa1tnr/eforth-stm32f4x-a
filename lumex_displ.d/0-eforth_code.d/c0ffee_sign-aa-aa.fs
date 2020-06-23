\ : c0ffee-sign
: line_end 0A outc ;
: s_atd1=()
 \ atd1=()
 29 28 3D 31   64 74 61
 7 1 - FOR outc NEXT
 line_end
;

: atef=(1)
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
