; 32 bit signed less than equal comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
l_le:
	POP BC

	POP HL
	POP DE

	PUSH BC

	CALL l_cmp
	RET C
	
	SCF
	RET Z

	DEC L

	OR A
	RET
