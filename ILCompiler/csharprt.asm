DELAY:
	POP HL	; return address

	POP BC   ; delay required

DELAY1:			; should be 14.65 microseconds per loop
	DEC BC
	LD A, B
	OR C
	JR NZ, DELAY1

	POP BC		; Remove msw of parameter - ignoring it

	JP (HL)
; This routine calculates the heap memory used
;
; Uses: HL, DE, BC

GCGetTotalMemory:	
	POP BC		; Save return address

	LD HL, 0	; MSW
	PUSH HL

	; Calculate memory used on heap
	LD HL, (HEAPNEXT)
	LD DE, HEAP
	SBC HL, DE
	PUSH HL		; LSW

	PUSH BC
	RET
; display number in HL as ascii
ITOA:
	PUSH IX

	BIT 7, H
	JR Z, CONV2

	LD A, '-'
    CALL PRINTCHR

	XOR A
	SUB L
	LD L, A
	LD A, 0
	SBC A, H
	LD H, A
    JP CONV2

UITOA:
    PUSH IX

CONV2:

	CALL B2D16
	CALL UTF8PRINT
	POP IX
	RET

; Converts unsigned int in DE:HL to ascii and prints it
LTOA:
    PUSH AF
	PUSH BC
	PUSH DE
	PUSH IX

	BIT 7, D        ; Test if long is negative
	JR Z, LTOA2

    LD A, '-'       ; Print -ve sign
    CALL PRINTCHR

    LD A, L         ; invert dehl
    CPL
    LD L, A
    LD A, H
    CPL
    LD H, A
    LD A, E
    CPL
    LD E, A
    LD A, D
    CPL
    LD D, A

    call l_inc_dehl     ; inc dehl

    JP LTOA2

ULTOA:
    PUSH AF
	PUSH BC
	PUSH DE
	PUSH IX

LTOA2:
	CALL B2D32
	CALL UTF8PRINT
	POP IX
	POP DE
	POP BC
	POP AF
	RET
	
; Combined routine for conversion of different sized binary numbers into
; directly printable ASCII(Z)-string
; Input value in registers, number size and -related to that- registers to fill
; is selected by calling the correct entry:
;
;  entry  inputregister(s)  decimal value 0 to:
;   B2D8             A                    255  (3 digits)
;   B2D16           HL                  65535   5   "
;   B2D24         E:HL               16777215   8   "
;   B2D32        DE:HL             4294967295  10   "
;   B2D48     BC:DE:HL        281474976710655  15   "
;   B2D64  IX:BC:DE:HL   18446744073709551615  20   "
;
; The resulting string is placed into a small buffer attached to this routine,
; this buffer needs no initialization and can be modified as desired.
; The number is aligned to the right, and leading 0's are replaced with spaces.
; On exit HL points to the first digit, (B)C = number of decimals
; This way any re-alignment / postprocessing is made easy.
; Changes: AF,BC,DE,HL,IX

; by Alwin Henseler

B2D8:    LD H,0
         LD L,A
B2D16:   LD E,0
B2D24:   LD D,0
B2D32:   LD BC,0
B2D48:   LD IX,0          ; zero all non-used bits
B2D64:   LD (B2DINV),HL
         LD (B2DINV+2),DE
         LD (B2DINV+4),BC
         LD (B2DINV+6),IX ; place full 64-bit input value in buffer
         LD HL,B2DBUF
         LD DE,B2DBUF+1
         LD (HL)," "
B2DFILC: EQU $-1         ; address of fill-character
         LD BC,18
         LDIR            ; fill 1st 19 bytes of buffer with spaces
         LD (B2DEND-1),BC ;set BCD value to "0" & place terminating 0
         LD E,1          ; no. of bytes in BCD value
         LD HL,B2DINV+8  ; (address MSB input)+1
         LD BC,0909H
         XOR A
B2DSKP0: DEC B
         JR Z,B2DSIZ     ; all 0: continue with postprocessing
         DEC HL
         OR (HL)         ; find first byte <>0
         JR Z,B2DSKP0
