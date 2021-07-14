; 32 bit signed greater than equal comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
l_ge:
	POP BC

	POP HL
	POP DE

	PUSH BC

	CALL l_cmp

	CCF
	RET C

	SCF
	RET Z

	DEC L
	CCF
	RET
