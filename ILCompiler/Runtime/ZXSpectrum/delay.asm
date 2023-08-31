DELAY:
	POP HL	; return address

	POP BC   ; delay required

DELAY1:
	HALT	; Wait for NMI to fire every 1/20th second
	DEC BC
	LD A, B
	OR C
	JR NZ, DELAY1

	POP BC		; Remove msw of parameter - ignoring it

	JP (HL)