B2DFND1: DEC C
         RLA
         JR NC,B2DFND1   ; determine no. of most significant 1-bit
         RRA
         LD D,A          ; byte from binary input value
B2DLUS2: PUSH HL
         PUSH BC
B2DLUS1: LD HL,B2DEND-1  ; address LSB of BCD value
         LD B,E          ; current length of BCD value in bytes
         RL D            ; highest bit from input value -> carry
B2DLUS0: LD A,(HL)
         ADC A,A
         DAA
         LD (HL),A       ; double 1 BCD byte from intermediate result
         DEC HL
         DJNZ B2DLUS0    ; and go on to double entire BCD value (+carry!)
         JR NC,B2DNXT
         INC E           ; carry at MSB -> BCD value grew 1 byte larger
         LD (HL),1       ; initialize new MSB of BCD value
B2DNXT:  DEC C
         JR NZ,B2DLUS1   ; repeat for remaining bits from 1 input byte
         POP BC          ; no. of remaining bytes in input value
         LD C,8          ; reset bit-counter
         POP HL          ; pointer to byte from input value
         DEC HL
         LD D,(HL)       ; get next group of 8 bits
         DJNZ B2DLUS2    ; and repeat until last byte from input value
B2DSIZ:  LD HL,B2DEND    ; address of terminating 0
         LD C,E          ; size of BCD value in bytes
         OR A
         SBC HL,BC       ; calculate address of MSB BCD
         LD D,H
         LD E,L
         SBC HL,BC
         EX DE,HL        ; HL=address BCD value, DE=start of decimal value
         LD B,C          ; no. of bytes BCD
         SLA C           ; no. of bytes decimal (possibly 1 too high)
         LD A,"0"
         RLD             ; shift bits 4-7 of (HL) into bit 0-3 of A
         CP "0"          ; (HL) was > 9h?
         JR NZ,B2DEXPH   ; if yes, start with recording high digit
         DEC C           ; correct number of decimals
         INC DE          ; correct start address
         JR B2DEXPL      ; continue with converting low digit
B2DEXP:  RLD             ; shift high digit (HL) into low digit of A
B2DEXPH: LD (DE),A       ; record resulting ASCII-code
         INC DE
B2DEXPL: RLD
         LD (DE),A
         INC DE
         INC HL          ; next BCD-byte
         DJNZ B2DEXP     ; and go on to convert each BCD-byte into 2 ASCII
         SBC HL,BC       ; return with HL pointing to 1st decimal
         RET

B2DINV:  DS 8            ; space for 64-bit input value (LSB first)
B2DBUF:  DS 20           ; space for 20 decimal digits
B2DEND:  DS 1            ; space for terminating 0

UTF8PRINT:
	LD A, (HL)
	CP 0
	JR Z, UTF8PRINTEND
    CALL PRINTCHR
	INC HL
	JR UTF8PRINT
UTF8PRINTEND:
	RET
; This routine performs the operation DEHL = DEHL + BCAF
;
; Uses: HL, HL', DE, BC, AF


i_add:
	EXX			; Save return address
	POP HL
	EXX

	POP HL		; LSW first
	POP DE		; MSW next

	POP BC
	ADD HL, BC	; Add LSW
	EX DE, HL

	POP BC		; Add MSW
	ADC HL, BC
	EX DE, HL	

				; Put result back on stack
	PUSH DE		; MSW first
	PUSH HL		; LSW next

	EXX
	PUSH HL
	EXX
	RET
; This routine adds two 16 bits numbers from the stack
; leaving the result on the stack as a 16 bit number
;
; Uses: HL, BC, DE


i_add16:
	POP DE		; Save return address

	POP HL		; First Arg
	POP BC		; Second Arg
	ADD HL, BC	; Add args

				; Put result back on stack
	PUSH HL		; LSW next

	PUSH DE
	RET
; Logical And of two 32 bit values on stack
; Leaves result on the stack
;
; Uses: HL, HL', DE, BC, AF

