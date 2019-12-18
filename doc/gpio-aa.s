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

; LINE 43: mov	r1, #0xF000 	; set PD12-15, turn on LEDs
;          b 1111 0000 0000 0000
;            fedc ba98 7654 3210

; suspect: 0xF000 is a list of port pins, PD15-0 and is bitmapped.
