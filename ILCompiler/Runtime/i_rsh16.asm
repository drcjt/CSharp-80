; This routine performs the operation HL = HL >> C
;
; Uses: HL, DE, BC, AF, AF'


i_rsh16:	
	POP IY

	POP HL
	POP BC

	LD A, C

	OR A
	JR Z, i_rsh16_end

	LD B, A

i_rsh16_loop:

	RR H
	RR L

	DJNZ i_rsh16_loop


i_rsh16_end:

	POP BC

	PUSH HL		; Put result back on stack

	JP (IY)
