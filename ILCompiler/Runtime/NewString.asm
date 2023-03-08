; Create a new string of the specified length
;
; Uses: HL, DE, BC

NewString:	
	PUSH HL		; Length passed in HL
	POP BC

	PUSH BC		; Save original size

	INC BC		; Multiply size by 2 as using UTF-16 so 2 bytes per character
	SLA C
	RL B

	PUSH BC
	CALL NewObject	; Allocate object
	POP HL		; Address of new object

	POP BC		; Restore original size

	POP DE		; Get return address

	PUSH HL		; Return value is address of new string

	LD (HL), C	; Set the size for the new string in the first 2 bytes
	INC HL
	LD (HL), B

	PUSH DE		; Restore return address

	RET
