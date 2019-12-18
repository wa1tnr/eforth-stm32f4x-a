;****************************************************************************
;	STM32eForth version 7.20
;	Chen-Hanson Ting,  July 2014

;	Subroutine Threaded Forth Model
;	Adapted to STM32F407-Discovery Board
;	Assembled by Keil uVision 5.10

;	Version 4.03
;	Direct Threaded Forth Model
;	Derived from 80386 eForth versin 4.02
;	and Chien-ja Wu's ARM7 eForth version 1.01

;	Subroutine thread (Branch-Link) model
;	  Register assignments
;	IP	 	R0 	;scratch
;	SP	 	R1 
;	RP	 	R2 
;	UP	 	R3 
;	WP	 	R4	;scratch 
;	TOS	 	R5 
;	XP	 	R6 	;scratch
;	YP	 	R7	;scratch
;	  All Forth words are called by 
;	BL.W	addr
;	  All low level code words are terminaled by
;	BX	LR 	(_NEXT)
;	  All high level Forth words start with
;	STRFD	RP!,{LR}	(_NEST)
;	  All high level Forth words end with
;	LDRFD	RP!,{PC}	(_UNNEST)
;	  Top of data stack is cached in R5
;	  USART1 at 115200 baud, 8 data bits, 1 stop bit, no parity
;	TX on PB6 and RX on PB7.

;	Version 5.02, 09oct04cht
;	fOR ADuC702x from Analog Devices
;	Version 6.01, 10apr08cht a
;	Align to at91sam7x256
;	Tested on Olimax SAM7-EX256 Board with LCD display
;	Running under uVision3 RealView from Keil
;	Version 7.01, 29jun14cht
;	Ported to STM32F407-Discovery Board, under uVision 5.10
;	Aligned to eForth 2 Model
;	Assembled to flash memory and executed therefrom.
;	Version 7.10, 30jun14cht
;	Flash memory mapped to Page 0 where codes are executed
;	Version 7.20, 02jul14cht
;	Irreducible Complexity
;	Code copied from flash to RAM, RAM mapped to Page 0.
;	TURNKEY saves current application from RAM to flash.

;****************************************************************************
; Minimal boot-up code

	AREA		RESET, CODE, READONLY
	THUMB
	EXPORT	__Vectors		; linker needs it
	EXPORT	Reset_Handler	; linker needs it

; Vector Table has only Reset Vector
__Vectors	DCD	0x10000400		; Top of hardware stack in CCM
	DCD		Reset_Handler	; Reset Handler
 
	ENTRY

Reset_Handler	
	BL	InitDevices	 	; RCC, GPIOs, USART1
	BL	UNLOCK			; unlock flash memory
	BL	REMAP				; remap RAM to page 0
	LDR	R0,=COLD-MAPOFFSET	; start Forth
	BX	R0
	ALIGN
 
;****************************************************************************
; Remap eForth to execute from RAM
;
; Copy eForth from flash to RAM
REMAP	
	mov	r0,#0x8000000
	mov	r1,#0x20000000
	add	r2,r1,#0x10000
REMAP1
	cmp	r1, r2
	ldrcc   r3, [r0], #4
	strcc   r3, [r1], #4
	bcc	REMAP1

