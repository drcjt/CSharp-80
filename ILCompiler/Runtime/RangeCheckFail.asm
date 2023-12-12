; This routine displays the out of range message and then exitsk
;
; Uses: HL

RangeCheckFail:
	LD HL, INDEX_OUT_OF_RANGE_MSG - 2
	CALL PRINT
	JP EXIT
