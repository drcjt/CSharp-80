;==================================================
; MULTIPLY ROUTINE 16 bit
; takes bc, de from stack and works out dehl = bc * de
; Low word of result (hl) is placed back on the stack

i_mul16:
    pop hl                  ; save return address into hl'
    exx

    pop bc
    pop de

    ld hl, 0
    ld a, 16

i_mul16_1:
    srl b
    rr c
    jr nc, i_mul16_2

    add hl, de

i_mul16_2:
    ex de,hl
    add hl, hl
    ex de, hl

    dec a

    jp nz, i_mul16_1

    push hl
    
    exx
    JP (HL)
