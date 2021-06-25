; Write a character to current cursor position
; Top of stack contains character to write (low byte)
WRITE:
	POP BC	; return address
	POP HL
	PUSH BC	; put return address back
	LD A, L
	CALL 33H	; TODO - consider using JP instead
	RET
