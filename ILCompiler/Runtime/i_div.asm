   ; signed division of 32-bit numbers
   ;
   ; enter : dehl = 32-bit divisor
   ;         dehl'= 32-bit dividend
   ;
   ; exit  : success
   ;
   ;            dehl = 32-bit quotient
   ;            dehl'= 32-bit remainder
   ;            carry reset
   ;
   ;         divide by zero
   ;
   ;            dehl = $ffffffff = ULONG_MAX
   ;            dehl'= dividend
   ;            carry set, errno = EDOM
   ;
   ; uses  : af, bc, de, hl, bc', de', hl'

i_div:
    pop af
    ex af, af'

    pop de
    pop hl
    exx
    pop de
    pop hl
    exx

   ld a,d
   or e
   or h
   or l
   jr z, divide_zero_s  

   call l0_small_div_32_32x32

   push hl
   push de

   ex af, af'
   push af
   ret

l0_small_div_32_32x32:

   ld a,d
   
   exx
   
   ld b,d                      ; b = MSB of dividend
   ld c,a                      ; c = MSB of divisor
   
   push bc                     ; save sign info

   bit 7,d
   call nz, i_neg_dehl         ; take absolute value of dividend

   exx

   bit 7,d
   call nz, i_neg_dehl         ; take absolute value of divisor

   call l0_small_divu_32_32x32

   ; dehl = unsigned quotient
   ; dehl'= unsigned remainder

   pop bc                      ; bc = sign info
   
   ld a,b
   xor c
   call m, i_neg_dehl          ; negate quotient if signs different
   
   bit 7,b
   jp z, i_div_end             ; if dividend > 0

   exx
   call i_neg_dehl             ; make remainder negative
   exx
   
   ; quotient = dehl
   ; remainder = dehl'

i_div_end:
    ret

divide_zero_s:

; TODO: this should be putting some values back on the stack too!
    dec de
    scf
    ret
