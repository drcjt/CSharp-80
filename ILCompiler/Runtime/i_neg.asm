i_neg:
	pop iy	; save return address

	pop hl
	pop de
	call i_neg_dehl
	push de
	push hl

	jp (iy)

; negate dehl
;
; enter : dehl = long
;
; exit  : dehl = -long
;
; uses  : af, de, hl, carry unaffected

i_neg_dehl:
   ld a,l
   cpl
   ld l,a
   
   ld a,h
   cpl
   ld h,a
   
   ld a,e
   cpl
   ld e,a
   
   ld a,d
   cpl
   ld d,a
   
   inc l
   ret nz
   
   inc h
   ret nz
   
   inc de
   ret