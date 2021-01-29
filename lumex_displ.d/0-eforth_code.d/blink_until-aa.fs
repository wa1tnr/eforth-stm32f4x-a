\ blink_until-aa.fs
\ Fri Jan 29 19:29:40 UTC 2021

( VARIABLE tog )

1 NEGATE tog !

: sdelay ( n -- ) \ shortest delay
  DEPTH 1 - 0<
  IF EXIT THEN
  FOR 2 FOR 2 FOR 2
      FOR 1 DROP NEXT
  NEXT NEXT NEXT ;

: toggle_led ( -- ) \ has minimum ON time delay - OFF returns w no delay
  tog @
  IF led on 100 sdelay 0 tog ! EXIT THEN
  1 NEGATE tog !
  led off ;

: blink_until ( -- c ) \ output: one char
  BEGIN
    ?KEY ( c T | F )
    IF EXIT THEN
    toggle_led
    tog @ NEGATE IF 55 delay THEN
  AGAIN ;
\ end.
