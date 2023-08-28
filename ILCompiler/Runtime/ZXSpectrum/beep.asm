BEEP:
	POP BC		; return address

	POP DE		; Duration
	POP HL

	POP HL		; Frequency
	POP AF

	PUSH BC
	PUSH IX

	; Freq in HL
	; Duration in DE

	call 03B5H	; ROM Beeper routine

	POP IX
	RET