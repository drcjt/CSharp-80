; Copy data from source address to destination address
;
; Uses: HL, DE, BC, AF, BC'

Memcpy:	
	EXX
	POP BC		; Save return address
	EXX

	POP HL		; Source address
	POP DE		; Destination address
	POP BC		; Count
	POP AF

	EXX
	PUSH BC		; put return address back
	EXX

	LD A, B
	OR C

	RET Z		; Nothing to do if count is zero

	LDIR 

	RET
