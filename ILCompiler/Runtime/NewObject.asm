; This routine allocates a new object using the EE Type pointer on top of the stack
;
; Uses: HL, DE, BC
;
; On Entry: BC = EETypePtr, DE = size to allocate
; On Exit: HL = pointer to allocated object

NewObjectNoSize:	

	; EETypePtr is in HL here
	; Get Base Size from EETypePtr into DE

	LD B, H
	LD C, L
	LD DE, 4	; Offset to BaseSize
	ADD HL, DE
	LD E, (HL)
	INC HL
	LD D, (HL)

NewObject:	

    ; Try to allocate memory from the heap
    CALL GCAlloc
    JR NC, NewOutOfMemory

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
	POP BC
	POP HL
	PUSH BC

	JP (HL)

NewOutOfMemory:

	LD HL, OOM_MSG - 2
	CALL PRINT

	; Panic exit
	JP EXIT
