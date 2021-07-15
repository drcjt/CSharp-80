;==================================================
; MULTIPLY ROUTINE 32*32BIT=32BIT
; H'L'HL = B'C'BC * D'E'DE
; NEEDS REGISTER A, CHANGES FLAGS
;

i_mul:
    pop af                  ; save return address in a'f'
    ex af, af'

    exx                     ; populate b'c'bc
    pop bc                  ; from stack
    exx
    pop bc

    exx                     ; populate d'e'de
    pop de                  ; from stack
    exx
    pop de

    AND     A               ; RESET CARRY FLAG
    SBC     HL,HL           ; LOWER RESULT = 0
    EXX
    SBC     HL,HL           ; HIGHER RESULT = 0
    LD      A,B             ; MPR IS AC'BC
    LD      B,32            ; INITIALIZE LOOP COUNTER
i_mul_1:
    SRA     A               ; RIGHT SHIFT MPR
    RR      C
    EXX
    RR      B
    RR      C               ; LOWEST BIT INTO CARRY
    JR      NC, i_mul_2
    ADD     HL,DE           ; RESULT += MPD
    EXX
    ADC     HL,DE
    EXX
i_mul_2:
    SLA     E               ; LEFT SHIFT MPD
    RL      D
    EXX
    RL      E
    RL      D
    DJNZ    i_mul_1
    EXX

    push hl                 ; put h'l'hl
    exx                     ; onto stack
    push hl                 ; as result
    exx

    ex af, af'
    push af
       
    RET
