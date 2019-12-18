;****************************************************************************
;	STM32eForth version 7.20
;	Chen-Hanson Ting,  July 2014

; .
; .

GPIOB	EQU	0x40020400
GPIOD	EQU	0x40020C00

; .
; .

; USART1 PB6 TX and PB7 RX; this works.

InitDevices
; init Reset Clock Control RCC registers
	ldr	r0, =RCC 		; RCC
	ldr	r1, [r0, #0x30]	; RCC_AHB1ENR
	orr	r1, #0xA		; GPIOBEN+GPIODEN
	str	r1, [r0, #0x30]
	ldr	r1, [r0, #0x44]	; RCC_APB2ENR
	orr	r1, #0x10		; USART1EN (1 << 4)
	str	r1, [r0, #0x44]
; init GPIOB
	ldr	r0, =GPIOB ; GPIOB
	ldr	r1, [r0, #0x00]	; GPIOx_MODER
	orr	r1, #0xA000   	; =AF Mode
	str	r1, [r0, #0x00]
	ldr	r1, [r0, #0x20]	; GPIOx_AFRL
	orr	r1, #0x77000000	; =AF7 USART1
	str	r1, [r0, #0x20]
; init USART1
	ldr	r0, =USART1 	; USART1
	movw	r1, #0x0200C	; enable USART
	strh	r1, [r0, #12]	; +12 USART_CR1 = 0x2000
	movs	r1, #139		; 16MHz/8.6875 (139, 0x8B) == 115200
	strh	r1, [r0, #8]	;  +8 USART_BR
; Configure PD12-15 as output with push-pull
	ldr	r0, =GPIOD 	; GPIOD
	mov	r1, #0x55000000	; output
	str	r1, [r0, #0x00]
	mov	r1, #0xF000 	; set PD12-15, turn on LEDs
	str	r1, [r0, #0x14]
	bx	lr
	ALIGN
	LTORG


; Wed Dec 18 00:27:18 UTC 2019

; LINE 41: mov	r1, #0x55000000	; output
;          b 0101 0101 0000 0000 0000 0000 0000 0000
;            1098 7654
; LINE 43: mov	r1, #0xF000 	; set PD12-15, turn on LEDs
;          b 1111 0000 0000 0000
;            fedc ba98 7654 3210

; suspect: 0xF000 is a list of port pins, PD15-0 and is bitmapped.


; Wed Dec 18 00:27:19 UTC 2019

; Datasheet: Rev 18, Page 281:

; 8.4.1 GPIO port mode register (GPIOx_MODER) (x = A through I)

; Address offset: 0x00

; Reset values:
; 0xA800 0000 for Port A
; 0x0000 0280 for Port B
; 0x0000 0000 for all other ports (want: Port C)
; Bits 2y:2y+1  MODERy[1:0] Port x configuration bits (y= 0..15)
; 00 input
; 01 gen purpose output
; 10 alternate function mode
; 11 analog mode

; So, two bits per port pin here.  That's the 2y:2y+1 business.

; and so, in LINE 41, above, the '55' means:
; 01  01  01  01   at the top end of the 0x55000000
; these mean '01' gen purpose output and they mean it,
; positionally, for the 'top four' places (15..12)
; which matches with expectation.


; detail - decode 0x55000000 in-context:

;    0x55000000

;      7    6    5    4    3    2    1    0
;    0101 0101 0000 0000 0000 0000 0000 0000

;    7b 7a  6b 6a  5b 5a  4b 4a     3b 3a  2b 2a  1b 1a  0b 0a
;    01 01  01 01  00 00  00 00     00 00  00 00  00 00  00 00

; PINS, PORTD:
;    15 14  13 12  11 10  09 08     07 06  05 04  03 02  01 00


; Wed Dec 18 00:58:51 UTC 2019

; Again: (LINES 39-45, from above)

; LINE 39: ; Configure PD12-15 as output with push-pull
; LINE 40: ldr	r0, =GPIOD 	; GPIOD
; LINE 41: mov	r1, #0x55000000	; output
; LINE 42: str	r1, [r0, #0x00]
; LINE 43: mov	r1, #0xF000 	; set PD12-15, turn on LEDs
; LINE 44: str	r1, [r0, #0x14]
; LINE 45: bx	lr


; Wed Dec 18 01:15:12 UTC 2019:

; Line 40 says to load register 0 (r0) with 'GPIOD'?
; Line 41 says to move the constant, 0x55.. into r1
; Line 42 says to store what's in r0 ('GPIOD') into r1, at offset 0x00?
; the offset guess is based on the datasheet mentioning 'offset 0x00' in same context, iirc.
; Line 43 says to move the constant, 0xF000 (decoded earlier as 'positions 15-12 e.g. 0xf = 1111')
; i.e. select the top four port pins of PORTD, for what is to follow ..
; Line 44 says to store what's in r0 ('GPIOD' .. hasn't been changed) into r1, at offset 0x14?
; .. if all guesses were good, so far, and not propagating bad guesswork down the page. ;)

; Line 45 is 'bx lr' which iirc Dr. Ting specifies as how we do things around here.
;
; to wit:
;  26 ;    All low level code words are terminaled by
;  27 ;    BX      LR      (_NEXT)

; btw R1 looks like Forth's Stack Pointer (usual data stack, not its 'return' stack) here.
; TOS will be R5 (not used here).
; R0 Dr. Ting specifies as 'scratch' register
; (and/or the Forth Virtual Machine's so-called 'Instruction Pointer').
; perhaps the designation as 'scratch' just means it's okay to borrow
; it, and restore the Instruction Pointer, afterward, once the VM is
; looping and processing instructions. ;)


; Wed Dec 18 01:23:04 UTC 2019

; LINE 101:
; 101 GPIOD   EQU     0x40020C00

; Line 101 says to equate the machine address 0x40020C00 with 'GPIOD'
; as a symbol to represet that address, in this assembler code/document.

; guess basis is: recognition of that address space as (vaguely) appropriate.
; datasheet can confirm that PORT_D is related to that address in some way. ;)

; So, with that in mind (and, itertively):
; Line 40 says to load register 0 (r0) with the address, 0x40020C00 .. which is GPIOD.  Perhaps!


; Wed Dec 18 01:41:19 UTC 2019

; p. 67 Rev 18 datasheet:

; 2.3 Memory Map

; "Table 1 gives the boundary addresses of the peripherals available IN ALL STM32F4xx devices." (w00t)
; That 'IN ALL' business is going to help here, quite a bit.

; Table 1.  STM32Fxx register boundary addresses (continued)

; 0x4002 0C00 - 0x4002 0FFF   GPIOD  (so there you go!)
; 0x4002 0800 - 0x4002 0BFF   GPIOC  (our target port)


; Wed Dec 18 01:45:49 UTC 2019

; was:
;  99 RCC     EQU     0x40023800
; 100 GPIOB   EQU     0x40020400
; 101 GPIOD   EQU     0x40020C00
; 102 USART1  EQU     0x40011000

; change to:
;  99 RCC     EQU     0x40023800
; 100 GPIOB   EQU     0x40020400
; 101 GPIOC   EQU     0x40020800
; 102 GPIOD   EQU     0x40020C00
; 103 USART1  EQU     0x40011000

; That should provide a new symbol, 'GPIOC' for our present use case.

END.
