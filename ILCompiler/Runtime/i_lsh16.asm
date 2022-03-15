; This routine performs the operation HL = HL << C
;
; Uses: HL, BC, AF


i_lsh16:	
	POP IY

	POP HL

	POP BC

	LD A, C

	OR A
	JR Z, i_lsh_end

	LD B, A

i_lsh_loop:

	ADD HL, HL

	DJNZ i_lsh_loop


i_lsh_end:

	PUSH HL		; Put result back on stack

	JP (IY)
