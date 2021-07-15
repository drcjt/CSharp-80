; 32 bit signed less than comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
i_lt:
	POP BC

	POP HL
	POP DE

	PUSH BC

	CALL i_cmp
	RET C
	
	DEC L
	RET
