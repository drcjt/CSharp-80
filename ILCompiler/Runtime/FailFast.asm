; This routine takes an exception object and displays the message
; then exits the program
;
; Uses: HL, DE
;
; On Entry: On stack: exception object

FailFast:
	; Remove return address
	POP DE

	; exception object
	POP HL

	; Get message object from exception
	INC HL
	INC HL
	LD E, (HL)
	INC HL
	LD D, (HL)
	EX DE, HL

	; print exception message
	CALL PRINT

	; exit
	JP EXIT