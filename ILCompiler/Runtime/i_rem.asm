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

i_rem:
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
   jr z, i_rem_divide_zero  

   call l0_small_div_32_32x32

   exx
   push hl
   push de
   exx

   ex af, af'
   push af
   ret

i_rem_divide_zero:

    dec de
    scf
    ret
