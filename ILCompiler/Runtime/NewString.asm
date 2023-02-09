; Create a new string of the specified length
;
; Uses: HL, DE, BC

NewString:	
	POP HL		; Save return address

	POP BC		; Get size of string to allocate
	POP DE

	PUSH HL		; Save return address on stack

	PUSH BC		; Same original size

	INC BC
	SLA C
	RL B

	PUSH BC

	CALL HEAPALLOC	; Allocate object

	POP HL		; Address of new object as 32 bits
	POP BC

	POP BC		; Restore original size

	POP DE		; Get return address

	PUSH HL		; Return value is address of new string

	LD (HL), C	; Set the size for the new string in the first 2 bytes
	INC HL
	LD (HL), B

	PUSH DE		; Restore return address

	RET
