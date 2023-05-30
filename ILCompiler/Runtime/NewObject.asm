; This routine allocates a new object using the EE Type pointer on top of the stack
;
; Uses: HL, DE, BC
;
; On entry: BC = EETypePtr, DE = size to be allocated

NewObject:	
	; Put address of allocated memory onto stack
	LD HL, (HEAPNEXT)
	PUSH HL

	; Move next available heap address by size of object to allocate
	ADD HL, DE
	PUSH HL

	; Check if Heap has collided with stack
	PUSH HL
	POP DE		; DE = HeapNext

	LD HL, -100		; Need to leave bit of a gap
	ADD HL, SP		; HL = SP - 100

	AND A		; clear carry flag
	SBC HL, DE	; HL = (SP - 100) - HEAPNEXT
	JR C, NewObject_NoSpace

	; Store new next available address in HEAPNEXT	
	POP HL
	LD (HEAPNEXT), HL

	; Swap address of allocated memory and return address
	POP HL	; Address of allocated memory
	POP DE	; Return address
	PUSH HL
	PUSH DE

	LD (HL), B	; Set EE Type pointer in newly allocated space
	INC HL
	LD (HL), C

	RET

NewObject_NoSpace:

	LD HL, OOM_MSG - 2
	CALL PRINT

	; Panic exit
	JP EXIT
