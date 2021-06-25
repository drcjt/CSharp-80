; 16 bit signed not equal comparison
; DE != HL
; Carry set if true
s_neq:
	OR A
	SBC HL, DE
	SCF
	LD HL, 1
	RET NZ
	OR A
	DEC L
	RET
