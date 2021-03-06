SETRES:
	EXX
	POP HL		; SAVE RETURN ADDRESS
	EXX

	POP HL		; Set or Reset
	POP DE		; Ignore msw

	POP DE		; Get y-coordinate into DE
	POP BC		; Ignore msw

	LD A, E		; Save y into A

	POP DE		; get x-coordinate into HL
	POP BC		; Ignore msw

	LD D, E		; Move x to D
	LD E, A		; Set E as y

	EXX
	PUSH HL		; push return address
	EXX

	LD A, L	; SET/RESET

	LD HL, SETRESDATA
	PUSH AF
	LD A, D		; X Coordinate
	PUSH AF
	LD A, E		; Y coordinate
	JP 150H
	
SETRESDATA:
	DB 29H
