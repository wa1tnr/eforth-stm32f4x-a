COLD 

: RCC 40023800 ;
: RCC_AHB1ENR RCC 30 + ;
: GPIOC 40020800 ;

: SETUPLED
  RCC @
  4 OR RCC_AHB1ENR ! ( GPIOCEN )
  4 GPIOC ! 0 GPIOC 14 + ! ;
: LED GPIOC 14 + 2 ;
: ON SWAP ! ;
: OFF DROP 0 SWAP ! ;


0 [IF]
 53 .equ RCC        ,   0x40023800

 55 set_gpio_dir:
 56   ldr r0, = RCC
 57   ldr r1, [r0, #0x30] @ RCC_AHB1ENR
 58   orr r1, #4 @ GPIOCEN #8 for GPIOD, want GPIOC
 59   str r1, [r0, #0x30]

 61   ldr r0, = 0x40020800 @ GPIOC maybe
 62   ldr r1, [r0, #0x00] @ GPIOx_MODER
 63   bic r1, #0x0000000C @ Mask PORT_C_01
 64   orr r1, #0x00000004 @ Output
 65   str r1, [r0, #0x00]
 66   bx lr
[THEN]
