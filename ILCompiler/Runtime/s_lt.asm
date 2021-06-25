; 16 bit signed less than comparison
; DE < HL
; Carry set if true
s_lt:
	LD A, H
	ADD A, 80H
	LD B, A
	LD A, D
	ADD A, 80H
	CP B
	JP NZ, s_cmp
	LD A, E
	CP L
	JP s_cmp
