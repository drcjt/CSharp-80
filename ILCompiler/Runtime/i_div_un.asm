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

i_div_un:
    pop bc

    exx
    pop hl      ; LSW
    pop de      ; MSW
    exx
    pop hl      ; LSW
    pop de      ; MSW

    push bc

   ld a,d
   or e
   or h
   or l
   jr z, divide_zero  

   call l0_small_divu_32_32x32

   pop bc

   push de      ; MSW
   push hl      ; LSW

   push bc
   ret

l0_small_divu_32_32x32:

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

 l1_small_divu_32_32x32:
   
   ; dede' = 32-bit divisor
   ; bcbc' = 32-bit dividend
   ; hlhl' = 0

   ld a,b
   ld b,32

loop_0:

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
   jr nc, loop_1

   exx
   add hl,de
   exx
   adc hl,de

loop_1:

   ccf
   djnz loop_0

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

   ret

divide_zero:
    jp ThrowDivideByZeroException
