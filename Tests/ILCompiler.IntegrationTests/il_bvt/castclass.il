.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.namespace castclass_
{

.class interface public TestAInterface {
}

.class interface public TestBInterface {
}


.class public Super
{
.method public void .ctor() {
.maxstack 10
    ldarg.0
    call instance void [mscorlib] System.Object::.ctor()
    ret
}
}

.class public Sub extends castclass_.Super implements castclass_.TestAInterface
{
.method public void .ctor() {
.maxstack 10
    ldarg.0
    call instance void castclass_.Super::.ctor()
    ret
}
}


.class public _castclass
{

.method public static int32 positivetestclass()  {
.maxstack 10

try_start:
    newobj instance void castclass_.Sub::.ctor()
    castclass castclass_.Super
    pop
    leave try_end
try_end:
    ldc.i4 0x1
    ret

handler_start:
    pop
    leave done
handler_end:
done:
    ldc.i4 0x0
    ret
.try try_start to try_end catch [mscorlib]System.InvalidCastException handler handler_start to handler_end
}

.method public static int32 negativetestclass()  {
.maxstack 10

try_start:
    newobj instance void castclass_.Sub::.ctor()
    castclass castclass_._castclass
    pop
    leave try_end
try_end:
    ldc.i4 0x0
    ret

handler_start:
    pop
    leave done
handler_end:
done:
    ldc.i4 0x1
    ret
.try try_start to try_end catch [mscorlib]System.InvalidCastException handler handler_start to handler_end
}

.method public static int32 positivetestinterface()  {
.maxstack 10

try_start:
    newobj instance void castclass_.Sub::.ctor()
    castclass castclass_.TestAInterface
    pop
    leave try_end
try_end:
    ldc.i4 0x1
    ret

handler_start:
    pop
    leave done
handler_end:
done:
    ldc.i4 0x0
    ret
.try try_start to try_end catch [mscorlib]System.InvalidCastException handler handler_start to handler_end
}

.method public static int32 negativetestinterface()  {
.maxstack 10

try_start:
    newobj instance void castclass_.Sub::.ctor()
    castclass castclass_.TestBInterface
    pop
    leave try_end
try_end:
    ldc.i4 0x0
    ret

handler_start:
    pop
    leave done
handler_end:
done:
    ldc.i4 0x1
    ret
.try try_start to try_end catch [mscorlib]System.InvalidCastException handler handler_start to handler_end
}



.method public static int32 Main() {
.entrypoint
.maxstack  5
.locals ()

    call int32 castclass_._castclass::positivetestclass()
    brfalse FAIL

    call int32 castclass_._castclass::negativetestclass()
    brfalse FAIL

    call int32 castclass_._castclass::positivetestinterface()
    brfalse FAIL

    call int32 castclass_._castclass::negativetestinterface()
    brfalse FAIL

PASS:
	ldc.i4 0x0000
	ret
FAIL:
	ldc.i4 0x0001
	ret
}
}

}
