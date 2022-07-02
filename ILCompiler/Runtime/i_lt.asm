; 32 bit signed less than comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
i_lt:
	POP BC

	POP HL
	POP DE

	EXX

	POP HL		; LSW
	POP DE		; MSW

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
	JR C, i_lt_1

	LD A, H
	OR L
	OR D
	OR E

i_lt_1:
	LD HL, 1

	JR Z, i_lt_nc

	CCF
	JP NC, i_lt_nc
	RET

i_lt_nc:
	DEC L
	RET
