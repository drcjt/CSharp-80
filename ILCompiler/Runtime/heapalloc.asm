; This routine performs the operation DEHL = DEHL + BCAF
;
; Uses: HL, DE


heapalloc:	
	POP IY		; Save return address

	; get size of memory to allocate
	POP DE		; LSW first

	; Get next available heap address into HL & BC
	LD HL, (HEAPNEXT)
	PUSH HL
	POP BC

	; Move next available heap address by size of object to allocate
	ADD HL, DE
	LD (HEAPNEXT), HL	; Store new next available address in heap

	; Put address of memory allocated back on the stack
	LD DE, 0
	PUSH DE		; MSW first	- always 0 as we only have 64k of addressable memory
	PUSH BC		; LSW next - allocated heap memory address

	JP (IY)