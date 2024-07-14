.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.class public value sealed valClass {
	.field public int32 Field1;
	.field public char Field2;

	.method public void Init() {
		.maxstack 4

		ldarg	0
		ldc.i4	0xFF
		stfld	int32 valClass::Field1

		ldarg	0
		ldc.i4	0x54
		stfld	char valClass::Field2

		ret
	}

	.method public virtual class [mscorlib]System.String ToString() {
		.maxstack 4
		ldstr "Some Test String"
		ret
	}
}

.class public _box_unbox {

.field public static value class valClass vc;


.method public static int32 Main() {
.entrypoint
.maxstack 10

	ldsflda value class valClass _box_unbox::vc
	call instance void valClass::Init()

	ldsflda value class valClass _box_unbox::vc
	ldfld int32 valClass::Field1

	ldc.i4 0xFF
	ceq
	brfalse fail

	ldsfld value class valClass _box_unbox::vc
	box valClass
	callvirt instance class [mscorlib]System.String [mscorlib]System.Object::ToString()
	ldstr "Some Test String"
	callvirt instance bool [mscorlib]System.Object::Equals(class [mscorlib]System.Object)
	brfalse fail

pass:
	ldc.i4 0x0000
	ret

fail:
	ldc.i4 0x0001
	ret
}
}