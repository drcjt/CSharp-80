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

	; Note new stack pointer doesn't account for any parameters passed on the stack
	; We deal with this as follows:
	;    1. Use new instruction pointer - 2 to get address of method we are unwinding, i.e. extract address from CALL instruction before return address
	;    2. Use address of method being unwound - 1 to get byte before method.
	;    3. The byte before the method will specify the number of parameters passed on the stack in bytes. So for 1 int parameter, the value would be 4.
	;    4. Increment the SP by the byte before the method
	
	; Save frame pointer, instruction pointer, stack pointer
	PUSH DE
	PUSH BC
	PUSH HL

	; Determine address of method being called that we are unwinding
	LD H, B
	LD L, C
	DEC HL
	LD B, (HL)
	DEC HL
	LD C, (HL)

	; Unwind information is immediately before the method
	DEC BC

	; Unwind information is the number of bytes for the parameters on the stack prior to the call
	LD A, (BC)

	LD D, 0
	LD E, A

	; Restore stack pointer and add bytes to remove parameters on the stack
	POP HL
	ADD HL, DE

	; Restore instruction pointer and frame pointer
	POP BC
	POP DE

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

	; Call instruction is 3 before return address
	DEC BC
	DEC BC
	DEC BC

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