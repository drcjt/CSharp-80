; CSharp runtime routines

; 16 bit signed less than comparison
; HL < DE
; Carry set if true
LESSTHAN:
   LD A, H
   ADD A, 80H
   LD B, A
   LD A, D
   ADD A, 80H
   CP B
   JP NZ, CP_RESULT
   LD A, E
   CP L
   JP CP_RESULT

CP_RESULT:   
   LD HL, 0
   RET NC
   INC HL
   RET