i_and:
	EXX
	POP HL
	EXX

	POP HL
	POP DE

	POP BC

	LD A, H
	AND B
	LD H, A

	LD A, L
	AND C
	LD L, A

	POP BC

	LD A, D
	AND B
	LD D, A

	LD A, E
	AND C
	LD E, A

	PUSH DE
	PUSH HL

	EXX
	PUSH HL
	EXX
	RET
; Logical And of two 16 bit values on stack
; Leaves result on the stack
;
; Uses: HL, DE, BC, AF

i_and16:
	POP BC

	POP HL
	POP DE

	LD A, H
	AND D
	LD H, A

	LD A, L
	AND E
	LD L, A

	PUSH HL
	
	PUSH BC
	RET; 32 bit comparison routine
;
; Entry dehl = secondary
;       on stack = primary (under two return addresses)
;
; Exit: z = number is zero
;       nz = number is non zero
;       c = number is negative
;       nc = number is positive

i_cmp:
	POP BC		; Save return address 1
	EXX

	POP BC		; Save return address 2

	POP HL		; LSW
	POP DE		; MSW

	PUSH BC		;  restore return address 2
	LD A, L

	EXX

	PUSH BC		; restore return address 1

	SUB L
	LD L, A

	EXX
	LD A, H
	EXX

	SBC A, H
	LD H, A

	EXX
	LD A, E
	EXX

	SBC A, E
	LD E, A

	EXX 
	LD A, D
	EXX

	SBC A, D
	LD D, A

	ADD A, A
	JR C, i_cmp_1

	LD A, H
	OR L
	OR D
	OR E

i_cmp_1:
	LD HL, 1
	RET
   ; signed division of 32-bit numbers
   ;
   ; enter : dehl = 32-bit divisor
   ;         dehl'= 32-bit dividend
   ;
   ; exit  : success
   ;
   ;            dehl = 32-bit quotient
   ;            dehl'= 32-bit remainder
   ;            carry reset
   ;
   ;         divide by zero
   ;
   ;            dehl = $ffffffff = ULONG_MAX
   ;            dehl'= dividend
   ;            carry set, errno = EDOM
   ;
   ; uses  : af, bc, de, hl, bc', de', hl'

i_div:
    pop bc

    exx
    pop hl      ; LSW
    pop de      ; MSW
    exx
    pop hl      ; LSW
    pop de      ; MSW

    push bc

   ld a,d
   or e
   or h
   or l
   jr z, divide_zero_s  

   call l0_small_div_32_32x32

   pop bc

   push de      ; MSW
   push hl      ; LSW

   push bc
   ret

l0_small_div_32_32x32:

   ld a,d
   
   exx
   
   ld b,d                      ; b = MSB of dividend
   ld c,a                      ; c = MSB of divisor
   
   push bc                     ; save sign info

   bit 7,d
   call nz, i_neg_dehl         ; take absolute value of dividend

   exx

   bit 7,d
   call nz, i_neg_dehl         ; take absolute value of divisor

   call l0_small_divu_32_32x32

   ; dehl = unsigned quotient
   ; dehl'= unsigned remainder

   pop bc                      ; bc = sign info
   
   ld a,b
   xor c
   call m, i_neg_dehl          ; negate quotient if signs different
   
   bit 7,b
   jp z, i_div_end             ; if dividend > 0

   exx
   call i_neg_dehl             ; make remainder negative
   exx
   
   ; quotient = dehl
   ; remainder = dehl'

i_div_end:
    ret

divide_zero_s:

; TODO: this should be putting some values back on the stack too!
    dec de
    scf

    ret
   ; unsigned division of 32-bit numbers
   ;
   ; enter : dehl = 32-bit divisor
   ;         dehl'= 32-bit dividend
   ;
   ; exit  : success
   ;
   ;            dehl = 32-bit quotient
   ;            dehl'= 32-bit remainder
   ;            carry reset
   ;
   ;         divide by zero
   ;
   ;            dehl = $ffffffff = ULONG_MAX
   ;            dehl'= dividend
   ;            carry set, errno = EDOM
   ;
   ; uses  : af, bc, de, hl, bc', de', hl'

