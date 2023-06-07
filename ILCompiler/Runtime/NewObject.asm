; This routine allocates a new object using the EE Type pointer on top of the stack
;
; Uses: HL, DE, BC
;
; On Entry: BC = EETypePtr, DE = size to allocate
; On Exit: HL = pointer to allocated object

OFFSET_BASESIZE		EQU 0

NewObject:	

	; Allocate and check if hit stack

	LD HL, (HEAPNEXT)
	ADD HL, DE
	OR A
	SBC HL, SP
	JR NC, AllocFailed
	ADD HL, SP

	LD (HEAPNEXT), HL

	; Calculate new object pointer
	OR A
	SBC HL, DE

	; Set the new object's EETypePtr
	LD (HL), C
	INC HL
	LD (HL), B
	DEC HL

	RET

AllocFailed:

	LD HL, OOM_MSG - 2
	CALL PRINT

	; Panic exit
	JP EXIT
