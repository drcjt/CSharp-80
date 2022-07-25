; This routine allocates space for 1-dimensional arrays in the heap
;
; Uses: HL, DE

newarr:	
	EXX
	POP HL		; Save return address
	EXX

	POP DE		; element size
	POP HL

	POP BC		; array size
	POP HL

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
	LD (HEAPNEXT), HL	; Store new next available address in heap

	POP HL;		Get return address

	; Put address of memory allocated back on the stack
	LD DE, 0
	PUSH DE		; MSW first	- always 0 as we only have 64k of addressable memory
	PUSH BC		; LSW next - allocated heap memory address

	PUSH HL;	put return address back

	RET
