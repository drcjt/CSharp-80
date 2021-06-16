; CSharp runtime routines

; 16 bit signed equal comparison
; DE == HL
; Carry set if true
EQL:
	OR A
	SBC HL, DE
	SCF
	INC HL
	RET Z
	XOR A
	LD L, A
	LD H, A
	RET

; 16 bit signed not equal comparison
; DE != HL
; Carry set if true
NOTEQ:
	OR A
	SBC HL, DE
	SCF
	LD HL, 1
	RET NZ
	OR A
	DEC L
	RET

; 16 bit signed greater than comparison
; DE > HL
; Carry set if true
GREATERTHAN:
	LD A, D
	ADD A, 80H
	LD B, A
   
	LD A, H
	ADD A, 80H
   
	CP B
	JP NZ, CP_RESULT
   
	LD A, L
	CP E
   
	JP CP_RESULT

; 16 bit signed greater than equal comparison
; DE >= HL
; Carry set if true
GREATERTHANEQ:
	LD A, H
	ADD A, 80H
	LD B, A
	LD A, D
	ADD A, 80H
	CP B
	CCF
	JP NZ, CP_RESULT
	LD A, E
	CP L
	CCF
	JP CP_RESULT

; 16 bit signed less than comparison
; DE < HL
; Carry set if true
LESSTHAN:
	LD A, H
	ADD A, 80H
	LD B, A
	LD A, D
	ADD A, 80H
	CP B
	JP NZ, CP_RESULT
	LD A, E
	CP L
	JP CP_RESULT

; 16 bit signed less than equal comparison
; DE <= HL
; Carry set if true
LESSTHANEQ:
	LD A, D
	ADD A, 80H
	LD B, A
	LD A, H
	ADD A, 80H
	CP B
	CCF
	JP NZ, CP_RESULT
	LD A, L
	CP E
	CCF
	JP CP_RESULT

CP_RESULT:   
	LD HL, 0
	RET NC
	INC HL
	RET

PRINT:
	LD A, (HL)
	CP 0
	JR Z, PRINTEND
	CALL 33H
	INC HL
	JR PRINT
PRINTEND:
	RET

; Write a character to current cursor position
; Top of stack contains character to write (low byte)
WRITE:
	POP BC	; return address
	POP HL
	PUSH BC	; put return address back
	LD A, L
	CALL 33H	; TODO - consider using JP instead
	RET

CLS:
	PUSH AF
	CALL 01C9H
	POP AF
	RET

SETXY:
	POP HL   ; return address
	POP BC   ; y 
	POP DE   ; x
	PUSH HL  ; restore return address
	LD HL, 4020H ; 
	LD (HL), E
	INC HL
	LD (HL), C
	RET

; display number in HL as ascii
NUM2DEC:
    push de
	push bc
	BIT 7, H
	JR Z, CONV

	LD A, '-'
	CALL 33H

	XOR A
	SUB L
	LD L, A
	LD A, 0
	SBC A, H
	LD H, A

CONV:
	LD B, 0
L1:	
	LD A, 10
	call div_hl_a
	push af
	inc b
	ld a, h
	or l
	jr nz, L1

L2:
	pop af
	or $30
	call 33h
	djnz L2

	pop bc
	pop de
	ret


; Divides HL by A, HL = HL / A, A = remainder
div_hl_a:
   push bc
   ld c, a
   xor	a
   ld	b, 16

_loop:
   add	hl, hl
   rla
   jr	c, $+5
   cp	c
   jr	c, $+4

   sub	c
   inc	l
   
   djnz	_loop
   
   pop bc

   ret