i_div_un:
    pop bc

    exx
    pop hl      ; LSW
    pop de      ; MSW
    exx
    pop hl      ; LSW
    pop de      ; MSW

    push bc

   ld a,d
   or e
   or h
   or l
   jr z, divide_zero  

   call l0_small_divu_32_32x32

   pop bc

   push de      ; MSW
   push hl      ; LSW

   push bc
   ret

l0_small_divu_32_32x32:

   xor a
   push hl
   exx
   ld c,l
   ld b,h
   pop hl
   push de
   ex de,hl
   ld l,a
   ld h,a
   exx
   pop bc
   ld l,a
   ld h,a

 l1_small_divu_32_32x32:
   
   ; dede' = 32-bit divisor
   ; bcbc' = 32-bit dividend
   ; hlhl' = 0

   ld a,b
   ld b,32

loop_0:

   exx
   rl c
   rl b
   exx
   rl c
   rla
   
   exx
   adc hl,hl
   exx
   adc hl,hl
   
   exx
   sbc hl,de
   exx
   sbc hl,de
   jr nc, loop_1

   exx
   add hl,de
   exx
   adc hl,de

loop_1:

   ccf
   djnz loop_0

   exx
   rl c
   rl b
   exx
   rl c
   rla

   ; quotient  = acbc'
   ; remainder = hlhl'
   
   push hl
   exx
   pop de
   push bc
   exx
   pop hl
   ld e,c
   ld d,a

   ret

divide_zero:

; TODO: this should be putting some values back on the stack too!
    dec de
    scf
    ret
; 32 bit signed equal comparison
; Args from Stack, DEHL == BCAF
; Carry set if true

i_eq:
	POP HL
	EXX 

	POP HL			; LSW
	POP DE			; MSW

	POP BC			; LSW
	AND A			; Clear zero flag
	SBC HL, BC
	JR NZ, i_eq_1

	EX DE, HL
	POP BC
	SBC HL, BC
	JR NZ, i_eq_2

	SCF				; set carry flag

	JP i_eq_3

i_eq_1:

	POP BC			; MSW

i_eq_2:
	XOR A			; Clear carry flag

i_eq_3:
	EXX
	PUSH HL
	RET
; 16 bit signed equal comparison
; Args from Stack, DEHL == BCAF
; Carry set if true

i_eq16:
	POP BC

	POP HL
	POP DE

	AND A			; Clear zero flag
	SBC HL, DE
	JR NZ, i_eq16_1

	SCF				; set carry flag

	JP i_eq16_2

i_eq16_1:
	XOR A			; Clear carry flag

i_eq16_2:
	PUSH BC
	RET; 32 bit signed greater than equal comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
i_ge:
	POP BC

	POP HL
	POP DE

	PUSH BC

	CALL i_cmp

	RET C

	SCF
	RET Z

	DEC L
	CCF
	RET
; 32 bit signed greater than comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
i_gt:
	POP BC

	POP HL
	POP DE

	EXX

	POP HL		; LSW
	POP DE		; MSW

	LD A, L

	EXX

	PUSH BC

	SUB L
	LD L, A

	EXX
	LD A, H
	EXX

	SBC A, H
	LD H, A

	EXX
	LD A, E
	EXX

	SBC A, E
	LD E, A

	EXX 
	LD A, D
	EXX

	SBC A, D
	LD D, A

	ADD A, A
	JR C, i_gt_1

	LD A, H
	OR L
	OR D
	OR E

i_gt_1:
	LD HL, 1

	JP NC, i_gt_nc
	RET

i_gt_nc:
	DEC L

	RET
