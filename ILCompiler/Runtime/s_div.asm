; Divide HL by DE
; quotient placed in DE
; Remainder placed in HL
s_div:
   ld   a, h
   ld   c, l
   ld	hl, 0
   ld	b, 16

s_div_loop:
   sll	c
   rla
   adc	hl, hl
   sbc	hl, de
   jr	nc, $+4
   add	hl, de
   dec	c
   
   djnz	s_div_loop
   
   ld d, a
   ld e, c

   ret
