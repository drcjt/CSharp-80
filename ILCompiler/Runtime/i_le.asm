; 32 bit signed less than equal comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
i_le:
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

	OR A
	RET
