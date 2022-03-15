; This routine performs the operation HL = HL << C
;
; Uses: HL, BC, AF


i_lsh16:	
	POP IY

	POP HL

	POP BC

	LD A, C

	OR A
	JR Z, i_lsh16_end

	LD B, A

i_lsh16_loop:

	ADD HL, HL

	DJNZ i_lsh16_loop


i_lsh16_end:

	PUSH HL		; Put result back on stack

	JP (IY)
