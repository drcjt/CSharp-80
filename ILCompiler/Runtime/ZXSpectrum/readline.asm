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

	POP HL		; Ret Address
	POP DE		; EETypePtr
	PUSH HL		; Ret Address

	; Put actual entered string length into HL
	LD H, 0
	LD L, B

	PUSH BC		; Save string length

	PUSH DE
	CALL NewString

	POP HL		; HL = allocated string
	POP BC		; BC = string length

	PUSH HL		; Save allocated string

	; Skip EEPtr and String Length
	LD DE, 4
	ADD HL, DE

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

INPUTBUF:	DEFS 240