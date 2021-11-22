CLS:
	ld	de, clsdata
	ld	c, 9	
	jp	5	; let bdos return for us
clsdata:	db      1bh,"[0m",1bh,"[;H",1bh,"[2J$"
