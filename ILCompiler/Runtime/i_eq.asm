; 32 bit signed equal comparison
; Args from Stack, DEHL == BCAF
; Carry set if true

i_eq:
	POP HL
	EXX 

	POP HL			; LSW
	POP DE			; MSW

	POP BC			; LSW
	AND A			; Clear zero flag
	SBC HL, BC
	JR NZ, i_eq_1

	EX DE, HL
	POP BC
	SBC HL, BC
	JR NZ, i_eq_2

	SCF				; set carry flag

	JP i_eq_3

i_eq_1:

	POP BC			; MSW

i_eq_2:
	XOR A			; Clear carry flag

i_eq_3:
	EXX
	PUSH HL
	RET
