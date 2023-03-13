; This routine allocates space for 1-dimensional arrays in the heap
;
; Uses: HL, DE

NewArray:	
	EXX
	POP HL		; Save return address
	EXX

	POP DE		; element size
	POP HL

	POP BC		; array size
	POP HL

	; Put array size into heap at next available location
	LD HL, (HEAPNEXT)
	LD (HL), C
	INC HL
	LD (HL), B

	EXX
	PUSH HL
	EXX 

	; Multipy size (DE) by element size (BC)
	LD A, 16
	LD HL, 0

newarr_mul16loop:
	ADD HL, HL
	rl e
	rl d
	jp nc, newarr_nomul16
	add hl, bc
	jp nc, newarr_nomul16
	inc de
newarr_nomul16:
	dec a
	jp nz, newarr_mul16loop

	; hl now has size of array to be allocated
	; Add 2 bytes to hold the array size
	inc hl
	inc hl

	; Get next available heap address into DE & BC
	LD DE, (HEAPNEXT)

	PUSH DE
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
	JR C, newarr_oom

	POP HL
	LD (HEAPNEXT), HL	; Store new next available address in heap

	POP HL;		Get return address

	; Put address of memory allocated back on the stack
	PUSH BC		; allocated heap memory address

	PUSH HL;	put return address back

	RET

newarr_oom:
	LD HL, OOM_MSG - 2
	CALL PRINT

	; Panic exit
	JP EXIT
