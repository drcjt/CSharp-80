; This routine performs an interface call by resolution through interface map/dispatch maps
; finally calling the method via the VTable
;
; For each dispatch map entry
;    If method slot matches
;    then
;		Use interface index in entry to find entry in interface map
;		If interface map entry matches required interface EEType
;       then dynamic dispatch based on implementation slot in dispatch map
; Next
; Repeat for base type
;
; Uses: HL, DE, BC, HL', DE', BC', AF
;
; On Entry: BC = Interface EEType Ptr, E = method slot, this pointer is on stack behind return address

; EEType contains, Component Size (2 bytes), flags (2 bytes), base size (2 bytes), related type (2 bytes), vtable slot count (1 byte), inteface slot count (1 byte)
BaseTypeOffset:		EQU 2 + 2 + 2

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

; Only check if type is array if System.Array is constructed type
if SYSTEMARRAY
	LD HL, (BASETYPE)

	; If flags = 0x18 then this is an SzArray so use the System.Array EEType as the base type

	INC HL
	INC HL
	LD C, (HL)
	INC HL
	LD B, (HL)
	INC HL

	; BC = flags

	LD HL, 0x18
	OR A
	SBC HL, BC
	JR NZ, GetBaseType

	; Use System.Array EEType as BASETYPE

	LD DE, (BASETYPE)
	LD HL, SYSTEMARRAY
	LD (BASETYPE), HL

	EX DE, HL
	LD DE, BaseTypeOffset + 2
	ADD HL, DE

	JR SearchDispatchMap

endif

GetBaseType:

	LD HL, (BASETYPE)	
	LD DE, BaseTypeOffset
	ADD HL, DE

	LD C, (HL)
	INC HL
	LD B, (HL)
	INC HL
	LD (BASETYPE), BC

SearchDispatchMap:

	LD B, (HL)	; vtable slot count
	INC HL
	LD A, (HL)	; interface slot count
	LD (INTFSLOTS), A
	INC HL

	LD (VTABLE), HL

	; Skip over the vtable

	EX DE, HL
	LD H, 0
	LD L, B
	ADD HL, HL		; 2 * vtable slots
	ADD HL, DE		; find end of vtable
	LD (INTFMAP), HL

	EX DE, HL
	LD H, 0
	LD L, A
	ADD HL, HL
	ADD HL, DE		; find end of interface map
		
	; HL now pointing at dispatch map entry count
	LD B, (HL)

	; Now search dispatch map for entry with required method slot
	LD A, 0
	CP B
	JR Z, CheckType

	; Compare interface map entry to desired interface eetype

	; HL pointing at dispatch map count
	; B is number of entries in map

NextMapEntry:
	INC HL
	LD D, (HL)	; interface index

	INC HL
	LD E, (HL)	; interface slot
	LD A, (REQDMETHOD)
	CP E
	JR NZ, NoMatch

	PUSH HL

	LD H, 0
	LD L, D
	ADD HL, HL
	EX DE, HL
	LD HL, (INTFMAP)
	ADD HL, DE

	; HL now pointing at entry in interface map need to check if this is what we want

	LD E, (HL)
	INC HL
	LD D, (HL)
	INC HL
	
	EX DE, HL
	LD DE, (REQDINTERFACE)
	OR A
	SBC HL, DE
	JR Z, Match

IntfIndexLoopEnd:
	POP HL

NoMatch:
	INC HL	; Skip implementation slot
	DJNZ NextMapEntry

	JR CheckType

Match:	
	POP HL

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


REQDINTERFACE:	DW 0
REQDMETHOD:		DB 0 
BASETYPE:		DW 0
INTFSLOTS:		DB 0
INTFMAP			DW 0
INTFINDEX:		DB 0
VTABLE:			DW 0 