; Remap RAM to page 0
	movw	R0,#0x3800		; SYSCFG register
	movt	R0,#0x4001
	mov	R1,#3
	str	R1,[R0,#0]		; map RAM to page 0
	bx	lr
	align

;****************************************************************************
; Here are devices used by eForth
RCC	EQU	0x40023800
GPIOB	EQU	0x40020400
GPIOD	EQU	0x40020C00
USART1	EQU	0x40011000
; Assumes system running from 16 MHz, HSI (Normal at Reset)
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
 
;****************************************************************************
; Version control

VER EQU	0x07	;major release version
EXT EQU	0x20	;minor extension

; Constants

;RAMOFFSET  EQU	0x00000000	;absolute
;MAPOFFSET  EQU	0x00000000	;absolute
RAMOFFSET  EQU	0x20000000	;remap
MAPOFFSET  EQU	0x08000000	;remap

COMPO EQU	0x040	;lexicon compile only 
IMEDD EQU	0x080	;lexicon immediate bit
MASKK EQU	0x0FFFFFF1F	;lexicon bit mask, allowed for Chineze character

CELLL EQU	4	;size of a cell
BASEE EQU	16	;default radix
VOCSS EQU	8	;depth of vocabulary stack

BKSPP EQU	8	;backspace
LF EQU	10	;line feed
CRR EQU	13	;carriage return
ERR EQU	27	;error escape
TIC EQU	39	;tick

;; Memory allocation	0//code>--//--<sp//tib>--rp//user//
;;	0000	;RAM memory mapped to Page 0, Reset vector
;;	0008	;init devices
;;	00C0	;initial system variables
;;	0100	;Forth dictionary
;;	2150	;top of dictionary, HERE
;;	2154	;WORD buffer
;;	FE00	;top of data stack
;;	FE00	;TIB terminal input buffer
;;	FF00	;top of return stack
;;	FF00	;system variables
;;	8000000	;flash, code image
;;	1000400	;top of hardware stack for interrupts
;;	20000000	;RAM

SPP 	EQU	0x2000FE00-RAMOFFSET	;top of data stack (SP0)
TIBB 	EQU	0x2000FE00-RAMOFFSET	;terminal input buffer (TIB)
RPP 	EQU	0x2000FF00-RAMOFFSET	;top of return stack (RP0)
UPP 	EQU	0x2000FF00-RAMOFFSET	;start of user area (UP0)
DTOP 	EQU	0x2000FC00-RAMOFFSET	;start of usable RAM area (HERE)

;**************************************************************************
;	Assemble inline direct threaded code ending.

 	MACRO	
 	_NEXT   		;end low level word
	BX	LR
	MEND

 	MACRO	
 	_NEST   		;start high level word
	STMFD	R2!,{LR}
	MEND

 	MACRO	
 	_UNNEST   		;end high level word
	LDMFD	R2!,{PC}
	MEND

 	MACRO	
 	_DOLIT   		;long literals
	BL	DOLIT
	MEND

 	MACRO	
 	_PUSH   		;push R5 on data stack
	STR	R5,[R1,#-4]!
	MEND

 	MACRO	
 	_POP   		;pop data stack to R5
	LDR	R5,[R1],#4
	MEND

;**************************************************************************
; COLD start moves the following to USER variables.
; MUST BE IN SAME ORDER AS USER VARIABLES.

	ALIGN	64  	; align to page boundary

UZERO
	DCD	0  			;Reserved
	DCD	HI-MAPOFFSET  	;'BOOT
	DCD	BASEE  		;BASE
	DCD	0			;tmp
	DCD	0			;SPAN
	DCD	0			;>IN
	DCD	0			;#TIB
	DCD	TIBB			;TIB
	DCD	INTER-MAPOFFSET	;'EVAL
	DCD	0			;HLD
	DCD	LASTN-MAPOFFSET	;CONTEXT
	DCD	CTOP-MAPOFFSET	;FLASH
	DCD	CTOP-MAPOFFSET	;RAM
	DCD	LASTN-MAPOFFSET	;LAST
	DCD	0,0			;reserved
ULAST
	ALIGN	
	
;**********************************************************************
; Start of Forth dictionary
; usart1

;   ?RX	 ( -- c T | F )
;	Return input character and true, or a false if no input.
	DCD	0
_QRX	DCB   4
	DCB	"?KEY"
	ALIGN
QKEY
QRX	
	_PUSH
	ldr	r4, =0x40011000	; USART1 F2/F4
	ldrh	r6, [r4, #0]	; USART->SR
	ands	r6, #0x20		; RXE
	BEQ	QRX1
 	LDR	R5, [R4, #4]
	_PUSH
	MVNNE	R5,#0
QRX1
	MOVEQ	R5,#0
	_NEXT

;   TX!	 ( c -- )
;	Send character c to the output device.

	DCD	_QRX-MAPOFFSET
_TXSTO	DCB   4
	DCB	"EMIT"
	ALIGN	
TXSTO
EMIT
TECHO
	ldr	r4, =0x40011000	; USART1 F2/F4
TX1	ldrh	r6, [r4, #0]	; USART->SR
	ands	r6, #0x80		; TXE
	beq	TX1
	strh	r5, [r4, #4]	; USART->DR
	_POP
	_NEXT

	ALIGN
	LTORG
	
;**************************************************************************
; The kernel

;   NOP	( -- w )
;	Push an inline literal.

	DCD	_TXSTO-MAPOFFSET
_NOP	DCB   3
	DCB	"NOP"
	ALIGN	
NOP
	_NEXT
	ALIGN

;   doLIT	( -- w )
;	Push an inline literal.

;	DCD	_NOP-MAPOFFSET
;_LIT	DCB   COMPO+5
;	DCB	"doLIT"
;	ALIGN	
DOLIT
	_PUSH
	BIC	LR,LR,#1		; clear b0 in LR
	LDR	R5,[LR],#4		; get literal at word boundary
	ORR	LR,LR,#1		; aet b0 in LR
	_NEXT
	ALIGN

;   EXECUTE	( ca -- )
;	Execute the word at ca.

	DCD	_NOP-MAPOFFSET
_EXECU	DCB   7
	DCB	"EXECUTE"
	ALIGN	
EXECU
	ORR	R4,R5,#1		; b0=1
	_POP
	BX	R4
	ALIGN

;   next	( -- )
;	Run time code for the single index loop.
;	: next ( -- ) \ hilevel model
;	 r> r> dup if 1 - >r @ >r exit then drop cell+ >r ;

;	DCD	_EXECU-MAPOFFSET
;_DONXT	DCB   COMPO+4
;	DCB	"next"
;	ALIGN	
DONXT
	LDR	R4,[R2]
	MOVS	R4,R4
	BNE	NEXT1
	ADD	R2,R2,#4
	ADD	LR,LR,#4
	_NEXT
NEXT1	SUB	R4,R4,#1
	STR	R4,[R2]
	LDR	LR,[LR,#-1]	; handle b0 in LR 
	ORR	LR,LR,#1
	_NEXT

;   ?branch	( f -- )
;	Branch if flag is zero.

;	DCD	_DONXT-MAPOFFSET
;_QBRAN	DCB   COMPO+7
;	DCB	"?branch"
;	ALIGN	
QBRAN
	MOVS	R4,R5
	_POP
	BNE	QBRAN1
	LDR	LR,[LR,#-1]
	ORR LR,LR,#1
	_NEXT
QBRAN1	ADD	LR,LR,#4
	_NEXT

;   branch	( -- )
;	Branch to an inline address.

;	DCD	_QBRAN-MAPOFFSET
;_BRAN	DCB   COMPO+6
;	DCB	"branch"
;	ALIGN	
BRAN
	LDR	LR,[LR,#-1]
	ORR	LR,LR,#1
	_NEXT
	ALIGN

;   EXIT	(  -- )
;	Exit the currently executing command.

	DCD	_EXECU-MAPOFFSET
_EXIT	DCB   4
	DCB	"EXIT"
	ALIGN	
EXIT
	_UNNEST

;   !	   ( w a -- )
;	Pop the data stack to memory.

	DCD	_EXIT-MAPOFFSET
_STORE	DCB   1
	DCB	"!"
	ALIGN	
STORE
	LDR	R4,[R1],#4
	STR	R4,[R5]
	_POP
	_NEXT

;   @	   ( a -- w )
;	Push memory location to the data stack.

	DCD	_STORE-MAPOFFSET
_AT	DCB   1
	DCB	"@"
	ALIGN	
AT
	LDR	R5,[R5]
	_NEXT

;   C!	  ( c b -- )
;	Pop the data stack to byte memory.

	DCD	_AT-MAPOFFSET
_CSTOR	DCB   2
	DCB	"C!"
	ALIGN	
CSTOR
	LDR	R4,[R1],#4
	STRB	R4,[R5]
	_POP
	_NEXT

;   C@	  ( b -- c )
;	Push byte memory location to the data stack.

	DCD	_CSTOR-MAPOFFSET
_CAT	DCB   2
	DCB	"C@"
	ALIGN	
CAT
	LDRB	R5,[R5]
	_NEXT

;   R>	  ( -- w )
;	Pop the return stack to the data stack.

	DCD	_CAT-MAPOFFSET
_RFROM	DCB   2
	DCB	"R>"
	ALIGN	
RFROM
	_PUSH
	LDR	R5,[R2],#4
	_NEXT
	ALIGN

;   R@	  ( -- w )
;	Copy top of return stack to the data stack.

	DCD	_RFROM-MAPOFFSET
_RAT	DCB   2
	DCB	"R@"
	ALIGN	
RAT
	_PUSH
	LDR	R5,[R2]
	_NEXT

;   >R	  ( w -- )
;	Push the data stack to the return stack.

	DCD	_RAT-MAPOFFSET
_TOR	DCB   COMPO+2
	DCB	">R"
	ALIGN	
TOR
	STR	R5,[R2,#-4]!
	_POP
	_NEXT
	ALIGN

;   SP@	 ( -- a )
;	Push the current data stack pointer.

	DCD	_TOR-MAPOFFSET
_SPAT	DCB   3
	DCB	"SP@"
	ALIGN	
SPAT
	_PUSH
	MOV	R5,R1
	_NEXT

;   DROP	( w -- )
;	Discard top stack item.

	DCD	_SPAT-MAPOFFSET
_DROP	DCB   4
	DCB	"DROP"
	ALIGN	
DROP
	_POP
	_NEXT
	ALIGN

;   DUP	 ( w -- w w )
;	Duplicate the top stack item.

	DCD	_DROP-MAPOFFSET
_DUPP	DCB   3
	DCB	"DUP"
	ALIGN	
DUPP
	_PUSH
	_NEXT
	ALIGN

;   SWAP	( w1 w2 -- w2 w1 )
;	Exchange top two stack items.

	DCD	_DUPP-MAPOFFSET
_SWAP	DCB   4
	DCB	"SWAP"
	ALIGN	
SWAP
	LDR	R4,[R1]
	STR	R5,[R1]
	MOV	R5,R4
	_NEXT

;   OVER	( w1 w2 -- w1 w2 w1 )
;	Copy second stack item to top.

	DCD	_SWAP-MAPOFFSET
_OVER	DCB   4
	DCB	"OVER"
	ALIGN	
OVER
	_PUSH
	LDR	R5,[R1,#4]
	_NEXT

;   0<	  ( n -- t )
;	Return true if n is negative.

	DCD	_OVER-MAPOFFSET
_ZLESS	DCB   2
	DCB	"0<"
	ALIGN	
ZLESS
	MOV	R4,#0
	ADD	R5,R4,R5,ASR #32
	_NEXT
	ALIGN

;   AND	 ( w w -- w )
;	Bitwise AND.

	DCD	_ZLESS-MAPOFFSET
_ANDD	DCB   3
	DCB	"AND"
	ALIGN	
ANDD
	LDR	R4,[R1],#4
	AND	R5,R5,R4
	_NEXT
	ALIGN

;   OR	  ( w w -- w )
;	Bitwise inclusive OR.

	DCD	_ANDD-MAPOFFSET
_ORR	DCB   2
	DCB	"OR"
	ALIGN	
ORR
	LDR	R4,[R1],#4
	ORR	R5,R5,R4
	_NEXT
	ALIGN

;   XOR	 ( w w -- w )
;	Bitwise exclusive OR.

	DCD	_ORR-MAPOFFSET
_XORR	DCB   3
	DCB	"XOR"
	ALIGN	
XORR
	LDR	R4,[R1],#4
	EOR	R5,R5,R4
	_NEXT
	ALIGN

;   UM+	 ( w w -- w cy )
;	Add two numbers, return the sum and carry flag.

	DCD	_XORR-MAPOFFSET
_UPLUS	DCB   3
	DCB	"UM+"
	ALIGN	
UPLUS
	LDR	R4,[R1]
	ADDS	R4,R4,R5
	MOV	R5,#0
	ADC	R5,R5,#0
	STR	R4,[R1]
	_NEXT

;   RSHIFT	 ( w # -- w )
;	Right shift # bits.

	DCD	_UPLUS-MAPOFFSET
_RSHIFT	DCB   6
	DCB	"RSHIFT"
	ALIGN	
RSHIFT
	LDR	R4,[R1],#4
	MOV	R5,R4,ASR R5
	_NEXT
	ALIGN

;   LSHIFT	 ( w # -- w )
;	Right shift # bits.

	DCD	_RSHIFT-MAPOFFSET
_LSHIFT	DCB   6
	DCB	"LSHIFT"
	ALIGN	
LSHIFT
	LDR	R4,[R1],#4
	MOV	R5,R4,LSL R5
	_NEXT
	ALIGN

;   +	 ( w w -- w )
;	Add.

	DCD	_LSHIFT-MAPOFFSET
_PLUS	DCB   1
	DCB	"+"
	ALIGN	
PLUS
	LDR	R4,[R1],#4
	ADD	R5,R5,R4
	_NEXT

;   -	 ( w w -- w )
;	Subtract.

	DCD	_PLUS-MAPOFFSET
_SUBB	DCB   1
	DCB	"-"
	ALIGN	
SUBB
	LDR	R4,[R1],#4
	RSB	R5,R5,R4
	_NEXT
	ALIGN

;   *	 ( w w -- w )
;	Multiply.

	DCD	_SUBB-MAPOFFSET
_STAR	DCB   1
	DCB	"*"
	ALIGN	
STAR
	LDR	R4,[R1],#4
	MUL	R5,R4,R5
	_NEXT
	ALIGN

;   UM*	 ( w w -- ud )
;	Unsigned multiply.

	DCD	_STAR-MAPOFFSET
_UMSTA	DCB   3
	DCB	"UM*"
	ALIGN	
UMSTA
	LDR	R4,[R1]
	UMULL	R6,R7,R5,R4
	STR	R6,[R1]
	MOV	R5,R7
	_NEXT

;   M*	 ( w w -- d )
;	Unsigned multiply.

	DCD	_UMSTA-MAPOFFSET
_MSTAR	DCB   2
	DCB	"M*"
	ALIGN	
MSTAR
	LDR	R4,[R1]
	SMULL	R6,R7,R5,R4
	STR	R6,[R1]
	MOV	R5,R7
	_NEXT

;   1+	 ( w -- w+1 )
;	Add 1.

	DCD	_MSTAR-MAPOFFSET
_ONEP	DCB   2
	DCB	"1+"
	ALIGN	
ONEP
	ADD	R5,R5,#1
	_NEXT
	ALIGN

;   1-	 ( w -- w-1 )
;	Subtract 1.

	DCD	_ONEP-MAPOFFSET
_ONEM	DCB   2
	DCB	"1-"
	ALIGN	
ONEM
	SUB	R5,R5,#1
	_NEXT
	ALIGN

;   2+	 ( w -- w+2 )
;	Add 1.

	DCD	_ONEM-MAPOFFSET
_TWOP	DCB   2
	DCB	"2+"
	ALIGN	
TWOP
	ADD	R5,R5,#2
	_NEXT
	ALIGN

;   2-	 ( w -- w-2 )
;	Subtract 2.

	DCD	_TWOP-MAPOFFSET
_TWOM	DCB   2
	DCB	"2-"
	ALIGN	
TWOM
	SUB	R5,R5,#2
	_NEXT
	ALIGN

;   CELL+	( w -- w+4 )
;	Add 4.

	DCD	_TWOM-MAPOFFSET
_CELLP	DCB   5
	DCB	"CELL+"
	ALIGN	
CELLP
	ADD	R5,R5,#4
	_NEXT
	ALIGN

;   CELL-	( w -- w-4 )
;	Subtract 4.

	DCD	_CELLP-MAPOFFSET
_CELLM	DCB   5
	DCB	"CELL-"
	ALIGN	
CELLM
	SUB	R5,R5,#4
	_NEXT
	ALIGN

;   BL	( -- 32 )
;	Blank (ASCII space).

	DCD	_CELLM-MAPOFFSET
_BLANK	DCB   2
	DCB	"BL"
	ALIGN	
BLANK
	_PUSH
	MOV	R5,#32
	_NEXT
	ALIGN

;   CELLS	( w -- w*4 )
;	Multiply 4.

	DCD	_BLANK-MAPOFFSET
_CELLS	DCB   5
	DCB	"CELLS"
	ALIGN	
CELLS
	MOV	R5,R5,LSL#2
	_NEXT
	ALIGN

;   CELL/	( w -- w*4 )
;	Divide by 4.

	DCD	_CELLS-MAPOFFSET
_CELLSL	DCB   5
	DCB	"CELL/"
	ALIGN	
CELLSL
	MOV	R5,R5,ASR#2
	_NEXT
	ALIGN

;   2*	( w -- w*2 )
;	Multiply 2.

	DCD	_CELLSL-MAPOFFSET
_TWOST	DCB   2
	DCB	"2*"
	ALIGN	
TWOST
	MOV	R5,R5,LSL#1
	_NEXT
	ALIGN

;   2/	( w -- w/2 )
;	Divide by 2.

	DCD	_TWOST-MAPOFFSET
_TWOSL	DCB   2
	DCB	"2/"
	ALIGN	
TWOSL
	MOV	R5,R5,ASR#1
	_NEXT
	ALIGN

;   ?DUP	( w -- w w | 0 )
;	Conditional duplicate.

	DCD	_TWOSL-MAPOFFSET
_QDUP	DCB   4
	DCB	"?DUP"
	ALIGN	
QDUP
	MOVS	R4,R5
	STRNE	R5,[R1,#-4]!
	_NEXT
	ALIGN

;   ROT	( w1 w2 w3 -- w2 w3 w1 )
;	Rotate top 3 items.

	DCD	_QDUP-MAPOFFSET
_ROT	DCB   3
	DCB	"ROT"
	ALIGN	
ROT
	LDR	R4,[R1]
	STR	R5,[R1]
	LDR	R5,[R1,#4]
	STR	R4,[R1,#4]
	_NEXT
	ALIGN

;   2DROP	( w1 w2 -- )
;	Drop top 2 items.

	DCD	_ROT-MAPOFFSET
_DDROP	DCB   5
	DCB	"2DROP"
	ALIGN	
DDROP
	_POP
	_POP
	_NEXT
	ALIGN

;   2DUP	( w1 w2 -- w1 w2 w1 w2 )
;	Duplicate top 2 items.

	DCD	_DDROP-MAPOFFSET
_DDUP	DCB   4
	DCB	"2DUP"
	ALIGN	
DDUP
	LDR	R4,[R1]
	STR	R5,[R1,#-4]!
	STR	R4,[R1,#-4]!
	_NEXT

;   D+	( d1 d2 -- d3 )
;	Add top 2 double numbers.

	DCD	_DDUP-MAPOFFSET
_DPLUS	DCB   2
	DCB	"D+"
	ALIGN	
DPLUS
	LDR	R4,[R1],#4
	LDR	R6,[R1],#4
	LDR	R7,[R1]
	ADDS	R4,R4,R7
	STR	R4,[R1]
	ADC	R5,R5,R6
	_NEXT

;   NOT	 ( w -- !w )
;	1"s complement.

	DCD	_DPLUS-MAPOFFSET
_INVER	DCB   3
	DCB	"NOT"
	ALIGN	
INVER
	MVN	R5,R5
	_NEXT
	ALIGN

;   NEGATE	( w -- -w )
;	2's complement.

	DCD	_INVER-MAPOFFSET
_NEGAT	DCB   6
	DCB	"NEGATE"
	ALIGN	
NEGAT
	RSB	R5,R5,#0
	_NEXT
	ALIGN

;   ABS	 ( w -- |w| )
;	Absolute.

	DCD	_NEGAT-MAPOFFSET
_ABSS	DCB   3
	DCB	"ABS"
	ALIGN	
ABSS
	TST	R5,#0x80000000
	RSBNE   R5,R5,#0
	_NEXT
	ALIGN

;   =	 ( w w -- t )
;	Equal?

	DCD	_ABSS-MAPOFFSET
_EQUAL	DCB   1
	DCB	"="
	ALIGN	
EQUAL
	LDR	R4,[R1],#4
	CMPS	R5,R4
	MVNEQ	R5,#0
	MOVNE	R5,#0
	_NEXT

;   U<	 ( w w -- t )
;	Unsigned equal?

	DCD	_EQUAL-MAPOFFSET
_ULESS	DCB   2
	DCB	"U<"
	ALIGN	
ULESS
	LDR	R4,[R1],#4
	CMPS	R4,R5
	MVNCC	R5,#0
	MOVCS	R5,#0
	_NEXT

;   <	( w w -- t )
;	Less?

	DCD	_ULESS-MAPOFFSET
_LESS	DCB   1
	DCB	"<"
	ALIGN	
LESS
	LDR	R4,[R1],#4
	CMPS	R4,R5
	MVNLT	R5,#0
	MOVGE	R5,#0
	_NEXT

;   >	( w w -- t )
;	greater?

	DCD	_LESS-MAPOFFSET
_GREAT	DCB   1
	DCB	">"
	ALIGN	
GREAT
	LDR	R4,[R1],#4
	CMPS	R4,R5
	MVNGT	R5,#0
	MOVLE	R5,#0
	_NEXT

;   MAX	 ( w w -- max )
;	Leave maximum.

	DCD	_GREAT-MAPOFFSET
_MAX	DCB   3
	DCB	"MAX"
	ALIGN	
MAX
	LDR	R4,[R1],#4
	CMPS	R4,R5
	MOVGT	R5,R4
	_NEXT

;   MIN	 ( w w -- min )
;	Leave minimum.

	DCD	_MAX-MAPOFFSET
_MIN	DCB   3
	DCB	"MIN"
	ALIGN	
MIN
	LDR	R4,[R1],#4
	CMPS	R4,R5
	MOVLT	R5,R4
	_NEXT

;   +!	 ( w a -- )
;	Add to memory.

	DCD	_MIN-MAPOFFSET
_PSTOR	DCB   2
	DCB	"+!"
	ALIGN	
PSTOR
	LDR	R4,[R1],#4
	LDR	R6,[R5]
	ADD	R6,R6,R4
	STR	R6,[R5]
	_POP
	_NEXT

;   2!	 ( d a -- )
;	Store double number.

	DCD	_PSTOR-MAPOFFSET
_DSTOR	DCB   2
	DCB	"2!"
	ALIGN	
DSTOR
	LDR	R4,[R1],#4
	LDR	R6,[R1],#4
	STR	R4,[R5],#4
	STR	R6,[R5]
	_POP
	_NEXT

;   2@	 ( a -- d )
;	Fetch double number.

	DCD	_DSTOR-MAPOFFSET
_DAT	DCB   2
	DCB	"2@"
	ALIGN	
DAT
	LDR	R4,[R5,#4]
	STR	R4,[R1,#-4]!
	LDR	R5,[R5]
	_NEXT
	ALIGN

;   COUNT	( b -- b+1 c )
;	Fetch length of string.

	DCD	_DAT-MAPOFFSET
_COUNT	DCB   5
	DCB	"COUNT"
	ALIGN	
COUNT
	LDRB	R4,[R5],#1
	_PUSH
	MOV	R5,R4
	_NEXT

;   DNEGATE	( d -- -d )
;	Negate double number.

	DCD	_COUNT-MAPOFFSET
_DNEGA	DCB   7
	DCB	"DNEGATE"
	ALIGN	
DNEGA
	LDR	R4,[R1]
	SUB	R8,R8,R8
	SUBS	R4,R6,R4
	SBC	R5,R6,R5
	STR	R4,[R1]
	_NEXT

;**************************************************************************
; System and user variables

;   doVAR	( -- a )
;	Run time routine for VARIABLE and CREATE.

;	DCD	_DNEGA-MAPOFFSET
;_DOVAR	DCB  COMPO+5
;	DCB	"doVAR"
;	ALIGN	
DOVAR
	_PUSH
	SUB	R5,LR,#1		; CLEAR B0
	_UNNEST
	ALIGN

;   doCON	( -- a ) 
;	Run time r outine for CONSTANT.

;	DCD	_DOVAR-MAPOFFSET
;_DOCON	DCB  COMPO+5
;	DCB	"doCON"
;	ALIGN	
DOCON
	_PUSH
	LDR	R5,[LR,#-1]	; clear b0
	_UNNEST

;   'BOOT	 ( -- a )
;	Applicarion.

	DCD	_DNEGA-MAPOFFSET
_TBOOT	DCB   5
	DCB	"'BOOT"
	ALIGN	
TBOOT
	_PUSH
	ADD	R5,R3,#4
	_NEXT
	ALIGN
	
;   BASE	( -- a )
;	Storage of the radix base for numeric I/O.

	DCD	_TBOOT-MAPOFFSET
_BASE	DCB   4
	DCB	"BASE"
	ALIGN	
BASE
	_PUSH
	ADD	R5,R3,#8
	_NEXT
	ALIGN

;   tmp	 ( -- a )
;	A temporary storage location used in parse and find.

;	DCD	_BASE-MAPOFFSET
;_TEMP	DCB   COMPO+3
;	DCB	"tmp"
;	ALIGN	
TEMP
	_PUSH
	ADD	R5,R3,#12
	_NEXT
	ALIGN

;   SPAN	( -- a )
;	Hold character count received by EXPECT.

	DCD	_BASE-MAPOFFSET
_SPAN	DCB   4
	DCB	"SPAN"
	ALIGN	
SPAN
	_PUSH
	ADD	R5,R3,#16
	_NEXT
	ALIGN

;   >IN	 ( -- a )
;	Hold the character pointer while parsing input stream.

	DCD	_SPAN-MAPOFFSET
_INN	DCB   3
	DCB	">IN"
	ALIGN	
INN
	_PUSH
	ADD	R5,R3,#20
	_NEXT
	ALIGN

;   #TIB	( -- a )
;	Hold the current count and address of the terminal input buffer.

	DCD	_INN-MAPOFFSET
_NTIB	DCB   4
	DCB	"#TIB"
	ALIGN	
NTIB
	_PUSH
	ADD	R5,R3,#24
	_NEXT
	ALIGN

;   'EVAL	( -- a )
;	Execution vector of EVAL.

	DCD	_NTIB-MAPOFFSET
_TEVAL	DCB   5
	DCB	"'EVAL"
	ALIGN	
TEVAL
	_PUSH
	ADD	R5,R3,#32
	_NEXT
	ALIGN

;   HLD	 ( -- a )
;	Hold a pointer in building a numeric output string.

	DCD	_TEVAL-MAPOFFSET
_HLD	DCB   3
	DCB	"HLD"
	ALIGN	
HLD
	_PUSH
	ADD	R5,R3,#36
	_NEXT
	ALIGN

;   CONTEXT	( -- a )
;	A area to specify vocabulary search order.

	DCD	_HLD-MAPOFFSET
_CNTXT	DCB   7
	DCB	"CONTEXT"
	ALIGN	
CNTXT
CRRNT
	_PUSH
	ADD	R5,R3,#40
	_NEXT
	ALIGN

;   CP	( -- a )
;	Point to top name in vocabulary.

	DCD	_CNTXT-MAPOFFSET
_CP	DCB   2
	DCB	"CP"
	ALIGN	
CPP
	_PUSH
	ADD	R5,R3,#44
	_NEXT
	ALIGN

;   LAST	( -- a )
;	Point to the last name in the name dictionary.

	DCD	_CP-MAPOFFSET
_LAST	DCB   4
	DCB	"LAST"
	ALIGN	
LAST
	_PUSH
	ADD	R5,R3,#52
	_NEXT
	ALIGN

;**************************************************************************
; Common functions

;   WITHIN	( u ul uh -- t )
;	Return true if u is within the range of ul and uh.

	DCD	_LAST-MAPOFFSET
_WITHI	DCB   6
	DCB	"WITHIN"
	ALIGN	
WITHI
	_NEST
	BL	OVER
	BL	SUBB
	BL	TOR
	BL	SUBB
	BL	RFROM
	BL	ULESS
	_UNNEST

; Divide

;   UM/MOD	( udl udh u -- ur uq )
;	Unsigned divide of a double by a single. Return mod and quotient.

	DCD	_WITHI-MAPOFFSET
_UMMOD	DCB   6
	DCB	"UM/MOD"
	ALIGN	
UMMOD
	MOV	R7,#1
	LDR	R4,[R1],#4
	LDR	R6,[R1]
UMMOD0	ADDS	R6,R6,R6
	ADCS	R4,R4,R4
	BCC	UMMOD1
	SUB	R4,R4,R5
	ADD	R6,R6,#1
	B UMMOD2
UMMOD1	SUBS	R4,R4,R5
	ADDCS	R6,R6,#1
	BCS	UMMOD2
	ADD	R4,R4,R5
UMMOD2	ADDS	R7,R7,R7
	BCC	UMMOD0
	MOV	R5,R6
	STR	R4,[R1]
	_NEXT
	ALIGN

;   M/MOD	( d n -- r q )
;	Signed floored divide of double by single. Return mod and quotient.

	DCD	_UMMOD-MAPOFFSET
_MSMOD	DCB  5
	DCB	"M/MOD"
	ALIGN	
MSMOD	
	_NEST
	BL	DUPP
	BL	ZLESS
	BL	DUPP
	BL	TOR
	BL	QBRAN
	DCD	MMOD1-MAPOFFSET
	BL	NEGAT
	BL	TOR
	BL	DNEGA
	BL	RFROM
MMOD1	  BL	TOR
	BL	DUPP
	BL	ZLESS
	BL	QBRAN
	DCD	MMOD2-MAPOFFSET
	BL	RAT
	BL	PLUS
MMOD2	  BL	RFROM
	BL	UMMOD
	BL	RFROM
	BL	QBRAN
	DCD	MMOD3-MAPOFFSET
	BL	SWAP
	BL	NEGAT
	BL	SWAP
MMOD3   
	_UNNEST

;   /MOD	( n n -- r q )
;	Signed divide. Return mod and quotient.

	DCD	_MSMOD-MAPOFFSET
_SLMOD	DCB   4
	DCB	"/MOD"
	ALIGN	
SLMOD
	_NEST
	BL	OVER
	BL	ZLESS
	BL	SWAP
	BL	MSMOD
	_UNNEST

;   MOD	 ( n n -- r )
;	Signed divide. Return mod only.

	DCD	_SLMOD-MAPOFFSET
_MODD	DCB  3
	DCB	"MOD"
	ALIGN	
MODD
	_NEST
	BL	SLMOD
	BL	DROP
	_UNNEST

;   /	   ( n n -- q )
;	Signed divide. Return quotient only.

	DCD	_MODD-MAPOFFSET
_SLASH	DCB  1
	DCB	"/"
	ALIGN	
SLASH
	_NEST
	BL	SLMOD
	BL	SWAP
	BL	DROP
	_UNNEST

;   */MOD	( n1 n2 n3 -- r q )
;	Multiply n1 and n2, then divide by n3. Return mod and quotient.

	DCD	_SLASH-MAPOFFSET
_SSMOD	DCB  5
	DCB	"*/MOD"
	ALIGN	
SSMOD
	_NEST
	BL	TOR
	BL	MSTAR
	BL	RFROM
	BL	MSMOD
	_UNNEST

;   */	  ( n1 n2 n3 -- q )
;	Multiply n1 by n2, then divide by n3. Return quotient only.

	DCD	_SSMOD-MAPOFFSET
_STASL	DCB  2
	DCB	"*/"
	ALIGN	
STASL
	_NEST
	BL	SSMOD
	BL	SWAP
	BL	DROP
	_UNNEST

;**************************************************************************
; Miscellaneous

;   ALIGNED	( b -- a )
;	Align address to the cell boundary.

	DCD	_STASL-MAPOFFSET
_ALGND	DCB   7
	DCB	"ALIGNED"
	ALIGN	
ALGND
	ADD	R5,R5,#3
	MVN	R4,#3
	AND	R5,R5,R4
	_NEXT
	ALIGN

;   >CHAR	( c -- c )
;	Filter non-printing characters.

	DCD	_ALGND-MAPOFFSET
_TCHAR	DCB  5
	DCB	">CHAR"
	ALIGN	
TCHAR
	_NEST
	_DOLIT
	DCD	0x7F
	BL	ANDD
	BL	DUPP	;mask msb
	BL	BLANK
	_DOLIT
	DCD	127
	BL	WITHI	;check for printable
	BL	INVER
	BL	QBRAN
	DCD	TCHA1-MAPOFFSET
	BL	DROP
	_DOLIT
	DCD	'_'	;replace non-printables
TCHA1
	  _UNNEST

;   DEPTH	( -- n )
;	Return the depth of the data stack.

	DCD	_TCHAR-MAPOFFSET
_DEPTH	DCB  5
	DCB	"DEPTH"
	ALIGN	
DEPTH
	_PUSH
	MOVW	R5,#0XFE00
;	MOVT	R5,#0X2000
	SUB	R5,R5,R1
	ASR	R5,R5,#2
	SUB	R5,R5,#1
	_NEXT
	ALIGN

;   PICK	( ... +n -- ... w )
;	Copy the nth stack item to tos.

	DCD	_DEPTH-MAPOFFSET
_PICK	DCB  4
	DCB	"PICK"
	ALIGN	
PICK
	_NEST
	BL	ONEP
	BL	CELLS
	BL	SPAT
	BL	PLUS
	BL	AT
	_UNNEST

;**************************************************************************
; Memory access

;   HERE	( -- a )
;	Return the top of the code dictionary.

	DCD	_PICK-MAPOFFSET
_HERE	DCB  4
	DCB	"HERE"
	ALIGN	
HERE
	_NEST
	BL	CPP
	BL	AT
	_UNNEST

;   PAD	 ( -- a )
;	Return the address of a temporary buffer.

	DCD	_HERE-MAPOFFSET
_PAD	DCB  3
	DCB	"PAD"
	ALIGN	
PAD
	_NEST
	BL	HERE
	ADD	R5,R5,#80
	_UNNEST

;   TIB	 ( -- a )
;	Return the address of the terminal input buffer.

	DCD	_PAD-MAPOFFSET
_TIB	DCB  3
	DCB	"TIB"
	ALIGN	
TIB
	_PUSH
	MOVW	R5,#0xFE00
	_NEXT
	ALIGN

;   @EXECUTE	( a -- )
;	Execute vector stored in address a.

	DCD	_TIB-MAPOFFSET
_ATEXE	DCB   8
	DCB	"@EXECUTE"
	ALIGN	
ATEXE
	MOVS	R4,R5
	_POP
	LDR	R4,[R4]
	ORR	R4,R4,#1
	BXNE	R4
	_NEXT
	ALIGN

;   CMOVE	( b1 b2 u -- )
;	Copy u bytes from b1 to b2.

	DCD	_ATEXE-MAPOFFSET
_CMOVE	DCB   5
	DCB	"CMOVE"
	ALIGN	
CMOVE
	LDR	R6,[R1],#4
	LDR	R7,[R1],#4
	B CMOV1
CMOV0	LDRB	R4,[R7],#1
	STRB	R4,[R6],#1
CMOV1	MOVS	R5,R5
	BEQ	CMOV2
	SUB	R5,R5,#1
	B CMOV0
CMOV2
	_POP
	_NEXT
	ALIGN

;   MOVE	( a1 a2 u -- )
;	Copy u words from a1 to a2.

	DCD	_CMOVE-MAPOFFSET
_MOVE	DCB   4
	DCB	"MOVE"
	ALIGN	
MOVE	AND	R5,R5,#-4
	LDR	R6,[R1],#4
	LDR	R7,[R1],#4
	B MOVE1
MOVE0	LDR	R4,[R7],#4
	STR	R4,[R6],#4
MOVE1	MOVS	R5,R5
	BEQ	MOVE2
	SUB	R5,R5,#4
	B MOVE0
MOVE2
	_POP
	_NEXT
	ALIGN

;   FILL	( b u c -- )
;	Fill u bytes of character c to area beginning at b.

	DCD	_MOVE-MAPOFFSET
_FILL	DCB   4
	DCB	"FILL"
	ALIGN	
FILL
	LDR	R6,[R1],#4
	LDR	R7,[R1],#4
FILL0	B FILL1
	MOV	R5,R5
FILL1	STRB	R5,[R7],#1
	MOVS	R6,R6
	BEQ	FILL2
	SUB	R6,R6,#1
	B FILL0
FILL2
	_POP
	_NEXT

;   PACK$	( b u a -- a )
;	Build a countedDCB with u characters from b. Null fill.

	DCD	_FILL-MAPOFFSET
_PACKS	DCB  5
	DCB	"PACK$$"
	ALIGN	
PACKS
	_NEST
	BL	ALGND
	BL	DUPP
	BL	TOR			;strings only on cell boundary
	BL	OVER
	BL	PLUS
	BL	ONEP
	_DOLIT
	DCD	0xFFFFFFFC
	BL	ANDD			;count mod cell
	_DOLIT
	DCD	0
	BL	SWAP
	BL	STORE			;null fill cell
	BL	RAT
	BL	DDUP
	BL	CSTOR
	BL	ONEP			;save count
	BL	SWAP
	BL	CMOVE
	BL	RFROM
	_UNNEST   			;move string

;**************************************************************************
; Numeric output, single precision

;   DIGIT	( u -- c )
;	Convert digit u to a character.

	DCD	_PACKS-MAPOFFSET
_DIGIT	DCB  5
	DCB	"DIGIT"
	ALIGN	
DIGIT
	_NEST
	_DOLIT
	DCD	9
	BL	OVER
	BL	LESS
	AND	R5,R5,#7
	BL	PLUS
	ADD	R5,R5,#'0'
	_UNNEST

;   EXTRACT	( n base -- n c )
;	Extract the least significant digit from n.

	DCD	_DIGIT-MAPOFFSET
_EXTRC	DCB  7
	DCB	"EXTRACT"
	ALIGN	
EXTRC
	_NEST
	_DOLIT
	DCD	0
	BL	SWAP
	BL	UMMOD
	BL	SWAP
	BL	DIGIT
	_UNNEST

;   <#	  ( -- )
;	Initiate the numeric output process.

	DCD	_EXTRC-MAPOFFSET
_BDIGS	DCB  2
	DCB	"<#"
	ALIGN	
BDIGS
	_NEST
	BL	PAD
	BL	HLD
	BL	STORE
	_UNNEST

;   HOLD	( c -- )
;	Insert a character into the numeric output string.

	DCD	_BDIGS-MAPOFFSET
_HOLD	DCB  4
	DCB	"HOLD"
	ALIGN	
HOLD
	_NEST
	BL	HLD
	BL	AT
	BL	ONEM
	BL	DUPP
	BL	HLD
	BL	STORE
	BL	CSTOR
	_UNNEST

;   #	   ( u -- u )
;	Extract one digit from u and append the digit to output string.

	DCD	_HOLD-MAPOFFSET
_DIG	DCB  1
	DCB	"#"
	ALIGN	
DIG
	_NEST
	BL	BASE
	BL	AT
	BL	EXTRC
	BL	HOLD
	_UNNEST

;   #S	  ( u -- 0 )
;	Convert u until all digits are added to the output string.

	DCD	_DIG-MAPOFFSET
_DIGS	DCB  2
	DCB	"#S"
	ALIGN	
DIGS
	_NEST
DIGS1	  BL	DIG
	BL	DUPP
	BL	QBRAN
	DCD	DIGS2-MAPOFFSET
	B	DIGS1
DIGS2
	  _UNNEST
	ALIGN

;   SIGN	( n -- )
;	Add a minus sign to the numeric output string.

	DCD	_DIGS-MAPOFFSET
_SIGN	DCB  4
	DCB	"SIGN"
	ALIGN	
SIGN
	_NEST
	BL	ZLESS
	BL	QBRAN
	DCD	SIGN1-MAPOFFSET
	_DOLIT
	DCD	'-'
	BL	HOLD
SIGN1
	  _UNNEST

;   #>	  ( w -- b u )
;	Prepare the outputDCB to be TYPE'd.

	DCD	_SIGN-MAPOFFSET
_EDIGS	DCB  2
	DCB	"#>"
	ALIGN	
EDIGS
	_NEST
	BL	DROP
	BL	HLD
	BL	AT
	BL	PAD
	BL	OVER
	BL	SUBB
	_UNNEST

;   str	 ( n -- b u )
;	Convert a signed integer to a numeric string.

;	DCD	_EDIGS-MAPOFFSET
;_STRR	DCB  3
;	DCB	"str"
;	ALIGN	
STRR
	_NEST
	BL	DUPP
	BL	TOR
	BL	ABSS
	BL	BDIGS
	BL	DIGS
	BL	RFROM
	BL	SIGN
	BL	EDIGS
	_UNNEST

;   HEX	 ( -- )
;	Use radix 16 as base for numeric conversions.

	DCD	_EDIGS-MAPOFFSET
_HEX	DCB  3
	DCB	"HEX"
	ALIGN	
HEX
	_NEST
	_DOLIT
	DCD	16
	BL	BASE
	BL	STORE
	_UNNEST

;   DECIMAL	( -- )
;	Use radix 10 as base for numeric conversions.

	DCD	_HEX-MAPOFFSET
_DECIM	DCB  7
	DCB	"DECIMAL"
	ALIGN	
DECIM
	_NEST
	_DOLIT
	DCD	10
	BL	BASE
	BL	STORE
	_UNNEST

;**************************************************************************
; Numeric input, single precision

;   DIGIT?	( c base -- u t )
;	Convert a character to its numeric value. A flag indicates success.

	DCD	_DECIM-MAPOFFSET
_DIGTQ	DCB  6
	DCB	"DIGIT?"
	ALIGN	
DIGTQ
	_NEST
	BL	TOR
	_DOLIT
	DCD	'0'
	BL	SUBB
	_DOLIT
	DCD	9
	BL	OVER
	BL	LESS
	BL	QBRAN
	DCD	DGTQ1-MAPOFFSET
	_DOLIT
	DCD	7
	BL	SUBB
	BL	DUPP
	_DOLIT
	DCD	10
	BL	LESS
	BL	ORR
DGTQ1	  BL	DUPP
	BL	RFROM
	BL	ULESS
	_UNNEST

;   NUMBER?	( a -- n T | a F )
;	Convert a numberDCB to integer. Push a flag on tos.

	DCD	_DIGTQ-MAPOFFSET
_NUMBQ	DCB  7
	DCB	"NUMBER?"
	ALIGN	
NUMBQ
	_NEST
	BL	BASE
	BL	AT
	BL	TOR
	_DOLIT
	DCD	0
	BL	OVER
	BL	COUNT
	BL	OVER
	BL	CAT
	_DOLIT
	DCD	'_'
	BL	EQUAL
	BL	QBRAN
	DCD	NUMQ1-MAPOFFSET
	BL	HEX
	BL	SWAP
	BL	ONEP
	BL	SWAP
	BL	ONEM
NUMQ1	  BL	OVER
	BL	CAT
	_DOLIT
	DCD	'-'
	BL	EQUAL
	BL	TOR
	BL	SWAP
	BL	RAT
	BL	SUBB
	BL	SWAP
	BL	RAT
	BL	PLUS
	BL	QDUP
	BL	QBRAN
	DCD	NUMQ6-MAPOFFSET
	BL	ONEM
	BL	TOR
NUMQ2	  BL	DUPP
	BL	TOR
	BL	CAT
	BL	BASE
	BL	AT
	BL	DIGTQ
	BL	QBRAN
	DCD	NUMQ4-MAPOFFSET
	BL	SWAP
	BL	BASE
	BL	AT
	BL	STAR
	BL	PLUS
	BL	RFROM
	BL	ONEP
	BL	DONXT
	DCD	NUMQ2-MAPOFFSET
	BL	RAT
	BL	SWAP
	BL	DROP
	BL	QBRAN
	DCD	NUMQ3-MAPOFFSET
	BL	NEGAT
NUMQ3	  BL	SWAP
	B.W	NUMQ5
NUMQ4	  BL	RFROM
	BL	RFROM
	BL	DDROP
	BL	DDROP
	_DOLIT
	DCD	0
NUMQ5	  BL	DUPP
NUMQ6	  BL	RFROM
	BL	DDROP
	BL	RFROM
	BL	BASE
	BL	STORE
	_UNNEST

;**************************************************************************
; Basic I/O

;   KEY	 ( -- c )
;	Wait for and return an input character.

	DCD	_NUMBQ-MAPOFFSET
_KEY	DCB  3
	DCB	"KEY"
	ALIGN	
KEY
	_NEST
KEY1	   BL	QRX
	BL	QBRAN
	DCD	KEY1-MAPOFFSET
	_UNNEST

;   SPACE	( -- )
;	Send the blank character to the output device.

	DCD	_KEY-MAPOFFSET
_SPACE	DCB  5
	DCB	"SPACE"
	ALIGN	
SPACE
	_NEST
	BL	BLANK
	BL	EMIT
	_UNNEST

;   SPACES	( +n -- )
;	Send n spaces to the output device.

	DCD	_SPACE-MAPOFFSET
_SPACS	DCB  6
	DCB	"SPACES"
	ALIGN	
SPACS
	_NEST
	_DOLIT
	DCD	0
	BL	MAX
	BL	TOR
	B.W	CHAR2
CHAR1  BL	SPACE
CHAR2  BL	DONXT
	DCD	CHAR1-MAPOFFSET
	_UNNEST

;   TYPE	( b u -- )
;	Output u characters from b.

	DCD	_SPACS-MAPOFFSET
_TYPEE	DCB	4
	DCB	"TYPE"
	ALIGN	
TYPEE
	_NEST
	BL  TOR
	B.W	TYPE2
TYPE1  BL  COUNT
	BL	TCHAR
	BL	EMIT
TYPE2  BL  DONXT
	DCD	TYPE1-MAPOFFSET
	BL	DROP
	_UNNEST

;   CR	  ( -- )
;	Output a carriage return and a line feed.

	DCD	_TYPEE-MAPOFFSET
_CR	DCB  2
	DCB	"CR"
	ALIGN	
CR
	_NEST
	_DOLIT
	DCD	CRR
	BL	EMIT
	_DOLIT
	DCD	LF
	BL	EMIT
	_UNNEST

;   do_$	( -- a )
;	Return the address of a compiled string.

;	DCD	_CR-MAPOFFSET
;_DOSTR	DCB  COMPO+3
;	DCB	"do$$"
;	ALIGN	
DOSTR
	_NEST
	BL	RFROM
	BL	RFROM			; b0 set
	BL	ONEM			; clear b0
	BL	DUPP
	BL	COUNT			; get addr-1 count
	BL	PLUS
	BL	ALGND			; end of string
	BL	ONEP			; restore b0
	BL	TOR				; address after string
	BL	SWAP			; count tugged
	BL	TOR
	_UNNEST

;   $"|	( -- a )
;	Run time routine compiled by _". Return address of a compiled string.

;	DCD	_DOSTR-MAPOFFSET
;_STRQP	DCB  COMPO+3
;	DCB	"$$""|"
;	ALIGN	
STRQP
	_NEST
	BL	DOSTR
	_UNNEST			;force a call to dostr

;   .$	( -- )
;	Run time routine of ." . Output a compiled string.

;	DCD	_STRQP-MAPOFFSET
;_DOTST	DCB  COMPO+2
;	DCB	".$$"
;	ALIGN	
DOTST
	_NEST
	BL	COUNT
	BL	TYPEE
	_UNNEST

;   ."|	( -- )
;	Run time routine of ." . Output a compiled string.

;	DCD	_DOTST-MAPOFFSET
;_DOTQP	DCB  COMPO+3
;	DCB	".""|"
;	ALIGN	
DOTQP
	_NEST
	BL	DOSTR
	BL	DOTST
	_UNNEST

;   .R	  ( n +n -- )
;	Display an integer in a field of n columns, right justified.

	DCD	_CR-MAPOFFSET
_DOTR	DCB  2
	DCB	".R"
	ALIGN	
DOTR
	_NEST
	BL	TOR
	BL	STRR
	BL	RFROM
	BL	OVER
	BL	SUBB
	BL	SPACS
	BL	TYPEE
	_UNNEST

;   U.R	 ( u +n -- )
;	Display an unsigned integer in n column, right justified.

	DCD	_DOTR-MAPOFFSET
_UDOTR	DCB  3
	DCB	"U.R"
	ALIGN	
UDOTR
	_NEST
	BL	TOR
	BL	BDIGS
	BL	DIGS
	BL	EDIGS
	BL	RFROM
	BL	OVER
	BL	SUBB
	BL	SPACS
	BL	TYPEE
	_UNNEST

;   U.	  ( u -- )
;	Display an unsigned integer in free format.

	DCD	_UDOTR-MAPOFFSET
_UDOT	DCB  2
	DCB	"U."
	ALIGN	
UDOT
	_NEST
	BL	BDIGS
	BL	DIGS
	BL	EDIGS
	BL	SPACE
	BL	TYPEE
	_UNNEST

;   .	   ( w -- )
;	Display an integer in free format, preceeded by a space.

	DCD	_UDOT-MAPOFFSET
_DOT	DCB  1
	DCB	"."
	ALIGN	
DOT
	_NEST
	BL	BASE
	BL	AT
	_DOLIT
	DCD	10
	BL	XORR			;?decimal
	BL	QBRAN
	DCD	DOT1-MAPOFFSET
	BL	UDOT
	_UNNEST			;no,display unsigned
DOT1	   BL	STRR
	BL	SPACE
	BL	TYPEE
	_UNNEST			;yes, display signed

;   ?	   ( a -- )
;	Display the contents in a memory cell.

	DCD	_DOT-MAPOFFSET
_QUEST	DCB  1
	DCB	"?"
	ALIGN	
QUEST
	_NEST
	BL	AT
	BL	DOT
	_UNNEST

;**************************************************************************
; Parsing

;   parse	( b u c -- b u delta ; string> )
;	ScanDCB delimited by c. Return found string and its offset.

;	DCD	_QUEST-MAPOFFSET
;_PARS	DCB  5
;	DCB	"parse"
;	ALIGN	
PARS
	_NEST
	BL	TEMP
	BL	STORE
	BL	OVER
	BL	TOR
	BL	DUPP
	BL	QBRAN
	DCD	PARS8-MAPOFFSET
	BL	ONEM
	BL	TEMP
	BL	AT
	BL	BLANK
	BL	EQUAL
	BL	QBRAN
	DCD	PARS3-MAPOFFSET
	BL	TOR
PARS1	  BL	BLANK
	BL	OVER
	BL	CAT			;skip leading blanks 
	BL	SUBB
	BL	ZLESS
	BL	INVER
	BL	QBRAN
	DCD	PARS2-MAPOFFSET
	BL	ONEP
	BL	DONXT
	DCD	PARS1-MAPOFFSET
	BL	RFROM
	BL	DROP
	_DOLIT
	DCD	0
	BL	DUPP
	_UNNEST
PARS2	  BL	RFROM
PARS3	  BL	OVER
	BL	SWAP
	BL	TOR
PARS4	  BL	TEMP
	BL	AT
	BL	OVER
	BL	CAT
	BL	SUBB			;scan for delimiter
	BL	TEMP
	BL	AT
	BL	BLANK
	BL	EQUAL
	BL	QBRAN
	DCD	PARS5-MAPOFFSET
	BL	ZLESS
PARS5	  BL	QBRAN
	DCD	PARS6-MAPOFFSET
	BL	ONEP
	BL	DONXT
	DCD	PARS4-MAPOFFSET
	BL	DUPP
	BL	TOR
	B	PARS7
PARS6	  BL	RFROM
	BL	DROP
	BL	DUPP
	BL	ONEP
	BL	TOR
PARS7	  BL	OVER
	BL	SUBB
	BL	RFROM
	BL	RFROM
	BL	SUBB
	_UNNEST
PARS8	  BL	OVER
	BL	RFROM
	BL	SUBB
	_UNNEST
	ALIGN

;   PARSE	( c -- b u ; string> )
;	Scan input stream and return counted string delimited by c.

	DCD	_QUEST-MAPOFFSET
_PARSE	DCB  5
	DCB	"PARSE"
	ALIGN	
PARSE
	_NEST
	BL	TOR
	BL	TIB
	BL	INN
	BL	AT
	BL	PLUS			;current input buffer pointer
	BL	NTIB
	BL	AT
	BL	INN
	BL	AT
	BL	SUBB			;remaining count
	BL	RFROM
	BL	PARS
	BL	INN
	BL	PSTOR
	_UNNEST

;   .(	  ( -- )
;	Output following string up to next ) .

	DCD	_PARSE-MAPOFFSET
_DOTPR	DCB  IMEDD+2
	DCB	".("
	ALIGN	
DOTPR
	_NEST
	_DOLIT
	DCD	')'
	BL	PARSE
	BL	TYPEE
	_UNNEST

;   (	   ( -- )
;	Ignore following string up to next ) . A comment.

	DCD	_DOTPR-MAPOFFSET
_PAREN	DCB  IMEDD+1
	DCB	"("
	ALIGN	
PAREN	_NEST
	_DOLIT
	DCD	')'
	BL	PARSE
	BL	DDROP
	_UNNEST

;   \	   ( -- )
;	Ignore following text till the end of line.

	DCD	_PAREN-MAPOFFSET
_BKSLA	DCB  IMEDD+1
	DCB	"\\"
	ALIGN	
BKSLA
	_NEST
	BL	NTIB
	BL	AT
	BL	INN
	BL	STORE
	_UNNEST

;   CHAR	( -- c )
;	Parse next word and return its first character.

	DCD	_BKSLA-MAPOFFSET
_CHAR	DCB  4
	DCB	"CHAR"
	ALIGN	
CHAR
	_NEST
	BL	BLANK
	BL	PARSE
	BL	DROP
	BL	CAT
	_UNNEST

;   WORD	( c -- a ; string> )
;	Parse a word from input stream and copy it to code dictionary.

	DCD	_CHAR-MAPOFFSET
_WORDD	DCB  4
	DCB	"WORD"
	ALIGN	
WORDD
	_NEST
	BL	PARSE
	BL	HERE
	BL	CELLP
	BL	PACKS
	_UNNEST

;   TOKEN	( -- a ; string> )
;	Parse a word from input stream and copy it to name dictionary.

	DCD	_WORDD-MAPOFFSET
_TOKEN	DCB  5
	DCB	"TOKEN"
	ALIGN	
TOKEN
	_NEST
	BL	BLANK
	BL	WORDD
	_UNNEST

;**************************************************************************
; Dictionary search

;   NAME>	( na -- ca )
;	Return a code address given a name address.

	DCD	_TOKEN-MAPOFFSET
_NAMET	DCB  5
	DCB	"NAME>"
	ALIGN	
NAMET
	_NEST
	BL	COUNT
	_DOLIT
	DCD	0x1F
	BL	ANDD
	BL	PLUS
	BL	ALGND
	_UNNEST

;   SAME?	( a a u -- a a f \ -0+ )
;	Compare u cells in two strings. Return 0 if identical.

	DCD	_NAMET-MAPOFFSET
_SAMEQ	DCB  5
	DCB	"SAME?"
	ALIGN	
SAMEQ
	_NEST
	BL	TOR
	B.W	SAME2
SAME1	  BL	OVER
	BL	RAT
	BL	CELLS
	BL	PLUS
	BL	AT			;32/16 mix-up
	BL	OVER
	BL	RAT
	BL	CELLS
	BL	PLUS
	BL	AT			;32/16 mix-up
	BL	SUBB
	BL	QDUP
	BL	QBRAN
	DCD	SAME2-MAPOFFSET
	BL	RFROM
	BL	DROP
	_UNNEST			;strings not equal
SAME2	  BL	DONXT
	DCD	SAME1-MAPOFFSET
	_DOLIT
	DCD	0
	_UNNEST			;strings equal

;   find	( a na -- ca na | a F )
;	Search a vocabulary for a string. Return ca and na if succeeded.

;	DCD	_SAMEQ-MAPOFFSET
;_FIND	DCB  4
;	DCB	"find"
;	ALIGN	
FIND
	_NEST
	BL	SWAP			; na a	
	BL	DUPP			; na a a
	BL	CAT			; na a count
	BL	CELLSL		; na a count/4
	BL	TEMP
	BL	STORE			; na a
	BL	DUPP			; na a a
	BL	AT			; na a word1
	BL	TOR			; na a
	BL	CELLP			; na a+4
	BL	SWAP			; a+4 na
FIND1
	BL	DUPP			; a+4 na na
	BL	QBRAN
	DCD	FIND6-MAPOFFSET	; end of vocabulary
	BL	DUPP			; a+4 na na
	BL	AT			; a+4 na name1
	_DOLIT
	DCD	MASKK
	BL	ANDD
	BL	RAT			; a+4 na name1 word1
	BL	XORR			; a+4 na ?
	BL	QBRAN
	DCD	FIND2-MAPOFFSET
	BL	CELLM			; a+4 la
	BL	AT			; a+4 next_na
	B.w	FIND1			; try next word
FIND2   
	BL	CELLP			; a+4 na+4
	BL	TEMP
	BL	AT			; a+4 na+4 count/4
	BL	SAMEQ			; a+4 na+4 ? 
FIND3	
	B.w	FIND4
FIND6	
	BL	RFROM			; a+4 0 name1 -- , no match
	BL	DROP			; a+4 0
	BL	SWAP			; 0 a+4
	BL	CELLM			; 0 a
	BL	SWAP			; a 0 
	_UNNEST			; return without a match
FIND4	
	BL	QBRAN			; a+4 na+4
	DCD	FIND5-MAPOFFSET	; found a match
	BL	CELLM			; a+4 na
	BL	CELLM			; a+4 la
	BL	AT			; a+4 next_na
	B.w	FIND1			; compare next name
FIND5	
	BL	RFROM			; a+4 na+4 count/4
	BL	DROP			; a+4 na+4
	BL	SWAP			; na+4 a+4
	BL	DROP			; na+4
	BL	CELLM			; na
	BL	DUPP			; na na
	BL	NAMET			; na ca
	BL	SWAP			; ca na
	_UNNEST			; return with a match
	ALIGN

;   NAME?	( a -- ca na | a F )
;	Search all context vocabularies for a string.

	DCD	_SAMEQ-MAPOFFSET
_NAMEQ	DCB  5
	DCB	"NAME?"
	ALIGN	
NAMEQ
	_NEST
	BL	CNTXT
	BL	AT
	BL	FIND
	_UNNEST

;**************************************************************************
; Terminal input

;   ^H	  ( bot eot cur -- bot eot cur )
;	Backup the cursor by one character.

;	DCD	_NAMEQ-MAPOFFSET
;_BKSP	DCB  2
;	DCB	"^H"
;	ALIGN	
BKSP
	_NEST
	BL	TOR
	BL	OVER
	BL	RFROM
	BL	SWAP
	BL	OVER
	BL	XORR
	BL	QBRAN
	DCD	BACK1-MAPOFFSET
	_DOLIT
	DCD	BKSPP
	BL	TECHO
;	BL	ATEXE
	BL	ONEM
	BL	BLANK
	BL	TECHO
;	BL	ATEXE
	_DOLIT
	DCD	BKSPP
	BL	TECHO
;	BL	ATEXE
BACK1
	  _UNNEST

;   TAP	 ( bot eot cur c -- bot eot cur )
;	Accept and echo the key stroke and bump the cursor.

;	DCD	_BKSP-MAPOFFSET
;_TAP	DCB  3
;	DCB	"TAP"
;	ALIGN	
TAP
	_NEST
	BL	DUPP
	BL	TECHO
;	BL	ATEXE
	BL	OVER
	BL	CSTOR
	BL	ONEP
	_UNNEST

;   kTAP	( bot eot cur c -- bot eot cur )
;	Process a key stroke, CR or backspace.

;	DCD	_TAP-MAPOFFSET
;_KTAP	DCB  4
;	DCB	"kTAP"
;	ALIGN	
KTAP
TTAP
	_NEST
	BL	DUPP
	_DOLIT
	DCD	CRR
	BL	XORR
	BL	QBRAN
	DCD	KTAP2-MAPOFFSET
	_DOLIT
	DCD	BKSPP
	BL	XORR
	BL	QBRAN
	DCD	KTAP1-MAPOFFSET
	BL	BLANK
	BL	TAP
	_UNNEST
	DCD	0			;patch
KTAP1	  BL	BKSP
	_UNNEST
KTAP2	  BL	DROP
	BL	SWAP
	BL	DROP
	BL	DUPP
	_UNNEST

;   ACCEPT	( b u -- b u )
;	Accept characters to input buffer. Return with actual count.

	DCD	_NAMEQ-MAPOFFSET
_ACCEP	DCB  6
	DCB	"ACCEPT"
	ALIGN	
ACCEP
	_NEST
	BL	OVER
	BL	PLUS
	BL	OVER
ACCP1	  BL	DDUP
	BL	XORR
	BL	QBRAN
	DCD	ACCP4-MAPOFFSET
	BL	KEY
	BL	DUPP
	BL	BLANK
	_DOLIT
	DCD	127
	BL	WITHI
	BL	QBRAN
	DCD	ACCP2-MAPOFFSET
	BL	TAP
	B	ACCP3
ACCP2	  BL	KTAP
;	BL	ATEXE
ACCP3	  
	B	ACCP1
ACCP4	  BL	DROP
	BL	OVER
	BL	SUBB
	_UNNEST

;   QUERY	( -- )
;	Accept input stream to terminal input buffer.

	DCD	_ACCEP-MAPOFFSET
_QUERY	DCB  5
	DCB	"QUERY"
	ALIGN	
QUERY
	_NEST
	BL	TIB
	_DOLIT
	DCD	80
	BL	ACCEP
	BL	NTIB
	BL	STORE
	BL	DROP
	_DOLIT
	DCD	0
	BL	INN
	BL	STORE
	_UNNEST

;**************************************************************************
; Error handling

;   ABORT	( a -- )
;	Reset data stack and jump to QUIT.

	DCD	_QUERY-MAPOFFSET
_ABORT	DCB  5
	DCB	"ABORT"
	ALIGN	
ABORT
	_NEST
	BL	SPACE
	BL	COUNT
	BL	TYPEE
	_DOLIT
	DCD	0X3F
	BL	EMIT
	BL	CR
	BL	PRESE
	B.W	QUIT
	ALIGN

;   _abort"	( f -- )
;	Run time routine of ABORT" . Abort with a message.

;	DCD	_ABORT-MAPOFFSET
;_ABORQ	DCB  COMPO+6
;	DCB	"abort\""
;	ALIGN	
ABORQ
	_NEST
	BL	QBRAN
	DCD	ABOR1-MAPOFFSET	;text flag
	BL	DOSTR
	BL	COUNT
	BL	TYPEE
	BL	CR
	B.W	QUIT
ABOR1	  BL	DOSTR
	BL	DROP
	_UNNEST			;drop error

;**************************************************************************
; The text interpreter

;   $INTERPRET  ( a -- )
;	Interpret a word. If failed, try to convert it to an integer.

	DCD	_ABORT-MAPOFFSET
_INTER	DCB  10
	DCB	"$$INTERPRET"
	ALIGN	
INTER
	_NEST
	BL	NAMEQ
	BL	QDUP	;?defined
	BL	QBRAN
	DCD	INTE1-MAPOFFSET
	BL	AT
	_DOLIT
	DCD	COMPO
	BL	ANDD	;?compile only lexicon bits
	BL	ABORQ
	DCB	13
	DCB	" compile only"
	ALIGN	
	BL	EXECU
	_UNNEST			;execute defined word
INTE1	  BL	NUMBQ
	BL	QBRAN
	DCD	INTE2-MAPOFFSET
	_UNNEST
INTE2	  B.W	ABORT	;error

;   [	   ( -- )
;	Start the text interpreter.

	DCD	_INTER-MAPOFFSET
_LBRAC	DCB  IMEDD+1
	DCB	"["
	ALIGN	
LBRAC
	_NEST
	_DOLIT
	DCD	INTER-MAPOFFSET
	BL	TEVAL
	BL	STORE
	_UNNEST

;   .OK	 ( -- )
;	Display "ok" only while interpreting.

	DCD	_LBRAC-MAPOFFSET
_DOTOK	DCB  3
	DCB	".OK"
	ALIGN	
DOTOK
	_NEST
	_DOLIT
	DCD	INTER-MAPOFFSET
	BL	TEVAL
	BL	AT
	BL	EQUAL
	BL	QBRAN
	DCD	DOTO1-MAPOFFSET
	BL	DOTQP
	DCB	3
	DCB	" ok"
	ALIGN	
DOTO1	  BL	CR
	_UNNEST

;   ?STACK	( -- )
;	Abort if the data stack underflows.

	DCD	_DOTOK-MAPOFFSET
_QSTAC	DCB  6
	DCB	"?STACK"
	ALIGN	
QSTAC
	_NEST
	BL	DEPTH
	BL	ZLESS	;check only for underflow
	BL	ABORQ
	DCB	10
	DCB	" underflow"
	ALIGN	
	_UNNEST

;   EVAL	( -- )
;	Interpret the input stream.

	DCD	_QSTAC-MAPOFFSET
_EVAL	DCB  4
	DCB	"EVAL"
	ALIGN	
EVAL
	_NEST
EVAL1	  BL	TOKEN
	BL	DUPP
	BL	CAT	;?input stream empty
	BL	QBRAN
	DCD	EVAL2-MAPOFFSET
	BL	TEVAL
	BL	ATEXE
	BL	QSTAC	;evaluate input, check stack
	B.W	EVAL1
EVAL2	  BL	DROP
	BL	DOTOK
	_UNNEST	;prompt
	ALIGN

;   PRESET	( -- )
;	Reset data stack pointer and the terminal input buffer.

	DCD	_EVAL-MAPOFFSET
_PRESE	DCB  6
	DCB	"PRESET"
	ALIGN	
PRESE
	_NEST
	MOVW	R1,#0XFE00		; init SP
;	MOVT	R1,#0X2000
	MOVW	R0,#0			; init TOS
	_UNNEST

;   QUIT	( -- )
;	Reset return stack pointer and start text interpreter.

	DCD	_PRESE-MAPOFFSET
_QUIT	DCB  4
	DCB	"QUIT"
	ALIGN	
QUIT
	_NEST
	MOVW	R2,#0XFF00
;	MOVT	R2,#0X2000
QUIT1	  BL	LBRAC			;start interpretation
QUIT2	  BL	QUERY			;get input
	BL	EVAL
	BL	BRAN
	DCD	QUIT2-MAPOFFSET	;continue till error

;**************************************************************************
; Flash memory interface

FLASH	EQU	0x40023C00
FLASH_KEYR	EQU	0X04
FLASH_SR	EQU	0x0C
FLASH_CR	EQU	0X10
FLASH_KEY1	EQU	0x45670123
FLASH_KEY2	EQU	0xCDEF89AB
	
UNLOCK	; unlock flash memory	
	ldr	r0, =FLASH
	ldr	r4, =FLASH_KEY1
	str	r4, [r0, #0x4]
	ldr	r4, =FLASH_KEY2
	str	r4, [r0, #0x4]
	mov	r4, #0x200		; PSIZE 32 bits
	str	r4, [r0, #0x10]
	
	_NEXT
WAIT_BSY
	ldr	r0, =FLASH
WAIT1	ldr	r4, [r0, #0x0C]	; FLASH_SR
	ands	r4, #0x1000	; BSY
	bne	WAIT1
	_NEXT
	ALIGN

;   ERASE_SECTOR	   ( sector -- )
;	  Erase one sector of flash memory.  Sector=0 to 11

	DCD	_QUIT-MAPOFFSET
_ESECT	DCB  12
	DCB	"ERASE_SECTOR"
	ALIGN	

ESECT 	; sector --
	_NEST
	bl	WAIT_BSY
	ldr	r4,[r0, #0x10]	; FLASH_CR
	bic	r4,r4,#0x78	; clear SNB
	lsl	R5,R5,#3		; align sector #
	orr	r4,r4,r5		; put in sector #
	orr	R4,R4,#0x10000	; set STRT bit
	orr	R4,R4,#0x200	; PSIZE=32
	orr	R4,R4,#2		; set SER bit, enable erase
	str	r4,[r0, #0x10]	; start erasing
;	bl	WAIT_BSY
	_POP
	_UNNEST

;   I!	   ( data address -- )
;	   Write one word into flash memory

	DCD	_ESECT-MAPOFFSET
_ISTOR	DCB  2
	DCB	"I!"
	ALIGN	

ISTOR	; data address --
	_NEST
	bl	WAIT_BSY
	ldr	r4, [r0, #0x10]	; FLASH_CR
	orr	r4,R4,#0x1		; PG
	str	r4, [r0, #0x10]	; enable programming
	bl	STORE
	bl	WAIT_BSY
	ldr	r4, [r0, #0x10]	; FLASH_CR
	bic	r4,R4,#0x1		; PG
	str	r4, [r0, #0x10]	; disable programming
	_UNNEST
	ALIGN
	LTORG

;   TURNKEY	( -- )
;	Copy dictionary from RAM to flash.

	DCD	_ISTOR-MAPOFFSET
_TURN	DCB   7
	DCB	"TURNKEY"
	ALIGN
TURN	_NEST
	_DOLIT			; save user area
	DCD	0XFF00
	_DOLIT
	DCD	0xC0			; to boot array
	_DOLIT
	DCD	0x40
	BL	MOVE
	_DOLIT
	DCD	0
	_DOLIT
	DCD	0x8000000
	BL	CPP
	BL	AT
	BL	CELLSL
	BL	TOR
TURN1	BL	OVER
	BL	AT
	BL	OVER
	BL	ISTOR
	BL	SWAP
	BL	CELLP
	BL	SWAP
	BL	CELLP
	BL	DONXT
	DCD	TURN1-MAPOFFSET
	BL	DDROP
	_UNNEST
	ALIGN

;**************************************************************************
; The compiler

;   '	   ( -- ca )
;	Search context vocabularies for the next word in input stream.

	DCD	_TURN-MAPOFFSET
_TICK	DCB  1
	DCB	"'"
	ALIGN	
TICK
	_NEST
	BL	TOKEN
	BL	NAMEQ	;?defined
	BL	QBRAN
	DCD	TICK1-MAPOFFSET
	_UNNEST	;yes, push code address
TICK1	B.W	ABORT	;no, error

;   ALLOT	( n -- )
;	Allocate n bytes to the ram area.

	DCD	_TICK-MAPOFFSET
_ALLOT	DCB  5
	DCB	"ALLOT"
	ALIGN	
ALLOT
	_NEST
	BL	CPP
	BL	PSTOR
	_UNNEST			;adjust code pointer

;   ,	   ( w -- )
;	Compile an integer into the code dictionary.

	DCD	_ALLOT-MAPOFFSET
_COMMA	DCB  1,","
	ALIGN	
COMMA
	_NEST
	BL	HERE
	BL	DUPP
	BL	CELLP	;cell boundary
	BL	CPP
	BL	STORE
	BL	STORE
	_UNNEST	;adjust code pointer, compile

;   [COMPILE]   ( -- ; string> )
;	Compile the next immediate word into code dictionary.

	DCD	_COMMA-MAPOFFSET
_BCOMP	DCB  IMEDD+9
	DCB	"[COMPILE]"
	ALIGN	
BCOMP
	_NEST
	BL	TICK
	BL	COMMA
	_UNNEST

;   COMPILE	( -- )
;	Compile the next address in colon list to code dictionary.

	DCD	_BCOMP-MAPOFFSET
_COMPI	DCB  COMPO+7
	DCB	"COMPILE"
	ALIGN	
COMPI
	_NEST
	BL	RFROM
	BIC	R5,R5,#1
	BL	DUPP
	BL	AT
	BL	CALLC			;compile BL instruction
	BL	CELLP
	ORR	R5,R5,#1
	BL	TOR
	_UNNEST			;adjust return address

;   LITERAL	( w -- )
;	Compile tos to code dictionary as an integer literal.

	DCD	_COMPI-MAPOFFSET
_LITER	DCB  IMEDD+7
	DCB	"LITERAL"
	ALIGN	
LITER
	_NEST
	BL	COMPI
	DCD	DOLIT-MAPOFFSET
	BL	COMMA
	_UNNEST

;   $,"	( -- )
;	Compile a literal string up to next " .

;	DCD	_LITER-MAPOFFSET
;_STRCQ	DCB  3
;	DCB	"$$,"""
;	ALIGN	
STRCQ
	_NEST
	_DOLIT
	DCD	-4
	BL	CPP
	BL	PSTOR
	_DOLIT
	DCD	'\"'
	BL	WORDD			;moveDCB to code dictionary
	BL	COUNT
	BL	PLUS
	BL	ALGND			;calculate aligned end of string
	BL	CPP
	BL	STORE
	_UNNEST 			;adjust the code pointer

;**************************************************************************
; Structures

;   FOR	 ( -- a )
;	Start a FOR-NEXT loop structure in a colon definition.

	DCD	_LITER-MAPOFFSET
_FOR	DCB  IMEDD+3
	DCB	"FOR"
	ALIGN	
FOR
	_NEST
	BL	COMPI
	DCD	TOR-MAPOFFSET
	BL	HERE
	_UNNEST

;   BEGIN	( -- a )
;	Start an infinite or indefinite loop structure.

	DCD	_FOR-MAPOFFSET
_BEGIN	DCB  IMEDD+5
	DCB	"BEGIN"
	ALIGN	
BEGIN
	_NEST
	BL	HERE
	_UNNEST

;   NEXT	( a -- )
;	Terminate a FOR-NEXT loop structure.

	DCD	_BEGIN-MAPOFFSET
_NEXT	DCB  IMEDD+4
	DCB	"NEXT"
	ALIGN	
NEXT
	_NEST
	BL	COMPI
	DCD	DONXT-MAPOFFSET
	BL	COMMA
	_UNNEST

;   UNTIL	( a -- )
;	Terminate a BEGIN-UNTIL indefinite loop structure.

	DCD	_NEXT-MAPOFFSET
_UNTIL	DCB  IMEDD+5
	DCB	"UNTIL"
	ALIGN	
UNTIL
	_NEST
	BL	COMPI
	DCD	QBRAN-MAPOFFSET
	BL	COMMA
	_UNNEST

;   AGAIN	( a -- )
;	Terminate a BEGIN-AGAIN infinite loop structure.

	DCD	_UNTIL-MAPOFFSET
_AGAIN	DCB  IMEDD+5
	DCB	"AGAIN"
	ALIGN	
AGAIN
	_NEST
	BL	COMPI
	DCD	BRAN-MAPOFFSET
	BL	COMMA
	_UNNEST

;   IF	  ( -- A )
;	Begin a conditional branch structure.

	DCD	_AGAIN-MAPOFFSET
_IFF	DCB  IMEDD+2
	DCB	"IF"
	ALIGN	
IFF
	_NEST
	BL	COMPI
	DCD	QBRAN-MAPOFFSET
	BL	HERE
	_DOLIT
	DCD	4
	BL	CPP
	BL	PSTOR
	_UNNEST

;   AHEAD	( -- A )
;	Compile a forward branch instruction.

	DCD	_IFF-MAPOFFSET
_AHEAD	DCB  IMEDD+5
	DCB	"AHEAD"
	ALIGN	
AHEAD
	_NEST
	BL	COMPI
	DCD	BRAN-MAPOFFSET
	BL	HERE
	_DOLIT
	DCD	4
	BL	CPP
	BL	PSTOR
	_UNNEST

;   REPEAT	( A a -- )
;	Terminate a BEGIN-WHILE-REPEAT indefinite loop.

	DCD	_AHEAD-MAPOFFSET
_REPEA	DCB  IMEDD+6
	DCB	"REPEAT"
	ALIGN	
REPEA
	_NEST
	BL	AGAIN
	BL	HERE
	BL	SWAP
	BL	STORE
	_UNNEST

;   THEN	( A -- )
;	Terminate a conditional branch structure.

	DCD	_REPEA-MAPOFFSET
_THENN	DCB  IMEDD+4
	DCB	"THEN"
	ALIGN	
THENN
	_NEST
	BL	HERE
	BL	SWAP
	BL	STORE
	_UNNEST

;   AFT	 ( a -- a A )
;	Jump to THEN in a FOR-AFT-THEN-NEXT loop the first time through.

	DCD	_THENN-MAPOFFSET
_AFT	DCB  IMEDD+3
	DCB	"AFT"
	ALIGN	
AFT
	_NEST
	BL	DROP
	BL	AHEAD
	BL	BEGIN
	BL	SWAP
	_UNNEST

;   ELSE	( A -- A )
;	Start the false clause in an IF-ELSE-THEN structure.

	DCD	_AFT-MAPOFFSET
_ELSEE	DCB  IMEDD+4
	DCB	"ELSE"
	ALIGN	
ELSEE
	_NEST
	BL	AHEAD
	BL	SWAP
	BL	THENN
	_UNNEST

;   WHILE	( a -- A a )
;	Conditional branch out of a BEGIN-WHILE-REPEAT loop.

	DCD	_ELSEE-MAPOFFSET
_WHILE	DCB  IMEDD+5
	DCB	"WHILE"
	ALIGN	
WHILE
	_NEST
	BL	IFF
	BL	SWAP
	_UNNEST

;   ABORT"	( -- ; string> )
;	Conditional abort with an error message.

	DCD	_WHILE-MAPOFFSET
_ABRTQ	DCB  IMEDD+6
	DCB	"ABORT\""
	ALIGN	
ABRTQ
	_NEST
	BL	COMPI
	DCD	ABORQ-MAPOFFSET
	BL	STRCQ
	_UNNEST

;   $"	( -- ; string> )
;	Compile an inlineDCB literal.

	DCD	_ABRTQ-MAPOFFSET
_STRQ	DCB  IMEDD+2
	DCB	"$$"""
	ALIGN	
STRQ
	_NEST
	BL	COMPI
	DCD	STRQP-MAPOFFSET
	BL	STRCQ
	_UNNEST

;   ."	( -- ; string> )
;	Compile an inlineDCB literal to be typed out at run time.

	DCD	_STRQ-MAPOFFSET
_DOTQ	DCB  IMEDD+2
	DCB	"."""
	ALIGN	
DOTQ
	_NEST
	BL	COMPI
	DCD	DOTQP-MAPOFFSET
	BL	STRCQ
	_UNNEST

;**************************************************************************
; Name compiler

;   ?UNIQUE	( a -- a )
;	Display a warning message if the word already exists.

	DCD	_DOTQ-MAPOFFSET
_UNIQU	DCB  7
	DCB	"?UNIQUE"
	ALIGN	
UNIQU
	_NEST
	BL	DUPP
	BL	NAMEQ			;?name exists
	BL	QBRAN
	DCD	UNIQ1-MAPOFFSET	;redefinitions are OK
	BL	DOTQP
	DCB	7
	DCB	" reDef "		;but warn the user
	ALIGN	
	BL	OVER
	BL	COUNT
	BL	TYPEE			;just in case its not planned
UNIQ1	BL	DROP
	_UNNEST

;   $,n	 ( na -- )
;	Build a new dictionary name using the data at na.

;	DCD	_UNIQU-MAPOFFSET
;_SNAME	DCB  3
;	DCB	"$$,n"
;	ALIGN	
SNAME
	_NEST
	BL	DUPP			; na na
	BL	CAT			; ?null input
	BL	QBRAN
	DCD	SNAM1-MAPOFFSET
	BL	UNIQU			; na
	BL	LAST			; na last
	BL	AT			; na la
	BL	COMMA			; na
	BL	DUPP			; na na
	BL	LAST			; na na last
	BL	STORE			; na , save na for vocabulary link
	BL	COUNT			; na+1 count
	BL	PLUS			; na+1+count
	BL	ALGND			; word boundary
	BL	CPP
	BL	STORE			; top of dictionary now
	_UNNEST
SNAM1
	BL	STRQP
	DCB	7," name? "
	B.W	ABORT

;   $COMPILE	( a -- )
;	Compile next word to code dictionary as a token or literal.

	DCD	_UNIQU-MAPOFFSET
_SCOMP	DCB  8
	DCB	"$$COMPILE"
	ALIGN	
SCOMP
	_NEST
	BL	NAMEQ
	BL	QDUP	;defined?
	BL	QBRAN
	DCD	SCOM2-MAPOFFSET
	BL	AT
	_DOLIT
	DCD	IMEDD
	BL	ANDD	;immediate?
	BL	QBRAN
	DCD	SCOM1-MAPOFFSET
	BL	EXECU
	_UNNEST			;it's immediate, execute
SCOM1	BL	CALLC			;it's not immediate, compile
	_UNNEST	
SCOM2	BL	NUMBQ
	BL	QBRAN
	DCD	SCOM3-MAPOFFSET
	BL	LITER
	_UNNEST			;compile number as integer
SCOM3	B.W	ABORT			;error

;   OVERT	( -- )
;	Link a new word into the current vocabulary.

	DCD	_SCOMP-MAPOFFSET
_OVERT	DCB  5
	DCB	"OVERT"
	ALIGN	
OVERT
	_NEST
	BL	LAST
	BL	AT
	BL	CNTXT
	BL	STORE
	_UNNEST

;   ;	   ( -- )
;	Terminate a colon definition.

	DCD	_OVERT-MAPOFFSET
_SEMIS	DCB  IMEDD+COMPO+1
	DCB	";"
	ALIGN	
SEMIS
	_NEST
	_DOLIT
	_UNNEST
	BL	COMMA
	BL	LBRAC
	BL	OVERT
	_UNNEST

;   ]	   ( -- )
;	Start compiling the words in the input stream.

	DCD	_SEMIS-MAPOFFSET
_RBRAC	DCB  1
	DCB	"]"
	ALIGN	
RBRAC
	_NEST
	_DOLIT
	DCD	SCOMP-MAPOFFSET
	BL	TEVAL
	BL	STORE
	_UNNEST

;   BL.W	( ca -- )
;	Assemble a branch-link long instruction to ca.
;	BL.W is split into 2 16 bit instructions with 11 bit address fields.

;	DCD	_RBRAC-MAPOFFSET
;_CALLC	DCB  5
;	DCB	"call,"
;	ALIGN	
CALLC
	_NEST
	BIC	R5,R5,#1		; clear b0 of address from R>
	BL	HERE
	BL	SUBB
	SUB	R5,R5,#4		; pc offset
	MOVW	R0,#0x7FF		; 11 bit mask
	MOV	R4,R5
	LSR	R5,R5,#12		; get bits 22-12
	AND	R5,R5,R0
	LSL	R4,R4,#15		; get bits 11-1
	ORR	R5,R5,R4
	ORR	R5,R5,#0xF8000000
	ORR	R5,R5,#0xF000
	BL	COMMA			; assemble BL.W instruction
	_UNNEST
	ALIGN

;	:	( -- ; string> )
;	Start a new colon definition using next word as its name.

	DCD	_RBRAC-MAPOFFSET
_COLON	DCB  1
	DCB	":"
	ALIGN	
COLON
	_NEST
	BL	TOKEN
	BL	SNAME
	_DOLIT
	_NEST
	BL	COMMA
	BL	RBRAC
	_UNNEST

;   IMMEDIATE   ( -- )
;	Make the last compiled word an immediate word.

	DCD	_COLON-MAPOFFSET
_IMMED	DCB  9
	DCB	"IMMEDIATE"
	ALIGN	
IMMED
	_NEST
	_DOLIT
	DCD	IMEDD
	BL	LAST
	BL	AT
	BL	AT
	BL	ORR
	BL	LAST
	BL	AT
	BL	STORE
	_UNNEST

;**************************************************************************
; Defining words

;   CONSTANT	( u -- ; string> )
;	Compile a new constant.

	DCD	_IMMED-MAPOFFSET
_CONST	DCB  8
	DCB	"CONSTANT"
	ALIGN	
CONST
	_NEST
	BL	TOKEN
	BL	SNAME
	BL	OVERT
	_DOLIT
	_NEST
	BL	COMMA
	_DOLIT
	DCD	DOCON-MAPOFFSET
	BL	CALLC
	BL	COMMA
	_UNNEST

;   CREATE	( -- ; string> )
;	Compile a new array entry without allocating code space.

	DCD	_CONST-MAPOFFSET
_CREAT	DCB  6
	DCB	"CREATE"
	ALIGN	
CREAT
	_NEST
	BL	TOKEN
	BL	SNAME
	BL	OVERT
	_DOLIT
	_NEST
	BL	COMMA
	_DOLIT
	DCD	DOVAR-MAPOFFSET
	BL	CALLC
	_UNNEST

;   VARIABLE	( -- ; string> )
;	Compile a new variable initialized to 0.

	DCD	_CREAT-MAPOFFSET
_VARIA	DCB  8
	DCB	"VARIABLE"
	ALIGN	
VARIA
	_NEST
	BL	CREAT
	_DOLIT
	DCD	0
	BL	COMMA
	_UNNEST

;**************************************************************************
; Tools

;   dm+	 ( a u -- a )
;	Dump u bytes from , leaving a+u on the stack.

;	DCD	_VARIA-MAPOFFSET
;_DMP	DCB  3
;	DCB	"dm+"
;	ALIGN	
DMP
	_NEST
	BL	OVER
	_DOLIT
	DCD	4
	BL	UDOTR			;display address
	BL	SPACE
	BL	TOR			;start count down loop
	B.W	PDUM2			;skip first pass
PDUM1	  BL	DUPP
	BL	CAT
	_DOLIT
	DCD	3
	BL	UDOTR			;display numeric data
	BL	ONEP			;increment address
PDUM2	  BL	DONXT
	DCD	PDUM1-MAPOFFSET	;loop till done
	_UNNEST

;   DUMP	( a u -- )
;	Dump u bytes from a, in a formatted manner.

	DCD	_VARIA-MAPOFFSET
_DUMP	DCB  4
	DCB	"DUMP"
	ALIGN	
DUMP
	_NEST
	BL	BASE
	BL	AT
	BL	TOR
	BL	HEX			;save radix,set hex
	_DOLIT
	DCD	16
	BL	SLASH			;change count to lines
	BL	TOR
	B.W	DUMP4			;start count down loop
DUMP1	  BL	CR
	_DOLIT
	DCD	16
	BL	DDUP
	BL	DMP			;display numeric
	BL	ROT
	BL	ROT
	BL	SPACE
	BL	SPACE
	BL	TYPEE			;display printable characters
DUMP4	  BL	DONXT
	DCD	DUMP1-MAPOFFSET	;loop till done
DUMP3	  BL	DROP
	BL	RFROM
	BL	BASE
	BL	STORE			;restore radix
	_UNNEST

;   .S	  ( ... -- ... )
;	Display the contents of the data stack.

	DCD	_DUMP-MAPOFFSET
_DOTS	DCB  2
	DCB	".S"
	ALIGN	
DOTS
	_NEST
	BL	SPACE
	BL	DEPTH			;stack depth
	BL	TOR			;start count down loop
	B.W	DOTS2			;skip first pass
DOTS1	  BL	RAT
	BL	PICK
	BL	DOT			;index stack, display contents
DOTS2	  BL	DONXT
	DCD	DOTS1-MAPOFFSET	;loop till done
	BL	SPACE
	_UNNEST

;   >NAME	( ca -- na | F )
;	Convert code address to a name address.

	DCD	_DOTS-MAPOFFSET
_TNAME	DCB  5
	DCB	">NAME"
	ALIGN	
TNAME
	_NEST
	BL	TOR			; 
	BL	CNTXT			; va
	BL	AT			; na
TNAM1
	BL	DUPP			; na na
	BL	QBRAN
	DCD	TNAM2-MAPOFFSET	; vocabulary end, no match
	BL	DUPP			; na na
	BL	NAMET			; na ca
	BL	RAT			; na ca code
	BL	XORR			; na f --
	BL	QBRAN
	DCD	TNAM2-MAPOFFSET
	BL	CELLM			; la 
	BL	AT			; next_na
	B.W	TNAM1
TNAM2	
	BL	RFROM
	BL	DROP			; 0|na --
	_UNNEST			;0

;   .ID	 ( na -- )
;	Display the name at address.

	DCD	_TNAME-MAPOFFSET
_DOTID	DCB  3
	DCB	".ID"
	ALIGN	
DOTID
	_NEST
	BL	QDUP			;if zero no name
	BL	QBRAN
	DCD	DOTI1-MAPOFFSET
	BL	COUNT
	_DOLIT
	DCD	0x1F
	BL	ANDD			;mask lexicon bits
	BL	TYPEE
	_UNNEST			;display name string
DOTI1	  BL	DOTQP
	DCB	9
	DCB	" {noName}"
	ALIGN	
	_UNNEST

;   SEE	 ( -- ; string> )
;	A simple decompiler.

	DCD	_DOTID-MAPOFFSET
_SEE	DCB  3
	DCB	"SEE"
	ALIGN	
SEE
	_NEST
	BL	TICK	; ca --, starting address
	BL	CR	
	_DOLIT
	DCD	20
	BL	TOR
SEE1	BL	CELLP			; a
	BL	DUPP			; a a
	BL	DECOMP		; a
	BL	DONXT
	DCD	SEE1-MAPOFFSET
	BL	DROP
	_UNNEST

;	DECOMPILE ( a -- )
;	Convert code in a.  Display name of command or as data.

	DCD	_SEE-MAPOFFSET
_DECOM	DCB  9
	DCB	"DECOMPILE"
	ALIGN
	
DECOMP	
	_NEST
	BL	DUPP			; a a
;	BL	TOR			; a
	BL	AT			; a code
	BL	DUPP			; a code code
	_DOLIT
	DCD	0xF800F800
	BL	ANDD
	_DOLIT
	DCD	0xF800F000
	BL	EQUAL			; a code ?
	BL	QBRAN
	DCD	DECOM2-MAPOFFSET	; not a command
	; a valid_code --, extract address and display name
	MOVW	R0,#0xFFE
	MOV	R4,R5
	LSL	R5,R5,#21		; get bits 22-12
	ASR	R5,R5,#9		; with sign extension
	LSR	R4,R4,#15		; get bits 11-1
	AND	R4,R4,R0		; retain only bits 11-1
	ORR	R5,R5,R4		; get bits 22-1
	NOP
	BL	OVER			; a offset a
	BL	PLUS			; a target-4
	BL	CELLP			; a target
	BL	TNAME			; a na/0 --, is it a name?
	BL	QDUP			; name address or zero
	BL	QBRAN
	DCD	DECOM1-MAPOFFSET
	BL	SPACE			; a na
	BL	DOTID			; a --, display name
;	BL	RFROM			; a
	BL	DROP
	_UNNEST
DECOM1	;BL	RFROM		; a
	BL	AT			; data
	BL	UDOT			; display data
	_UNNEST
DECOM2	BL	UDOT
;	BL	RFROM
	BL	DROP
	_UNNEST

;   WORDS	( -- )
;	Display the names in the context vocabulary.

	DCD	_DECOM-MAPOFFSET
_WORDS	DCB  5
	DCB	"WORDS"
	ALIGN	
WORDS
	_NEST
	BL	CR
	BL	CNTXT
	BL	AT			;only in context
WORS1
	BL	QDUP			;?at end of list
	BL	QBRAN
	DCD	WORS2-MAPOFFSET
	BL	DUPP
	BL	SPACE
	BL	DOTID			;display a name
	BL	CELLM
	BL	AT
	B.W	WORS1
WORS2
	_UNNEST
	ALIGN

;**************************************************************************
; cold start

;   VER	 ( -- n )
;	Return the version number of this implementation.

;	DCD	_WORDS-MAPOFFSET
;_VERSN	DCB  3
;	DCB	"VER"
;	ALIGN	
VERSN
	_NEST
	_DOLIT
	DCD	VER*256+EXT
	_UNNEST

;   hi	  ( -- )
;	Display the sign-on message of eForth.

	DCD	_WORDS-MAPOFFSET
_HI	DCB  2
	DCB	"HI"
	ALIGN	
HI
	_NEST
	BL	CR	;initialize I/O
	BL	DOTQP
	DCB	13
	DCB	"stm32eForth v"	;model
	ALIGN	
	BL	BASE
	BL	AT
	BL	HEX	;save radix
	BL	VERSN
	BL	BDIGS
	BL	DIG
	BL	DIG
	_DOLIT
	DCD	'.'
	BL	HOLD
	BL	DIGS
	BL	EDIGS
	BL	TYPEE	;format version number
	BL	BASE
	BL	STORE
	BL	CR
	_UNNEST			;restore radix

;   COLD	( -- )
;	The high level cold start sequence.

	DCD	_HI-MAPOFFSET
LASTN	DCB  4
	DCB	"COLD"
	ALIGN	
COLD
; Initiate Forth registers
	MOVW R3,#0xFF00		; user area
;	MOVT R3,#0x2000		; 
	MOV R2,R3  		; return stack
	SUB	R1,R2,#0x100 	; data stack
	MOV R5,#0			; tos
	NOP
	_NEST
COLD1
	_DOLIT
	DCD	UZERO-MAPOFFSET
	_DOLIT
	DCD	UPP
	_DOLIT
	DCD	ULAST-UZERO
	BL	MOVE 			;initialize user area
	BL	PRESE			;initialize stack and TIB
	BL	TBOOT
	BL	ATEXE			;application boot
	BL	OVERT
	B.W	QUIT			;start interpretation
	ALIGN
COLD2	
CTOP
	DCD	0XFFFFFFFF		; keep CTOP even
	END
