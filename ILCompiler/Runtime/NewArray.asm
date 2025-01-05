; This routine allocates space for 1-dimensional arrays in the heap
;
; Uses: HL, DE

; On entry on stack: EETypePtr, element size, element count

NewArray:
	; Save return address
	POP AF
	EX AF, AF'

	; EETypePtr
	POP HL

	; Element Size
	POP BC
	POP AF

	; Element Count
	POP DE
	POP AF

	; Save EETypePtr & Element Count
	PUSH DE
	PUSH HL

	; HL = EETypePtr
	; BC = Element Size
	; DE = Element Count

	; Multipy size (DE) by element size (BC)
	LD A, 16
	LD HL, 0
newarr_mul16loop:
	ADD HL, HL
	RL E
	RL D
	JP NC, newarr_nomul16
	ADD HL, BC
	JP NC, newarr_nomul16
	INC DE
newarr_nomul16:
	DEC A
	JP NZ, newarr_mul16loop

	; HL = size in bytes of elements
	; BC = Element Size
	; DE = 0

	; Add 4 to HL to cater for EETypePtr and element count
	LD DE, 4
	ADD HL, DE

	LD D, H
	LD E, L

	POP BC

	; DE = size to allocate
	; BC = EETypePtr
	; Allocate object
	CALL NEWOBJECT

	POP HL	; ptr to new object
	POP DE
	PUSH HL

	; Set the new object's element count
	INC HL
	INC HL
	LD (HL), E
	INC HL
	LD (HL), D

	EX AF, AF'
	PUSH AF		; Restore return address

	RET