; 32 bit unsigned greater than comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
i_gt_un:
	POP BC

	POP HL
	POP DE

	EXX

	POP HL		; LSW
	POP DE		; MSW

	LD A, L

	EXX

	PUSH BC

	SUB L
	LD L, A

	EXX
	LD A, H
	EXX

	SBC A, H
	LD H, A

	EXX
	LD A, E
	EXX

	SBC A, E
	LD E, A

	EXX 
	LD A, D
	EXX

	SBC A, D
	LD D, A

	JR C, i_gt_un_1

	LD A, H
	OR L
	OR D
	OR E

i_gt_un_1:
	LD HL, 1

	JP NC, i_gt_un_nc
	RET

i_gt_un_nc:
	DEC L

	RET
; 32 bit signed less than equal comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
i_le:
	POP BC

	POP HL
	POP DE

	PUSH BC

	CALL i_cmp
	
	CCF
	RET C
	
	SCF
	RET Z

	DEC L

	OR A
	RET
; This routine performs the operation DEHL = DEHL << C
;
; Uses: HL, HL', DE, BC, AF


i_lsh:	
	EXX
	POP HL
	EXX

	POP HL
	POP DE

	POP BC

	LD A, C

	OR A
	JR Z, i_lsh_end

	LD B, A
	LD A, E

i_lsh_loop:

	ADD HL, HL
	RLA
	RL D

	DJNZ i_lsh_loop

	LD E, A

i_lsh_end:

	POP BC

	PUSH DE		; Put result back on stack
	PUSH HL

	EXX
	PUSH HL
	EXX
	RET
; This routine performs the operation HL = HL << C
;
; Uses: HL, BC, AF, DE


i_lsh16:	
	POP DE

	POP HL

	POP BC

	LD A, C

	OR A
	JR Z, i_lsh16_end

	LD B, A

i_lsh16_loop:

	ADD HL, HL

	DJNZ i_lsh16_loop


i_lsh16_end:

	PUSH HL		; Put result back on stack

	PUSH DE
	RET
; 32 bit signed less than comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
i_lt:
	POP BC

	POP HL
	POP DE

	EXX

	POP HL		; LSW
	POP DE		; MSW

	LD A, L

	EXX

	PUSH BC

	SUB L
	LD L, A

	EXX
	LD A, H
	EXX

	SBC A, H
	LD H, A

	EXX
	LD A, E
	EXX

	SBC A, E
	LD E, A

	EXX 
	LD A, D
	EXX

	SBC A, D
	LD D, A

	ADD A, A
	JR C, i_lt_1

	LD A, H
	OR L
	OR D
	OR E

i_lt_1:
	LD HL, 1

	JR Z, i_lt_nc

	CCF
	JP NC, i_lt_nc
	RET

i_lt_nc:
	DEC L
	RET
; 32 bit unsigned less than comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
i_lt_un:
	POP BC

	POP HL
	POP DE

	EXX

	POP HL		; LSW
	POP DE		; MSW

	LD A, L

	EXX

	PUSH BC

	SUB L
	LD L, A

	EXX
	LD A, H
	EXX

	SBC A, H
	LD H, A

	EXX
	LD A, E
	EXX

	SBC A, E
	LD E, A

	EXX 
	LD A, D
	EXX

	SBC A, D
	LD D, A

	JR C, i_lt_un_1

	LD A, H
	OR L
	OR D
	OR E

i_lt_un_1:
	LD HL, 1

	JR Z, i_lt_un_nc

	CCF
	JP NC, i_lt_un_nc
	RET

i_lt_un_nc:
	DEC L
	RET
; 16 bit signed less than comparison
; Args from Stack, HL < DE
; Carry set if true

i_lt_un16:
	POP BC

	POP DE
	POP HL

	AND A			; Clear zero flag
	SBC HL, DE

	JR C, i_lt_un16_1

	JR Z, i_lt_un16_1

	SCF				; set carry flag

	JP i_lt_un16_2

i_lt_un16_1:
	XOR A			; Clear carry flag

i_lt_un16_2:
	PUSH BC
	RET
;==================================================
; MULTIPLY ROUTINE 32*32BIT=32BIT
; H'L'HL = B'C'BC * D'E'DE
; NEEDS REGISTER A, CHANGES FLAGS
;

