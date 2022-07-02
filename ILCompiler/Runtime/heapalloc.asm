; This routine performs the operation DEHL = DEHL + BCAF
;
; Uses: HL, DE

HEAPNEXT:	DB '  '

heapalloc:	
	POP BC		; Save return address

	; get size of memory to allocate
	POP DE		; LSW first

	PUSH BC		; put return address back

	; Get next available heap address into HL & BC
	LD HL, (HEAPNEXT)
	PUSH HL
	POP BC

	; Move next available heap address by size of object to allocate
	ADD HL, DE
	LD (HEAPNEXT), HL	; Store new next available address in heap

	POP HL;		Get return address

	; Put address of memory allocated back on the stack
	LD DE, 0
	PUSH DE		; MSW first	- always 0 as we only have 64k of addressable memory
	PUSH BC		; LSW next - allocated heap memory address

	PUSH HL;	put return address back

	RET
