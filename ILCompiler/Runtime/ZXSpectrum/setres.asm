P_FLAG	EQU	23697

SETRES:

	EXX
	POP HL		; SAVE RETURN ADDRESS
	EXX

	POP HL		; Set or Reset
	POP DE		; Ignore msw

	POP DE		; Get y-coordinate into DE
	POP BC		; Ignore msw

	LD A, 175	; Save 175-y into A
	SUB E

	POP DE		; get x-coordinate into HL
	POP BC		; Ignore msw

	LD D, E		; Move x to D
	LD E, A		; Set E as y

	EXX
	PUSH HL		; push return address
	EXX

	LD A, L	; SET/RESET

	LD HL, P_FLAG
	LD (HL), A

	LD B, E
	LD C, D

	CALL 22E5H

	RET
	