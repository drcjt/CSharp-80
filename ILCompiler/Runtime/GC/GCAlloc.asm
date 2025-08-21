; Allocate memory from the heap
;
; Uses: HL, DE
; On Entry: DE = size to allocate
; On Exit:
;   if allocation successful: HL = pointer to allocated memory, HEAPNEXT updated, Carry Set
;   If allocation fails     : Carry not set

GCAlloc:

    LD HL, (HEAPNEXT)

	ADD HL, DE
	OR A
	SBC HL, SP

	RET NC

	ADD HL, SP

	LD (HEAPNEXT), HL
    SCF
    RET
