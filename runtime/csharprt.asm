; CSharp runtime routines

; 16 bit signed equal comparison
; DE == HL
; Carry set if true
EQUAL:
   OR A
   SBC HL, DE
   SCF
   INC HL
   RET Z
   XOR A
   LD L, A
   LD H, A
   RET

; 16 bit signed not equal comparison
; DE != HL
; Carry set if true
NOTEQUAL:
   OR A
   SBC HL, DE
   SCF
   LD HL, 1
   RET NZ
   OR A
   DEC L
   RET

; 16 bit signed greater than comparison
; DE > HL
; Carry set if true
GREATERTHAN:
   LD A, D
   ADD A, 80H
   LD B, A
   
   LD A, H
   ADD A, 80H
   
   CP B
   JP NZ, CP_RESULT
   
   LD A, L
   CP E
   
   JP CP_RESULT

; 16 bit signed greater than equal comparison
; DE >= HL
; Carry set if true
   LD A, H
   ADD A, 80H
   LD B, A
   LD A, D
   ADD A, 80H
   CP B
   CCF
   JP NZ, CP_RESULT
   LD A, E
   CP L
   CCF
   JP CP_RESULT

; 16 bit signed less than comparison
; DE < HL
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

; 16 bit signed less than equal comparison
; DE <= HL
; Carry set if true
   LD A, D
   ADD A, 80H
   LD B, A
   LD A, H
   ADD A, 80H
   CP B
   CCF
   JP NZ, CP_RESULT
   LD A, L
   CP E
   CCF
   JP CP_RESULT

CP_RESULT:   
   LD HL, 0
   RET NC
   INC HL
   RET
