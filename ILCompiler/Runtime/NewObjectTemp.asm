; This routine allocates a new object with the specified size on top of the stack
;
; Uses: HL, DE, BC

; This is temporary till NewString and NewArray have been rewritten to not use this routine at all
; and to accept eetype to set as first 2 bytes of allocated space

NewObjectTemp:
	POP BC		; Save return address

	; get size of memory to allocate
	POP DE

	PUSH BC		; put return address back

	; Get next available heap address into HL & BC
	LD HL, (HEAPNEXT)
	PUSH HL
	POP BC

	; Move next available heap address by size of object to allocate
	ADD HL, DE

	; Check if Heap has collided with stack
	PUSH HL	

	PUSH HL
	POP DE		; DE = HeapNext

	LD HL, -100		; Need to leave bit of a gap
	ADD HL, SP		; HL = SP - 100

	AND A		; clear carry flag
	SBC HL, DE	; HL = (SP - 100) - HEAPNEXT
	JR C, NewObjectTemp_NoSpace

	POP HL
	LD (HEAPNEXT), HL	; Store new next available address in heap

	POP HL;		Get return address

	; Put address of memory allocated back on the stack
	PUSH BC		; allocated heap memory address

	PUSH HL;	put return address back
	RET

NewObjectTemp_NoSpace:

	LD HL, OOM_MSG - 2
	CALL PRINT

	; Panic exit
	JP EXIT
