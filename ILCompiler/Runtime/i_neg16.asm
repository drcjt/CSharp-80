i_neg16:
	pop hl	; save return address

    pop de

    ld a, e
    cpl
    ld e, a
    ld a, d
    cpl
    ld d, a

    inc de

    push de

    jp (hl)
