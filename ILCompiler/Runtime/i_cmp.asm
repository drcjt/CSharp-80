; 32 bit comparison routine
;
; Entry dehl = secondary
;       on stack = primary (under two return addresses)
;
; Exit: z = number is zero
;       nz = number is non zero
;       c = number is negative
;       nc = number is positive

i_cmp:
	POP BC
	EXX

	POP BC

	POP HL
	POP DE

	PUSH BC
	LD A, L

	EXX

	PUSH BC

	SUB L
	LD L, A

	EXX
	LD A, H
	EXX

	SBC A, H
	LD H, A

	EXX
	LD A, E
	EXX

	SBC A, E
	LD E, A

	EXX 
	LD A, D
	EXX

	SBC A, D
	LD D, A

	ADD A, A
	JR C, i_cmp_1

	LD A, H
	OR L
	OR D
	OR E

i_cmp_1:
	LD HL, 1
	RET
