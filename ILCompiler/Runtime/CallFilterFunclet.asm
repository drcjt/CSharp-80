; Call a filter funclet
;
; Uses: HL, DE, BC, IX, BC'
;
; On Entry: hanlder address, parent frame pointer

CALLFILTERFUNCLET:
    EXX
    POP BC        ; Return address
    EXX
    POP DE        ; Parent Frame Pointer
    POP HL        ; filter address
    POP BC        ; Exception object

    EXX
    PUSH BC       ; Push return address for after funclet has executed
    EXX
    PUSH IX       ; Save ExceptionHandling.InvokeSecondPass frame pointer

    PUSH DE
    POP IX        ; Set IX to parent frame pointer for finally block

    LD DE, CALLFILTERFUNCLET_RETURN
    PUSH DE        ; Push return address for after filter has executed

    PUSH BC       ; Push exception object for filter

    JP (HL)        ; Call the filter funclet

CALLFILTERFUNCLET_RETURN:
    POP DE        ; Save bool result from filter
    POP BC

    POP IX

    POP HL

    PUSH BC        ; Push bool result from filter as return value
    PUSH DE

    JP (HL)
