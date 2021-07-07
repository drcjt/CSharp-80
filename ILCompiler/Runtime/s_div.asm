; This routine performs the operation HL = HL / DE
; quotient placed in DE
; Remainder placed in HL
;
; Uses: DE, HL, AF, BC, AF'

s_div:
	POP AF			; Save return address
	EX AF, AF'

	POP DE			; Get values to divide
	POP HL

	LD A, H
	LD C, L
	LD HL, 0
	LD B, 16

s_div_loop:
	SLL C
	RLA
	ADC HL, HL
	SBC HL, DE
	JR NC, $+4
	ADD HL, DE
	DEC C

	DJNZ s_div_loop   

	LD D, A
	LD E, C

	PUSH DE			; Push result

	EX AF, AF'		; Restore return address
	PUSH AF

	RET
