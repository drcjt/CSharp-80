SCR_CT	EQU 23692	; Scroll Count

PRINTCHR:
	RST 0x10

	; Reset Scroll count to prevent scroll? message
	LD A, 255
	LD (SCR_CT), A

	RET
