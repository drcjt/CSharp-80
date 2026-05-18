.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}

.assembly TestAssembly
{
	.ver 0:0:0:0
}

.class public Test_nestedtrycatch
{

.method public static int32 Main() {
.entrypoint
.maxstack  5
.locals (int32, int32)

    ldc.i4 0x00
    stloc   0
    ldc.i4 0x00
    stloc   1

begin:
    .try
    {
        ldc.i4 0x01
        ldloc 0
        or
        stloc 0

        .try
        {
            ldc.i4 0x02
            ldloc 0
            or
            stloc 0

            .try
            {
                ldc.i4 0x04
                ldloc 0
                or
                stloc 0

                leave done
            } catch [mscorlib]System.Exception {
                ldc.i4 0x04
                ldloc 1
                or
                stloc 1
                leave done
            }
        } catch [mscorlib]System.Exception {
            ldc.i4 0x04
            ldloc 1
            or
            stloc 1
            leave done
        }
    } catch [mscorlib]System.Exception {
        ldc.i4 0x04
        ldloc 1
        or
        stloc 1
        leave done
    }

done:
    ldloc 0
    ldc.i4 0x07
    ceq
    brfalse FAIL
    ldloc 1
    ldc.i4 0x00
    ceq
    brfalse FAIL

PASS:
	ldc.i4 0x0000
	ret
FAIL:
	ldc.i4 0x0001
	ret
}
}
