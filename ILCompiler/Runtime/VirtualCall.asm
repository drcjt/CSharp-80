; This routine performs a virtual call by dynamic dispatch through the VTable
;
; Uses: HL, DE, BC
;
; On Entry: BC = VTable offset

VirtualCall:	
	POP DE		; Return address
	POP HL		; This pointer

	PUSH HL		; Restore stack
	PUSH DE

	LD E, (HL)	; Get EEType Ptr into HL
	INC HL
	LD D, (HL)
	LD H, D
	LD L, E

VirtualCall_Internal:
	ADD HL, BC	; Add VTable slot offset to EEType Ptr

	LD E, (HL)	; Get entry in VTable slot into HL
	INC HL
	LD D, (HL)
	LD H, D
	LD L, E

	JP (HL)		; Dynamic dispatch
