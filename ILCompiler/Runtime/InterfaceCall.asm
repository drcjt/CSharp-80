; This routine performs an interface call by resolution through interface map/dispatch maps
; finally calling the method via the VTable
;
; Uses: HL, DE, BC
;
; On Entry: BC = Interface EEType Ptr, DE = method slot

InterfaceCall:	

	; Save required interface eetype ptr
	LD (reqdinterface), BC
	LD (reqdmethod), DE

	POP AF		; Return address
	POP HL		; This pointer

	PUSH AF

	; First get this EEType
	LD E, (HL)	; Get EEType Ptr into HL
	INC HL
	LD D, (HL)
	LD H, D
	LD L, E

CheckType:
	; Next find the interface slot count
	; EEType contains, flags (2 bytes), base size (2 bytes), related type (2 bytes), vtable slot count (1 byte), inteface slot count (1 byte)

	LD DE,4
	ADD HL, DE

	LD C, (HL)
	INC HL
	LD B, (HL)
	INC HL
	LD (BaseType), BC

	LD B, (HL)	; B = vtable slot count
	INC HL
	LD C, (HL)	; C = interface slot count
	INC HL

	LD (vtable), HL

	; Skip over the vtable & interface slots
	EX DE, HL

	LD H, 0
	LD L, B

	LD B, 0
	; C already contains interface slot count
	LD (intfslots), BC

	ADD HL, BC

	ADD HL, HL	; 2 bytes per slot

	ADD HL, DE	; add slots * 2 to ptr to start of slots

	; HL now pointing at dispatch map entry count

	LD B, (HL)

	; Now search dispatch map for entry with required method slot
	LD A, 0
	CP B
	JR NZ, SearchDispatchMap

	LD HL, (BaseType)
	JP CheckType

SearchDispatchMap:
	; Compare interface map entry to desired interface eetype

	; HL pointing at dispatch map count
	; B is number of entries in map

	; Save ptr to intfmap
	DEC HL
	LD (intfmap), HL
	INC HL

	LD DE, (reqdmethod)

NextMapEntry:
	INC HL
	LD D, (HL)
	INC HL
	LD A, (HL)	; interface slot
	CP E
	JP NZ, NoMatch

	Call GetIntfIndex
	
	LD A, (intfindex+1)
	CP D
	JR NZ, NoMatch

	
	; Got right dispatch map entry - get the implementation slot
	INC HL
	LD E, (HL)
	LD D, 0

	; HL = vtable + (implementation slot * 2)
	EX DE, HL
	ADD HL, HL
	LD DE, (vtable)
	ADD HL, DE

	LD E, (HL)	; Get entry in VTable slot into HL
	INC HL
	LD D, (HL)
	LD H, D
	LD L, E

	JP (HL)		; Dynamic dispatch

NoMatch:
	INC HL	; Skip implementation slot

	DJNZ NextMapEntry

	LD HL, (BaseType)

	; TODO - check HL for zero - but if it is what do we do?? fail fast??

	JP CheckType

; Determine the interface index - assumes interface will be in the map
GetIntfIndex:
	PUSH BC
	PUSH HL
	PUSH DE

	LD A, (intfslots)
	LD B, A
	LD HL, (intfmap)

	DEC B

GetIntfIndexLoop:

	LD D, (HL)
	DEC HL
	LD E, (HL)
	DEC HL

	LD A, (reqdinterface+1)
	CP D
	JR NZ, LoopEnd
	LD A, (reqdinterface)
	CP E
	JR NZ, LoopEnd

	LD (intfindex), bc
	JR FoundIntfIndex

LoopEnd:
	DJNZ GetIntfIndexLoop

FoundIntfIndex:
	POP DE
	POP HL
	POP BC
	RET

reqdinterface: dw 0
reqdmethod: dw 0 
basetype: dw 0
intfslots: dw 0
intfmap dw 0
intfindex: dw 0
vtable: dw 0 