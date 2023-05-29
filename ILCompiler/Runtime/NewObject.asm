; This routine allocates a new object using the EE Type pointer on top of the stack
;
; Uses: HL, DE, BC, IX

NewObject:	
	POP BC		; Save return address

	;get EE Type pointer into DE
	POP DE

	; Put address of allocated memory onto stack
	LD HL, (HEAPNEXT)
	PUSH HL

	PUSH BC		; put return address back

	PUSH IX		; Save IX

	PUSH DE		; Move DE into IX
	POP IX

	; Load base size into DE
	LD D, (IX+1)
	LD E, (IX+0)

	; Move next available heap address by size of object to allocate
	ADD HL, DE

	; Save HeapNext
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
	LD (HEAPNEXT), HL	;

	PUSH IX		; Move EE Type pointer into BC
	POP BC
	LD (HL), B	; Set EE Type pointer in newly allocated space
	INC HL
	LD (HL), C

	POP IX		; Restore IX

	RET

NewObject_NoSpace:

	LD HL, OOM_MSG - 2
	CALL PRINT

	; Panic exit
	JP EXIT
