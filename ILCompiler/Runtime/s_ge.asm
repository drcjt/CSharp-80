; 16 bit signed greater than equal comparison
; DE >= HL
; Carry set if true
s_ge:
	POP BC

	POP HL
	POP DE

	PUSH BC

	LD A, H
	ADD A, 80H
	LD B, A
	LD A, D
	ADD A, 80H
	CP B
	CCF
	JP NZ, s_cmp
	LD A, E
	CP L
	CCF
	JP s_cmp
