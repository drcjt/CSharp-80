   ; unsigned division of 32-bit numbers
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

i_rem_un:
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
   jr z, i_rem_un_divide_zero  

   xor a
   push hl
   exx
   ld c,l
   ld b,h
   pop hl
   push de
   ex de,hl
   ld l,a
   ld h,a
   exx
   pop bc
   ld l,a
   ld h,a
   
   ; dede' = 32-bit divisor
   ; bcbc' = 32-bit dividend
   ; hlhl' = 0

   ld a,b
   ld b,32

i_rem_un_loop_0:

   exx
   rl c
   rl b
   exx
   rl c
   rla
   
   exx
   adc hl,hl
   exx
   adc hl,hl
   
   exx
   sbc hl,de
   exx
   sbc hl,de
   jr nc, i_rem_un_loop_1

   exx
   add hl,de
   exx
   adc hl,de

i_rem_un_loop_1:

   ccf
   djnz i_rem_un_loop_0

   exx
   rl c
   rl b
   exx
   rl c
   rla

   ; quotient  = acbc'
   ; remainder = hlhl'
   
   push hl
   exx
   pop de
   push bc
   exx
   pop hl
   ld e,c
   ld d,a

   ; quotient = dehl
   ; remainder = dehl'

   exx
   push hl
   push de
   exx

   ex af, af'
   push af
   ret

i_rem_un_divide_zero:

    dec de
    scf
    ret
