DELAY:
	POP HL	; return address

	POP BC
	POP BC   ; delay required

DELAY1:			; should be 14.65 microseconds per loop
	DEC BC
	LD A, B
	OR C
	JR NZ, DELAY1

	PUSH HL
	RET
