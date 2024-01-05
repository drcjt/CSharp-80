; This routine retrieves the next EH Clause
;
; Uses: HL, DE, BC
;
; On Entry: EHClause struct to populate, EHEnum struct, all on stack
; On Exit: bool on stack, false if reached end of clauses, true otherwise

EHENUMNEXT:
	POP BC	; Return Address

	POP DE	; Address of EHClause return struct
	POP HL	; Address of EHEnum

	PUSH BC		; return address
	PUSH HL		; address of ehenum

	LD C, (HL)
	INC HL
	LD B, (HL)
	LD H, B		; hl now is address of eh clause to get
	LD L, C

	LD B, D
	LD C, E

	LD DE, EH_CLAUSES_END
	OR A
	SBC HL, DE
	PUSH HL		; HL = 0 if reached end of clauses
	ADD HL, DE

	LD D, B
	LD E, C

	LD BC, 6
	LDIR

	LD D, H
	LD E, L

	POP BC
	POP HL
	LD (HL), E
	INC HL
	LD (HL), D

	POP DE

	PUSH BC
	PUSH BC

	PUSH DE

	RET