i_neg:
	pop bc	; save return address

	pop de
	pop hl
	call i_neg_dehl
	push hl
	push de

	push bc
	ret

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