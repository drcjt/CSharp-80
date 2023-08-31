DELAY:
	POP HL	; return address

	POP BC   ; delay required

DELAY1:			; should be 14.65 microseconds per loop
	DEC BC
	LD A, B
	OR C
	JR NZ, DELAY1

	POP BC		; Remove msw of parameter - ignoring it

	JP (HL)
