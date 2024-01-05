; Call a catch handler
;
; Uses: HL, DE
;
; On Entry: exception info, handler address, exception object all on stack

CALLCATCHHANDLER:
	POP HL		; Return address - discard

	; Restore IX
	POP HL		; exception info address
	LD E, (HL)
	INC HL
	LD D, (HL)
	INC HL
	LD C, (HL)
	INC HL
	LD B, (HL)

	; DE = SP
	; BC = frame pointer

	; Restore frame pointer
	PUSH BC
	POP IX

	; Move SP into HL
	LD H, D
	LD L, E

	POP DE		; handler address
	POP BC		; exception object

	; Restore stack pointer
	LD SP, HL

	; Push exception object
	PUSH BC

	; Jump to handler
	LD H, D
	LD L, E
	JP (HL)