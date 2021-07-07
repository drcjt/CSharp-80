; 16 bit signed greater than comparison
; DE > HL
; Carry set if true
s_gt:
	POP BC

	POP HL
	POP DE

	PUSH BC

	LD A, D
	ADD A, 80H
	LD B, A
   
	LD A, H
	ADD A, 80H
   
	CP B
	JP NZ, s_cmp
   
	LD A, L
	CP E
   
	JP s_cmp