i_mul:
    pop hl                  ; save return address into hl

                            ; populate bc from stack as lsw, b'c' as msw
    pop bc                  ; from stack
    exx
    pop bc

    exx                     ; populate de from stack as lsw, d'e' as msw
    pop de                  ; from stack
    exx
    pop de
    exx

    push hl

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

    pop bc

    exx
    push hl                 ; put h'l'hl
    exx                     ; onto stack
    push hl                 ; as result

    push bc
    ret
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
    PUSH HL
    EXX
    RET
i_neg:
	pop bc	; save return address

	pop hl
	pop de
	call i_neg_dehl
	push de
	push hl

	push bc
	ret

; negate dehl
;
; enter : dehl = long
;
; exit  : dehl = -long
;
; uses  : af, de, hl, carry unaffected

i_neg_dehl:
   ld a,l
   cpl
   ld l,a
   
   ld a,h
   cpl
   ld h,a
   
   ld a,e
   cpl
   ld e,a
   
   ld a,d
   cpl
   ld d,a
   
   inc l
   ret nz
   
   inc h
   ret nz
   
   inc de
   ret; 32 bit signed not equals comparison
; Args from Stack, DEHL != BCAF
; Carry set if true

i_neq:
	POP HL
	EXX

	POP HL
	POP DE

	POP BC
	LD A, C
	CP L
	JR NZ, i_neq_1

	LD A, B
	CP H
	JR NZ, i_neq_1

	POP BC

	LD A, C
	CP E
	JR NZ, i_neq_2

	LD A, B
	CP D
	JR NZ, i_neq_2

	JP i_neq_3

i_neq_1:

	POP BC

i_neq_2:
	SCF

i_neq_3
	EXX
	PUSH HL
	RET
; 16 bit signed not equals comparison
; Args from Stack, DEHL != BCAF
; Carry set if true

i_neq16:
	POP BC		; Save return address

	POP HL
	POP DE

	LD A, L
	CP E
	JR NZ, i_neq16_1

	LD A, H
	CP D
	JR NZ, i_neq16_1

	JP i_neq16_2

i_neq16_1:
	SCF

i_neq16_2
	PUSH BC
	RET
; Logical Not of one 32 bit values on stack
; Leaves result on the stack
;
; Uses: HL, DE, BC, AF

i_not:
	POP BC

	POP HL
	POP DE

	LD A, L
	CPL
	LD L, A

	LD A, H
	CPL
	LD H, A

	LD A, E
	CPL
	LD E, A

	LD A, D
	CPL
	LD D, A
   
	PUSH DE
	PUSH HL

	PUSH BC
	RET
; Logical Or of two 32 bit values on stack
; Leaves result on the stack
;
; Uses: HL, HL', DE, BC, AF

i_or:
	EXX
	POP HL
	EXX

	POP HL
	POP DE

	POP BC

	LD A, H
	OR B
	LD H, A

	LD A, L
	OR C
	LD L, A

	POP BC

	LD A, D
	OR B
	LD D, A

	LD A, E
	OR C
	LD E, A

	PUSH DE
	PUSH HL

	EXX
	PUSH HL
	EXX
	RET
; Logical Or of two 16 bit values on stack
; Leaves result on the stack
;
; Uses: HL, DE, BC, AF

i_or16:
	POP BC

	POP HL
	POP DE

	LD A, H
	OR D
	LD H, A

	LD A, L
	OR E
	LD L, A

	PUSH HL

	PUSH BC
	RET
   ; unsigned division of 32-bit numbers
   ;
   ; enter : dehl = 32-bit divisor
   ;         dehl'= 32-bit dividend
   ;
   ; exit  : success
   ;
   ;            dehl = 32-bit quotient
   ;            dehl'= 32-bit remainder
   ;            carry reset
   ;
   ;         divide by zero
   ;
   ;            dehl = $ffffffff = ULONG_MAX
   ;            dehl'= dividend
   ;            carry set, errno = EDOM
   ;
   ; uses  : af, bc, de, hl, bc', de', hl'

