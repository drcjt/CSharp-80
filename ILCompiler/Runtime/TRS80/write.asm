; Write a character to current cursor position
; Top of stack contains character to write (low byte)
WRITE:
	POP BC	; return address

	POP HL
	POP DE

	PUSH BC	; put return address back

	LD A, L
	JP 33H
