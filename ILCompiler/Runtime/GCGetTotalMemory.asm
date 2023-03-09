; This routine calculates the heap memory used
;
; Uses: HL, DE, BC

GCGetTotalMemory:	
	POP BC		; Save return address

	LD HL, 0	; MSW
	PUSH HL

	; Calculate memory used on heap
	LD HL, (HEAPNEXT)
	LD DE, HEAP
	SBC HL, DE
	PUSH HL		; LSW

	PUSH BC
	RET
