; Write a specific value into a block of memory
;
; Uses: HL, DE, BC, AF, BC'

Memset:	
	EXX
	POP BC		; Save return address
	EXX

	POP HL		; Destination address

	POP DE		; Initial value
	POP AF

	POP BC		; Count
	POP AF

	EXX
	PUSH BC		; put return address back
	EXX

	LD A, B
	OR C

	RET Z		; Nothing to do if count is zero

	LD A, E
	LD D, H
	LD E, L

	LD (HL), A
	INC DE
	DEC BC

	LD A, B
	OR C

	RET Z

	LDIR

	RET
