; 16 bit signed less than equal comparison
; DE <= HL
; Carry set if true
s_le:
	LD A, D
	ADD A, 80H
	LD B, A
	LD A, H
	ADD A, 80H
	CP B
	CCF
	JP NZ, s_cmp
	LD A, L
	CP E
	CCF
	JP s_cmp
