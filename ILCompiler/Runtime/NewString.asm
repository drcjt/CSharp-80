; Create a new string of the specified length
;
; Uses: HL, DE, BC

; TODO: Use this in readline too
STRING_BASE_SIZE		EQU 4	; EE TYPE + SIZE = 4 bytes

NewString:	

	; On entry HL = original size, stack has EEType

	POP DE		; Return Address
	POP BC		; EEType
	PUSH DE		; Restore Return Address

	PUSH HL		; Original Size

	; Compute overall size (align(base size + (element size * elements), 4))	
	INC HL
	SLA L
	RL H

	; Add base string size
	LD DE, STRING_BASE_SIZE
	ADD HL, DE

	; Move size in bytes to DE
	LD D, H
	LD E, L

	; Allocate the string on the heap and set the EEType
	CALL NEWOBJECT

	POP HL		; Address of allocated string
	POP BC		; Original size

	POP DE		; Save return address

	PUSH HL		; Address of allocated string

	INC HL		; skip EE Type
	INC HL

	LD (HL), C	; Set the size for the new string
	INC HL
	LD (HL), B

	PUSH DE		; Restores return address

	RET