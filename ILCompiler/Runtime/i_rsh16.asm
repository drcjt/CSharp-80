; This routine performs the operation HL = HL >> C
;
; Uses: HL, DE, BC, AF, AF'


i_rsh:	
	POP IY

	POP HL
	POP BC

	LD A, C

	OR A
	JR Z, i_rsh_end

	LD B, A

i_rsh_loop:

	RR H
	RR L

	DJNZ i_lsh_loop


i_rsh_end:

	POP BC

	PUSH HL		; Put result back on stack

	JP (IY)
