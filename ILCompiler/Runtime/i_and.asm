; This rout; Logical And of two 32 bit values on stack
; Leaves result on the stack
;
; Uses: HL, DE, BC, AF, IY

i_and:
	pop iy

	pop hl
	pop de

	pop bc

	ld a, h
	and b
	ld h, a

	ld a, l
	and c
	ld l, a

	pop bc

	ld a, d
	and b
	ld d, a

	ld a, e
	and c
	ld e, a

	push de
	push hl

	jp (iy)