i_rem:
    pop bc

    exx
    pop hl
    pop de
    exx
    pop hl
    pop de

    push bc

   ld a,d
   or e
   or h
   or l
   jr z, i_rem_divide_zero  

   call l0_small_div_32_32x32

   pop bc

   exx
   push de
   push hl
   exx

   push bc
   ret

i_rem_divide_zero:

    dec de
    scf
    ret
   ; unsigned division of 32-bit numbers
   ;
   ; enter : dehl = 32-bit divisor
   ;         dehl'= 32-bit dividend
   ;
   ; exit  : success
   ;
   ;            dehl = 32-bit quotient
   ;            dehl'= 32-bit remainder
   ;            carry reset
   ;
   ;         divide by zero
   ;
   ;            dehl = $ffffffff = ULONG_MAX
   ;            dehl'= dividend
   ;            carry set, errno = EDOM
   ;
   ; uses  : af, bc, de, hl, bc', de', hl'

i_rem_un:
    pop bc

    exx
    pop hl
    pop de
    exx
    pop hl
    pop de

    push bc

   ld a,d
   or e
   or h
   or l
   jr z, i_rem_un_divide_zero  

   call l0_small_divu_32_32x32

   pop bc

   exx
   push de
   push hl
   exx

   push bc

   ret

i_rem_un_divide_zero:

    dec de
    scf
    ret
; This routine performs the operation DEHL = DEHL >> C
;
; Uses: HL, HL', DE, BC, AF


i_rsh:	
	EXX
	POP HL
	EXX

	POP HL
	POP DE

	POP BC

	LD A, C

	OR A
	JR Z, i_rsh_end

	LD B, A
	LD A, E

i_rsh_loop:

	SRA D
	RRA
	RR H
	RR L

	DJNZ i_rsh_loop

	LD E, A

i_rsh_end:

	POP BC

	PUSH DE		; Put result back on stack
	PUSH HL

	EXX 
	PUSH HL
	EXX
	RET
; This routine performs the operation HL = HL >> C
;
; Uses: HL, HL', DE, BC, AF


i_rsh16:	
	EXX
	POP HL
	EXX

	POP HL
	POP BC

	LD A, C

	OR A
	JR Z, i_rsh16_end

	LD B, A

i_rsh16_loop:

	RR H
	RR L

	DJNZ i_rsh16_loop


i_rsh16_end:

	POP BC

	PUSH HL		; Put result back on stack

	EXX
	PUSH HL
	EXX
	RET
; This routine performs the operation DEHL = DEHL - BCAF
;
; Uses: HL, HL', DE, BC, AF

i_sub:	
	EXX			; Save return address
	POP HL
	EXX

	POP HL		; LSW
	POP DE		; MSW

	OR A		; Clear carry
	POP BC		; LSW
	SBC HL, BC

	POP BC		; MSW
	EX DE, HL
	SBC HL, BC
	EX DE, HL

	; Put result back on stack
	PUSH DE		; MSW
	PUSH HL		; LSW

	EXX
	PUSH HL
	EXX
	RET
l_inc_dehl:

   ; increment 32-bit value
   ;
   ; enter : dehl = 32 bit number
   ;
   ; exit  : dehl = dehl + 1
   ;
   ; uses  : f, de, hl

   inc l
   ret nz
   
   inc h
   ret nz
   
   inc de
   ret
; This routine allocates space for 1-dimensional arrays in the heap
;
; Uses: HL, DE

NewArray:	
	EXX
	POP HL		; Save return address
	EXX

	POP DE		; element size
	POP HL

	POP BC		; array size
	POP HL

	; Put array size into heap at next available location
	LD HL, (HEAPNEXT)
	LD (HL), C
	INC HL
	LD (HL), B

	EXX
	PUSH HL
	EXX 

	; Multipy size (DE) by element size (BC)
	LD A, 16
	LD HL, 0

newarr_mul16loop:
	ADD HL, HL
	rl e
	rl d
	jp nc, newarr_nomul16
	add hl, bc
	jp nc, newarr_nomul16
	inc de
