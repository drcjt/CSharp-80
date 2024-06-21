; This routine allocates a new object using the EE Type pointer on top of the stack
;
; Uses: HL, DE, BC
;
; On Entry: BC = EETypePtr, DE = size to allocate
; On Exit: HL = pointer to allocated object

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

	PUSH HL

	; Set the new object's EETypePtr
	LD (HL), C
	INC HL
	LD (HL), B

	; Zero rest of allocated memory
	LD B, D
	LD C, E
	INC BC	; Skip over first 1 byte of EEType

NewObject_ZeroLoop:
	DEC BC
	LD A, B
	OR C
	JR Z, NewObject_ZeroLoopEnd

	INC HL
	LD (HL), 0
	JR NewObject_ZeroLoop

NewObject_ZeroLoopEnd:
	POP HL

	RET

AllocFailed:

	LD HL, OOM_MSG - 2
	CALL PRINT

	; Panic exit
	JP EXIT
