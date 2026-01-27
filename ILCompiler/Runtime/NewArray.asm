; This routine allocates space for 1-dimensional arrays in the heap
;
; Uses: HL, DE, BC, AF, HL'

; On entry on stack: EETypePtr, ElementCount in HL

NewArray:
	; Save return address
	POP AF
	EX AF, AF'

	; Get Element count into DE
	EX DE, HL

	; Get EETypePtr
	POP HL

	; Save EETypePtr & Element Count
	PUSH DE ; DE = Element Count
	PUSH HL	; HL = EETypePtr

	; Get Element Size from EEType
	LD C, (HL)
	INC HL
	LD B, (HL)

	; HL = EETypePtr
	; BC = Element Size
	; DE = Element Count

	; Multipy size (DE) by element size (BC)
	; Result is in HL
	LD A, B
	LD B, 16
NEWARR_MUL16LOOP:
	ADD HL, HL
	SLA C
	RLA
	JR NC, NEWARR_NOMUL16
	ADD HL, DE
NEWARR_NOMUL16:
	DJNZ NEWARR_MUL16LOOP

	; HL = size in bytes of elements
	; BC = Element Size
	; DE = 0

	POP BC	; EEType Ptr

	PUSH HL

	; Add base size to bytes to allocate
	LD HL, 6
	ADD HL, BC
	LD E, (HL)
	INC HL
	LD D, (HL)

	; Base size in DE

	; Add base size to element size * element count
	POP HL
	ADD HL, DE
	EX DE, HL

	; DE = size to allocate
	; BC = EETypePtr
	; Allocate object
	CALL NEWOBJECT

	POP HL	; ptr to new object
	POP DE	; element count
	PUSH HL

	; Skip past the EETypePtr
	INC HL
	INC HL

	; Set the new object's element count
	LD (HL), E
	INC HL
	LD (HL), D

	EX AF, AF'
	PUSH AF
	RET
