; 32 bit signed less than comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
l_lt:
	POP BC

	POP HL
	POP DE

	PUSH BC

	CALL l_cmp
	RET C
	
	DEC L
	RET
