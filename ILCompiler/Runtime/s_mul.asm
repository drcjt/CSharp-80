; This routine performs the operation DEHL = BC * DE
;
; Uses: HL, AF, DE, BC, AF'

s_mul:
	POP AF				; Save return address to AF'
	EX AF, AF'

	POP DE
	POP BC

	LD HL, 0
	LD A, 16
s_mul_loop:
	ADD HL, HL
	RL E
	RL D
	JP NC, s_mul_nomul
	ADD HL, BC
	JP NC, s_mul_nomul
	INC DE				; This instruction (with the jump) is like an "ADC DE,0"
s_mul_nomul:
	DEC A
	JP NZ, s_mul_loop

	PUSH HL				; Put result back on stack

	EX AF, AF'			; Restore return address
	PUSH AF

	RET
