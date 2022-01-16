;==================================================
; MULTIPLY ROUTINE 32*32BIT=32BIT
; H'L'HL = B'C'BC * D'E'DE
; NEEDS REGISTER A, CHANGES FLAGS
;

i_mul:
    pop iy                  ; save return address into iy

                            ; populate bc from stack as lsw, b'c' as msw
    pop bc                  ; from stack
    exx
    pop bc

    exx                     ; populate de from stack as lsw, d'e' as msw
    pop de                  ; from stack
    exx
    pop de
    exx

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

    exx
    push hl                 ; put h'l'hl
    exx                     ; onto stack
    push hl                 ; as result

    JP (IY)
