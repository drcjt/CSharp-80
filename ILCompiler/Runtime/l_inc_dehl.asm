l_inc_dehl:

   ; increment 32-bit value
   ;
   ; enter : dehl = 32 bit number
   ;
   ; exit  : dehl = dehl + 1
   ;
   ; uses  : f, de, hl

   inc l
   ret nz
   
   inc h
   ret nz
   
   inc de
   ret
