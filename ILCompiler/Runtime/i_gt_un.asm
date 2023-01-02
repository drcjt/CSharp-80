; 32 bit unsigned greater than comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
i_gt_un:
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

	JR C, i_gt_un_1

	LD A, H
	OR L
	OR D
	OR E

i_gt_un_1:
	LD HL, 1

	JP NC, i_gt_un_nc
	RET

i_gt_un_nc:
	DEC L

	RET
