; This routine returns the address of the EHClauses
; leaving the result on the stack as a 16 bit pointer
;
; Uses: HL, DE

EHENUMINIT:
	POP DE

	POP HL	; Address of EHEnum

	LD BC, EH_CLAUSES
	LD (HL), C
	INC HL
	LD (HL), B

	EX DE, HL
	JP (HL)