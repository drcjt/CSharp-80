; Create a new string of the specified length
;
; Uses: HL, DE, BC

; TODO: Use this in readline too
STRING_BASE_SIZE		EQU	2

NewString:	
	PUSH HL		; Save original size

	; Compute overall size (align(base size + (element size * elements), 4))
	INC HL		; Multiply elements * element size
	SLA L
	RL H

	LD BC, STRING_BASE_SIZE		; Add base size
	ADD HL, BC

	PUSH HL
	CALL NewObjectTemp	; Allocate object
	POP HL		; Address of new object

	POP BC		; Restore original size

	POP DE		; Get return address

	PUSH HL		; Return value is address of new string
	
	INC HL		; Skip base size
	INC HL

	LD (HL), C	; Set the size for the new string in the first 2 bytes
	INC HL
	LD (HL), B

	PUSH DE		; Restore return address

	RET
