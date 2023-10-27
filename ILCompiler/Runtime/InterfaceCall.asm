; This routine performs an interface call by resolution through interface map/dispatch maps
; finally calling the method via the VTable
;
; Uses: HL, DE, BC, HL', DE', BC', AF
;
; On Entry: BC = Interface EEType Ptr, E = method slot

InterfaceCall:	

	; Save required interface eetype ptr
	LD (REQDINTERFACE), BC

	LD A, E
	LD (REQDMETHOD), A

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
	LD (BASETYPE), BC

	LD B, (HL)	; B = vtable slot count
	INC HL
	LD C, (HL)	; C = interface slot count
	INC HL

	LD (VTABLE), HL

	; Skip over the vtable & interface slots
	EX DE, HL

	LD H, 0
	LD L, B

	LD B, 0
	; C already contains interface slot count
	LD A, C
	LD (INTFSLOTS), A

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
	JP NZ, NoMatch

	EXX

	LD A, (INTFSLOTS)
	LD B, A
	LD HL, (INTFMAP)

GetIntfIndexLoop:

	LD A, (REQDINTERFACE + 1)
	CP (HL)
	JR NZ, LoopEnd1
	DEC HL
	LD A, (REQDINTERFACE)
	CP (HL)
	JR NZ, LoopEnd2
	DEC HL

	DEC B
	LD A, B
	LD (INTFINDEX), A
	JR FoundIntfIndex

LoopEnd1:
	DEC HL
LoopEnd2:
	DEC HL
	DJNZ GetIntfIndexLoop

FoundIntfIndex:
	EXX
	
	LD A, (INTFINDEX)
	CP D
	JR NZ, NoMatch
	
	; Got right dispatch map entry - get the implementation slot
	INC HL
	LD E, (HL)
	LD D, 0

	; HL = vtable + (implementation slot * 2)
	EX DE, HL
	ADD HL, HL
	LD DE, (VTABLE)
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

	LD HL, (BASETYPE)

	; TODO - check HL for zero - but if it is what do we do?? fail fast??

	JP CheckType

REQDINTERFACE: DW 0
REQDMETHOD: DB 0 
BASETYPE: DW 0
INTFSLOTS: DB 0
INTFMAP DW 0
INTFINDEX: DB 0
VTABLE: DW 0 