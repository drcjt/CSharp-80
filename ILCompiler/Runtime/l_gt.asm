; 32 bit signed greater than comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
l_gt:
	POP BC

	POP HL
	POP DE

	PUSH BC

	CALL l_cmp
	JR Z, l_gt_1

	CCF
	RET C

l_gt_1:
	DEC L
	RET
