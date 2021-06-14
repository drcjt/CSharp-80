; CSharp runtime routines

; 16 bit signed equal comparison
; DE == HL
; Carry set if true
EQ:
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
NOTEQ:
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
GREATERTHANEQ:
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
LESSTHANEQ:
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

; Write a character to current cursor position
; Top of stack contains character to write (low byte)
WRITE:
   POP BC	; return address
   POP HL
   PUSH BC	; put return address back
   LD A, L
   CALL 33H
   RET
