; Call a finally funclet
;
; Uses: HL, DE, BC, IX
;
; On Entry: hanlder address, parent frame pointer

CALLFINALLYFUNCLET:
    POP BC        ; Return address
    POP DE        ; Parent Frame Pointer
    POP HL        ; handler address

    PUSH BC       ; Push return address for after funclet has executed
    PUSH IX       ; Save ExceptionHandling.InvokeSecondPass frame pointer

    PUSH DE
    POP IX        ; Set IX to parent frame pointer for finally block

    CALL JUMPTOHL   ; Call the finally funclet

    POP IX

    RET
