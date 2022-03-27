; This routine adds two 16 bits numbers from the stack
; leaving the result on the stack as a 16 bit number
;
; Uses: HL, BC, DE


i_add16:
	POP DE		; Save return address

	POP HL		; First Arg
	POP BC		; Second Arg
	ADD HL, BC	; Add args

				; Put result back on stack
	PUSH HL		; LSW next

	PUSH DE
	RET
