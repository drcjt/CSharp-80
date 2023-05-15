;==================================================
; MULTIPLY ROUTINE 16 bit
; takes bc, de from stack and works out dehl = bc * de
; Low word of result (hl) is placed back on the stack

i_mul16:
    pop hl                  ; save return address into hl'
    exx

    pop bc
    pop de

    ld a, b
    ld b, 16

i_mul16_1:
    add hl, hl
    sla c
    rla
    jr nc, i_mul16_2

    add hl, de

i_mul16_2:
    djnz i_mul16_1

    push hl
    
    exx
    JP (HL)
