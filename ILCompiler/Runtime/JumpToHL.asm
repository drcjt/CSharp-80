; Jumps to address in HL. Can be called to emulate a CALL (HL) instruction
;
; Uses: HL
;
; On Entry: Not applicable

JUMPTOHL:
    JP (HL)
