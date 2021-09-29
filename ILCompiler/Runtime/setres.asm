SETRES:
	POP BC		; SAVE RETURN ADDRESS

	POP HL		; Set or Reset
	POP HL

	POP DE		; Get y-coordinate into DE
	POP DE

	LD A, E		; Save y into A

	POP DE		; get x-coordinate into HL
	POP DE

	LD D, E		; Move x to D
	LD E, A		; Set E as y

	PUSH BC		; push return address

	LD a, L	; SET/RESET

	LD HL, SETRESDATA
	PUSH AF
	LD A, D		; X Coordinate
	PUSH AF
	LD A, E		; Y coordinate
	JP 150H
	
SETRESDATA:
	DB 29H
