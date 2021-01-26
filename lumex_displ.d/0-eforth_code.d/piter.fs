\ iter
\ piter.fs
\ Tue Jan 26 16:11:25 UTC 2021
: itjj ( iter -- ) \ iterate - payload: 'dump'
  2 * \ normalize
  1 -
  -80 \ neg. integer to normalize address
  SWAP
  FOR 80 + DUP
      80 DUMP \ payload
      CR
      9 delay
  NEXT
  DROP
;
: viterjj ( -- ) space ." piter.fs 26 Jan 2021 16:11:25z" cr ;
\ : iter ( iter -- ) itjj ; \ alias
\ : viter ( -- ) viterjj ; \ alias
\ END.
