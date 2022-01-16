; Fast keyboard test routine.
; Returns non-zero int32 on stack if key pressed

KEYAVAIL:
	POP IY	; return address

	LD HL, 0
	PUSH HL

	LD HL, (387FH)
	LD H, 0
	PUSH HL

	JP (IY)
