; Copy data from source address to destination address
;
; Uses: HL, DE, BC, AF, BC'

Memcpy:
    POP AF      ; Save return address

	POP DE		; Destination address
	POP HL		; Source address
	POP BC		; Count

    PUSH AF     ; put return address back

	LD A, B
	OR C

	RET Z		; Nothing to do if count is zero

	LDIR 

	RET
