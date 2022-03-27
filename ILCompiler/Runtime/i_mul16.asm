;==================================================
; MULTIPLY ROUTINE 16 bit
; takes bc, de from stack and works out dehl = bc * de
; Low word of result (hl) is placed back on the stack

i_mul16:
    exx
    pop hl                  ; save return address into hl'
    exx

    pop bc
    pop de

    ld hl, 0
    ld a, 16

i_mul16_1:
    add hl, hl
    rl e
    rl d
    jp nc, i_mul16_2
    inc de
i_mul16_2:
    dec a
    jp nz, i_mul16_1

    push hl
    
    exx
    PUSH HL
    EXX
    RET
