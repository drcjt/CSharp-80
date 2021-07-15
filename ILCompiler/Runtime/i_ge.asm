; 32 bit signed greater than equal comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
i_ge:
	POP BC

	POP HL
	POP DE

	PUSH BC

	CALL i_cmp

	CCF
	RET C

	SCF
	RET Z

	DEC L
	CCF
	RET
