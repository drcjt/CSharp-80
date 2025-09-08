.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}

.assembly rvastatic
{
	.ver 0:0:0:0
}

.class public A {

.method static void V1()
{
	.maxstack 50
    .locals ()

    ldsfld int8 [rvastatic]A::a0100
    ldc.i4 0
    beq a0101
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0101:
    ldsfld int16 [rvastatic]A::a0101
    ldc.i4 1
    beq a0102
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0102:
    ldsfld int32 [rvastatic]A::a0102
    ldc.i4 2
    beq a0103
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0103:
    ldsfld int8 [rvastatic]A::a0103
    ldc.i4 3
    beq a0104
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0104:
    ldsfld int16 [rvastatic]A::a0104
    ldc.i4 4
    beq a0105
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0105:
    ldsfld int32 [rvastatic]A::a0105
    ldc.i4 5
    beq a0106
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0106:
    ldsfld int8 [rvastatic]A::a0106
    ldc.i4 6
    beq a0107
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0107:
    ldsfld int16 [rvastatic]A::a0107
    ldc.i4 7
    beq a0108
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0108:
    ldsfld int32 [rvastatic]A::a0108
    ldc.i4 8
    beq a0109
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0109:
    ldsfld int8 [rvastatic]A::a0109
    ldc.i4 9
    beq a0110
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0110:
    ldsfld int16 [rvastatic]A::a0110
    ldc.i4 10
    beq a0111
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0111:
    ldsfld int32 [rvastatic]A::a0111
    ldc.i4 11
    beq a0112
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0112:

    ret
}

.method static void V2()
{
	.maxstack 50
    .locals ()

    ldsflda int8 [rvastatic]A::a0100
    ldind.i1
    ldc.i4 0
    beq a0100
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0100:
    ldsflda int16 [rvastatic]A::a0101
    ldind.i2
    ldc.i4 1
    beq a0101
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0101:
    ldsflda int32 [rvastatic]A::a0102
    ldind.i4
    ldc.i4 2
    beq a0102
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0102:
    ldsflda int8 [rvastatic]A::a0103
    ldind.i1
    ldc.i4 3
    beq a0103
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0103:
    ldsflda int16 [rvastatic]A::a0104
    ldind.i2
    ldc.i4 4
    beq a0104
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0104:
    ldsflda int32 [rvastatic]A::a0105
    ldind.i4
    ldc.i4 5
    beq a0105
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0105:
    ldsflda int8 [rvastatic]A::a0106
    ldind.i1
    ldc.i4 6
    beq a0106
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0106:
    ldsflda int16 [rvastatic]A::a0107
    ldind.i2
    ldc.i4 7
    beq a0107
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0107:
    ldsflda int32 [rvastatic]A::a0108
    ldind.i4
    ldc.i4 8
    beq a0108
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0108:
    ldsflda int8 [rvastatic]A::a0109
    ldind.i1
    ldc.i4 9
    beq a0109
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0109:
    ldsflda int16 [rvastatic]A::a0110
    ldind.i2
    ldc.i4 10
    beq a0110
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0110:
    ldsflda int32 [rvastatic]A::a0111
    ldind.i4
    ldc.i4 11
    beq a0111
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0111:
    ldsflda int8 [rvastatic]A::a0112
    ldind.i1
    ldc.i4 0x12
    beq a0112
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0112:
    ldsflda int16 [rvastatic]A::a0113
    ldind.i2
    ldc.i4 0x3412
    beq a0113
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0113:
    ldsflda int32 [rvastatic]A::a0114
    ldind.i4
    ldc.i4 0x78563412
    beq a0114
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0114:

    ret
}

.method static void V3()
{
	.maxstack 50
    .locals ()

    ldsfld int8 [rvastatic]A::a0100
    ldc.i4 1
    add
    stsfld int8 [rvastatic]A::a0100
    ldsfld int8 [rvastatic]A::a0100
    ldc.i4 1
    beq a0100
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0100:
    ldsfld int16 [rvastatic]A::a0101
    ldc.i4 1
    add
    stsfld int16 [rvastatic]A::a0101
    ldsfld int16 [rvastatic]A::a0101
    ldc.i4 2
    beq a0101
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0101:
    ldsfld int32 [rvastatic]A::a0102
    ldc.i4 1
    add
    stsfld int32 [rvastatic]A::a0102
    ldsfld int32 [rvastatic]A::a0102
    ldc.i4 3
    beq a0102
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0102:
    ldsfld int8 [rvastatic]A::a0103
    ldc.i4 1
    add
    stsfld int8 [rvastatic]A::a0103
    ldsfld int8 [rvastatic]A::a0103
    ldc.i4 4
    beq a0103
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0103:
    ldsfld int16 [rvastatic]A::a0104
    ldc.i4 1
    add
    stsfld int16 [rvastatic]A::a0104
    ldsfld int16 [rvastatic]A::a0104
    ldc.i4 5
    beq a0104
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0104:
    ldsfld int32 [rvastatic]A::a0105
    ldc.i4 1
    add
    stsfld int32 [rvastatic]A::a0105
    ldsfld int32 [rvastatic]A::a0105
    ldc.i4 6
    beq a0105
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0105:
    ldsfld int8 [rvastatic]A::a0106
    ldc.i4 1
    add
    stsfld int8 [rvastatic]A::a0106
    ldsfld int8 [rvastatic]A::a0106
    ldc.i4 7
    beq a0106
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0106:
    ldsfld int16 [rvastatic]A::a0107
    ldc.i4 1
    add
    stsfld int16 [rvastatic]A::a0107
    ldsfld int16 [rvastatic]A::a0107
    ldc.i4 8
    beq a0107
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0107:
    ldsfld int32 [rvastatic]A::a0108
    ldc.i4 1
    add
    stsfld int32 [rvastatic]A::a0108
    ldsfld int32 [rvastatic]A::a0108
    ldc.i4 9
    beq a0108
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0108:
    ldsfld int8 [rvastatic]A::a0109
    ldc.i4 1
    add
    stsfld int8 [rvastatic]A::a0109
    ldsfld int8 [rvastatic]A::a0109
    ldc.i4 10
    beq a0109
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0109:
    ldsfld int16 [rvastatic]A::a0110
    ldc.i4 1
    add
    stsfld int16 [rvastatic]A::a0110
    ldsfld int16 [rvastatic]A::a0110
    ldc.i4 11
    beq a0110
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0110:
    ldsfld int32 [rvastatic]A::a0111
    ldc.i4 1
    add
    stsfld int32 [rvastatic]A::a0111
    ldsfld int32 [rvastatic]A::a0111
    ldc.i4 12
    beq a0111
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0111:
    ldsfld int8 [rvastatic]A::a0112
    ldc.i4 1
    add
    stsfld int8 [rvastatic]A::a0112
    ldsfld int8 [rvastatic]A::a0112
    ldc.i4 0x13
    beq a0112
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0112:
    ldsfld int16 [rvastatic]A::a0113
    ldc.i4 1
    add
    stsfld int16 [rvastatic]A::a0113
    ldsfld int16 [rvastatic]A::a0113
    ldc.i4 0x3413
    beq a0113
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw

a0113:
    ldsfld int32 [rvastatic]A::a0114
    ldc.i4 1
    add
    stsfld int32 [rvastatic]A::a0114
    ldsfld int32 [rvastatic]A::a0114
    ldc.i4 0x78563413
    beq a0114
    newobj instance void [mscorlib]System.Exception::.ctor()
    throw
a0114:
    ret
}

.method public static int32 Main() {
.entrypoint
.maxstack  5
.locals ()

	call void A::V1()
	call void A::V2()
	call void A::V3()
	ldc.i4 0x0000
    ret
}

.field public static int8 a0100 at b0100
.field public static int16 a0101 at b0101
.field public static int32 a0102 at b0102
.field public static int8 a0103 at b0103
.field public static int16 a0104 at b0104
.field public static int32 a0105 at b0105
.field public static int8 a0106 at b0106
.field public static int16 a0107 at b0107
.field public static int32 a0108 at b0108
.field public static int8 a0109 at b0109
.field public static int16 a0110 at b0110
.field public static int32 a0111 at b0111
.field public static int8 a0112 at b0112
.field public static int16 a0113 at b0113
.field public static int32 a0114 at b0114
}

.data b0100 = int8(0)
.data b0101 = int16(1)
.data b0102 = int32(2)
.data b0103 = int8(3)
.data b0104 = int16(4)
.data b0105 = int32(5)
.data b0106 = int8(6)
.data b0107 = int16(7)
.data b0108 = int32(8)
.data b0109 = int8(9)
.data b0110 = int16(10)
.data b0111 = int32(11)

.data b0112 = bytearray(12)
.data b0113 = bytearray(12 34)
.data b0114 = bytearray(12 34 56 78)
