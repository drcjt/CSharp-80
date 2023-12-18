; This routine captures the instruction pointer at the point
; an exception is thrown, the corresponding frame pointer for
; the containing method. These are packaged into an ExInfo struct
; and passed to the managed ThrowException code

;
; Uses: HL, DE, IX
;
; On Entry: On stack: exception object, return address 

ThrowEx:

	; POP HL
	; POP DE

	; Put exception object on stack
	; PUSH DE

	; Put ExInfo struct on stack
	; PUSH HL
	PUSH IX

	CALL THROWEXCEPTION
