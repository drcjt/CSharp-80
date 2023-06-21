; Allocate a new string
;
; Uses: HL, BC, DE
;
; On entry: HL = element count, EETypePtr on stack
; On exit: HL = pointer to allocated object

NewString:

	; Save return address
	POP AF
	EX AF, AF'

	; EETypePtr
	POP BC

	; Move element count to DE
	LD D, H
	LD E, L

	; Save element count
	PUSH HL

	; Compute overall size (base size + (element size * elements)) where element size = 2, base size = 4
	INC DE
	INC DE
	SLA E
	RL D

	; Allocate object
	CALL NEWOBJECT

	; HL = pointer to allocated object

	POP DE
	PUSH HL

	; Set the new object's element count
	INC HL
	INC HL
	LD (HL), E
	INC HL
	LD (HL), D

	EX AF, AF'
	PUSH AF		; Restore return address

	RET