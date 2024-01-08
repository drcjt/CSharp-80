; This routine captures the instruction pointer, frame pointe and stack pointer
; at the point an exception is thrown. These are packaged into an ExInfo struct
; and passed to the managed ThrowException code along with the exception object

;
; Uses: HL, DE, BC, IX
;
; On Entry: On stack: exception object, return address 

ThrowEx:
	; Get return address
	POP DE

	; exception object
	POP BC

	; Create ExInfo on stack
	; First is ExInfo._exception object, initialise this to null
	LD HL, 0
	PUSH HL

	; Initialise ExInfo._frameIter.InstructionPointer
	DEC DE	; Return address is after call instruction so need to decrement by 1
	PUSH DE

	; Initialise ExInfo._frameIter.FramePointer
	PUSH IX	

	; Initialise ExInfo._frameIter.StackPointer
	LD HL, 6	; Accounts for _frameIter
	ADD HL, SP
	PUSH HL

	; Now setup arguments for ThrowException
	; First is exception object
;	PUSH BC

	; Now we have reference to ExInfo struct itself
	LD HL, 0
	ADD HL, SP
	PUSH HL

	CALL THROWEXCEPTION