; 32 bit signed greater than comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
i_gt:
	POP BC

	POP HL
	POP DE

	PUSH BC

	CALL i_cmp
	RET C
	
	DEC L
	RET
