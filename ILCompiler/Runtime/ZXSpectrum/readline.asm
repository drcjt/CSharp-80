READKEYPRESS:
	PUSH HL
	LD HL, 0x5C08
	LD A, 255
	LD (HL), A
AWAITKEY:
	CP (HL)
	JR Z, AWAITKEY
	LD A, (HL)
	POP HL
	RET

; read a line (upto 240 characters) of text

READLINE:
	LD HL, INPUTBUF
	LD B, 240
	LD C, B

READNEXTKEY:
	CALL READKEYPRESS
	CP 13
	JR Z, READOVER

	LD (HL), A
	LD A, B
	OR A
	JR Z, READNEXTKEY

	LD A, (HL)
	INC HL

	; DISPLAY TYPED CHARACTER
	PUSH DE
	RST 0x10
	POP DE

	DEC B
	JR  READNEXTKEY

READOVER:
	LD A, 0DH	; Display carriage return
	PUSH DE
	RST 0x10
	POP DE

	LD A, C		; calculate number of entered characters
	SUB B
	LD B, A

	LD HL, INPUTBUF
	
	; B is string length
	; HL is string buffer

	; Now need to heap alloc a proper dotnet string and copy characters
	; String has 2 bytes for length, followed by characters which are utf-16 so 2 bytes per character

	; Put actual entered string length into BC
	LD C, B
	LD B, 0

	PUSH BC	; Save actual entered string size

	; Required size = (actual size + 1) * 2, as we need 2 initial bytes tos hold the length
	; and then each character itself will need 2 bytes as we are using utf-16
	INC BC	; Add 1
	SLA C	; Multiply size by 2
	RL B

	; Allocate string object on heap
	PUSH BC
	CALL NewObject

	POP HL		; Pointer to heap allocated string
	POP BC		; Length of string
	POP DE		; Return Address

	PUSH DE		; Save return address
	PUSH HL		; Save pointer to string object on heap

	INC HL		; Add base size
	INC HL

	; Put length into first 2 bytes of string object
	LD (HL), C
	INC HL
	LD (HL), 0
	INC HL

	; Copy characters from input buffer to heap allocated string object
	LD DE, INPUTBUF
COPYCHAR:
	LD A, (DE)
	LD (HL), A		; Character from input buffer goes first
	INC HL
	LD (HL), 0		; Extra byte as dotnet chars as utf-16
	INC HL

	INC DE			; Move to next char in input buffer
	DEC C
	JP NZ, COPYCHAR	; Continue copying till all chars done

	POP HL		; Pointer to heap allocated string
	POP DE		; Return address

	LD BC, 0

	PUSH BC		; MSW of heap allocated string object - always 0 as we only have 64kb of addressable memory
	PUSH HL		; LSW of heap allocated string obejct

	PUSH DE		; Restore return address
	RET

INPUTBUF:	DEFS 240
