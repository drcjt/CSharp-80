s_mul:                           ; This routine performs the operation DEHL=BC*DE
	LD HL, 0
	LD A, 16
s_mul_loop:
	ADD HL, HL
	RL E
	RL D
	JP NC, s_mul_nomul
	ADD HL, BC
	JP NC, s_mul_nomul
	INC DE						; This instruction (with the jump) is like an "ADC DE,0"
s_mul_nomul:
	DEC A
	JP NZ, s_mul_loop
	RET
