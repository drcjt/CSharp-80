; 32 bit signed greater than comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
i_gt:
	POP BC

	POP HL
	POP DE

	PUSH BC

	CALL i_cmp
	JR Z, i_gt_1

	CCF
	RET C

i_gt_1:
	DEC L
	RET
