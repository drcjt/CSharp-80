; This routine captures the instruction pointer at the point
; an exception is thrown, the corresponding frame pointer for
; the containing method. These are packaged into an ExInfo struct
; and passed to the managed ThrowException code

;
; Uses: HL, DE, IX
;
; On Entry: On stack: exception object, return address 

ThrowEx:
	; Get return address
	POP DE

	LD HL, 0		; HL = SP before Call to ThrowEx
	ADD HL, SP

	; Load ExInfo struct to stack, first is instructionPointer
	DEC DE	; Return address is after call instruction so need to decrement by 1
	PUSH DE

	; Second is frame pointer
	PUSH IX	

	; Finally we have SP but need to account for exception object
	INC HL
	INC HL
	PUSH HL

	CALL THROWEXCEPTION