newarr_nomul16:
	dec a
	jp nz, newarr_mul16loop

	; hl now has size of array to be allocated
	; Add 2 bytes to hold the array size
	inc hl
	inc hl

	; Get next available heap address into DE & BC
	LD DE, (HEAPNEXT)

	PUSH DE
	POP BC

	; Move next available heap address by size of object to allocate
	ADD HL, DE

	; Check if Heap has collided with stack
	PUSH HL	

	PUSH HL
	POP DE		; DE = HeapNext

	LD HL, -100		; Need to leave bit of a gap
	ADD HL, SP		; HL = SP - 100

	AND A		; clear carry flag
	SBC HL, DE	; HL = (SP - 100) - HEAPNEXT
	JR C, newarr_oom

	POP HL
	LD (HEAPNEXT), HL	; Store new next available address in heap

	POP HL;		Get return address

	; Put address of memory allocated back on the stack
	PUSH BC		; allocated heap memory address

	PUSH HL;	put return address back

	RET

newarr_oom:
	LD HL, OOM_MSG - 2
	CALL PRINT

	; Panic exit
	JP EXIT
; This routine allocates a new object with the specified size on top of the stack
;
; Uses: HL, DE, BC

NewObject:	
	POP BC		; Save return address

	; get size of memory to allocate
	POP DE

	PUSH BC		; put return address back

	; Get next available heap address into HL & BC
	LD HL, (HEAPNEXT)
	PUSH HL
	POP BC

	; Move next available heap address by size of object to allocate
	ADD HL, DE

	; Check if Heap has collided with stack
	PUSH HL	

	PUSH HL
	POP DE		; DE = HeapNext

	LD HL, -100		; Need to leave bit of a gap
	ADD HL, SP		; HL = SP - 100

	AND A		; clear carry flag
	SBC HL, DE	; HL = (SP - 100) - HEAPNEXT
	JR C, NewObject_NoSpace

	POP HL
	LD (HEAPNEXT), HL	; Store new next available address in heap

	POP HL;		Get return address

	; Put address of memory allocated back on the stack
	PUSH BC		; allocated heap memory address

	PUSH HL;	put return address back
	RET

NewObject_NoSpace:

	LD HL, OOM_MSG - 2
	CALL PRINT

	; Panic exit
	JP EXIT
; Create a new string of the specified length
;
; Uses: HL, DE, BC

; TODO: Use this in readline too
STRING_BASE_SIZE		EQU	2

NewString:	
	PUSH HL		; Save original size

	; Compute overall size (align(base size + (element size * elements), 4))
	INC HL		; Multiply elements * element size
	SLA L
	RL H

	LD BC, STRING_BASE_SIZE		; Add base size
	ADD HL, BC

	PUSH HL
	CALL NewObject	; Allocate object
	POP HL		; Address of new object

	POP BC		; Restore original size

	POP DE		; Get return address

	PUSH HL		; Return value is address of new string
	
	INC HL		; Skip base size
	INC HL

	LD (HL), C	; Set the size for the new string in the first 2 bytes
	INC HL
	LD (HL), B

	PUSH DE		; Restore return address

	RET
PRINT:
	LD BC, 2		; Add base size
	ADD HL, BC

	LD E, (HL)	; Get string length into DE
	INC HL
	LD D, (HL)
	INC HL

	LD B, E		; Mystery fast loop calculus
	DEC DE
	INC D

PRINTLOOP:
	LD A, (HL)
	CALL PRINTCHR
	INC HL			; Chars are utf-16 so skip 2 bytes
	INC HL

	DJNZ PRINTLOOP
	DEC D
	JP NZ, PRINTLOOP

	RET
; This routine widens a 16 bit signed int to a 32 bit signed int
;
; Uses: HL, DE, BC


stoi:	
	POP BC		; Save return address

	POP DE

	LD H, D
	ADD HL, HL
	SBC HL, HL

	PUSH HL
	PUSH DE

	PUSH BC
	RET