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
	CALL DIV_HL_A
	PUSH AF
	INC B
	LD A, H
	OR L
	JR NZ, L1

L2:
	POP AF
	OR 30H
	CALL 33H
	DJNZ L2

	POP BC
	POP DE
	RET

; Divides HL by A, HL = HL / A, A = remainder
DIV_HL_A:
	PUSH BC
	
	LD C, A
	XOR A
	LD B, 16

_LOOP:
	ADD HL, HL
	RLA
	JR C, $+5
	CP C
	JR C, $+4

	SUB C
	INC L

	DJNZ _LOOP

	POP BC	
	RET
 
MUL16:                           ; This routine performs the operation DEHL=BC*DE
	LD HL, 0
	LD A, 16
MUL16LOOP:
	ADD HL, HL
	RL E
	RL D
	JP NC, NOMUL16
	ADD HL, BC
	JP NC, NOMUL16
	INC DE						; This instruction (with the jump) is like an "ADC DE,0"
NOMUL16:
	DEC A
	JP NZ, MUL16LOOP
	RET

; display number in HL as ascii
NUM2DEC2:
	PUSH IX

	BIT 7, H
	JR Z, CONV2

	LD A, '-'
	CALL 33H

	XOR A
	SUB L
	LD L, A
	LD A, 0
	SBC A, H
	LD H, A

CONV2:

	CALL B2D16
	CALL PRINT
	POP IX
	RET

; Converts unsigned int in DE:HL to ascii and prints it
LTOA:
    PUSH AF
	PUSH BC
	PUSH DE
    PUSH IY
	PUSH IX
	CALL B2D32
	CALL PRINT
	POP IX
	POP IY
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
