; This routine gets the details of the next stack frame
;
; Uses: HL, DE, BC
;
; On Entry: ref StackFrameIter struct
; On Exit: bool on stack, false if reached end of clauses, true otherwise

;   Stack frame looks like this
;
;   |                       |
;   |-----------------------|
;   |       incoming        |
;   |       arguments       |
;   |-----------------------|
;   |    return address     |
;   +=======================+
;   |     IX (optional)     |    Not present if no locals or params
;   |-----------------------|   <-- IX will point to here when method code executes
;   |    Local variables    |
;   |-----------------------|
;   |   Arguments for the   |
;   ~     next method       ~
;   |                       |
;   |      |                |
;   |      | Stack grows    |
;          | downward
;          V


SFINEXT:
	POP BC	; Return Address

	POP HL	; Address of StackFrameIter struct

	PUSH BC

	; Use IX to get previous IX value e.g. frame pointer
	; Use return address to get instruction pointer
	; Set SP to just above return address from stack

	PUSH HL

	; Skip stack pointer
	INC HL
	INC HL

	; Get current frame pointer
	LD E, (HL)
	INC HL
	LD D, (HL)

	LD H, D
	LD L, E
	; HL = current frame pointer
	
	; Get previous frame pointer
	LD E, (HL)
	INC HL
	LD D, (HL)

	; Get return address from stack frame
	INC HL
	LD C, (HL)
	INC HL
	LD B, (HL)
	INC HL

	; DE = new frame pointer
	; BC = new instruction pointer
	; HL = new stack pointer

	PUSH HL

	; Compare BC to EH_ENDIP
	LD HL, EH_ENDIP
	OR A
	SBC HL, BC
	JR Z, SFINEXT_ENDOFFRAMES
	ADD HL, BC

	POP AF		; new stack pointer
	POP HL		; address of stack frame iterator

	PUSH DE		; save new frame pointer

	PUSH AF		; put new stack pointer into DE
	POP DE

	LD (HL), E	; save new stack pointer into stack frame iterator
	INC HL
	LD (HL), D
	INC HL

	POP DE		; restore new frame pointer

	LD (HL), E	; save new frame pointer into stack frame iterator
	INC HL
	LD (HL), D
	INC HL

	LD (HL), C	; save new instruction pointer into stack frame iterator
	INC HL
	LD (HL), B

	JR SFINEXT_RETVALUE

SFINEXT_ENDOFFRAMES:
	POP DE
	POP DE

SFINEXT_RETVALUE:
	POP BC

	; Just return false for now
	LD DE, 0
	PUSH DE
	PUSH HL

	PUSH BC
	RET