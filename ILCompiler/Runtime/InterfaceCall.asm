; This routine performs an interface call by resolution through interface map/dispatch maps
; finally calling the method via the VTable
;
; For each dispatch map entry
;    If method slot matches
;    then
;       search interface map to determine interface index
;       If dispatch map entry matches interface index
;       then dynamic dispatch based on implementation slot in dispatch map
; Next
; Repeat for base type
;
; Uses: HL, DE, BC, HL', DE', BC', AF
;
; On Entry: BC = Interface EEType Ptr, E = method slot, this pointer is on stack behind return address


; EEType contains, flags (2 bytes), base size (2 bytes), related type (2 bytes), vtable slot count (1 byte), inteface slot count (1 byte)
BaseTypeOffset:		EQU 4

InterfaceCall:	
	LD (REQDINTERFACE), BC

	LD A, E
	LD (REQDMETHOD), A

	POP AF
	POP HL		; This pointer
	PUSH AF

	LD E, (HL)	; Get EEType as starting type to check
	INC HL
	LD D, (HL)
	LD (BASETYPE), DE

CheckType:
	LD HL, (BASETYPE)

	LD DE, BaseTypeOffset
	ADD HL, DE

	LD C, (HL)
	INC HL
	LD B, (HL)
	INC HL
	LD (BASETYPE), BC

	LD B, (HL)	; vtable slot count
	INC HL
	LD A, (HL)	; interface slot count
	LD (INTFSLOTS), A
	INC HL

	LD (VTABLE), HL

	; Skip over the vtable & interface slots
	EX DE, HL

	LD H, 0
	LD L, B

	LD B, 0
	LD C, A

	ADD HL, BC
	ADD HL, HL	; 2 bytes per slot
	ADD HL, DE	; add slots * 2 to ptr to start of slots

	; HL now pointing at dispatch map entry count
	LD B, (HL)

	; Now search dispatch map for entry with required method slot
	LD A, 0
	CP B
	JR NZ, SearchDispatchMap

	JR CheckType

SearchDispatchMap:
	; Compare interface map entry to desired interface eetype

	; HL pointing at dispatch map count
	; B is number of entries in map

	; Save ptr to intfmap
	DEC HL
	LD (INTFMAP), HL
	INC HL

	LD A, (REQDMETHOD)
	LD E, A

NextMapEntry:
	INC HL
	LD D, (HL)	; interface index
	INC HL
	LD A, (HL)	; interface slot
	CP E
	JR NZ, NoMatch

	EXX

	LD A, (INTFSLOTS)
	LD B, A
	LD HL, (INTFMAP)

GetIntfIndexLoop:

	LD A, (REQDINTERFACE + 1)
	CP (HL)
	DEC HL
	JR NZ, IntfIndexLoopEnd
	LD A, (REQDINTERFACE)
	CP (HL)
	DEC HL
	JR NZ, IntfIndexLoopEnd

	DEC B
	LD A, B
	LD (INTFINDEX), A
	EXX

	LD A, (INTFINDEX)	; Check interface index matches
	CP D
	JR NZ, NoMatch
	
	; Got right dispatch map entry - get the implementation slot
	INC HL
	LD E, (HL)
	LD D, 0

	; HL = implementation slot * 2, BC = vtable
	EX DE, HL
	ADD HL, HL
	LD BC, (VTABLE)

	; Dynamic dispatch to destination method
	JP VirtualCall_Internal

IntfIndexLoopEnd:
	DJNZ GetIntfIndexLoop

	EXX

NoMatch:
	INC HL	; Skip implementation slot
	DJNZ NextMapEntry

	JR CheckType

REQDINTERFACE:	DW 0
REQDMETHOD:		DB 0 
BASETYPE:		DW 0
INTFSLOTS:		DB 0
INTFMAP			DW 0
INTFINDEX:		DB 0
VTABLE:			DW 0 