.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}

.assembly TestAssembly
{
	.ver 0:0:0:0
}

.class public Test_nestedtryfinally
{
.method public static int32 Main() {
.entrypoint
.maxstack  5
.locals (int32, int32)

    ldc.i4 0x00
    stloc   0
    ldc.i4 0x00
    stloc   1

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
            } finally
            {
                ldc.i4 0x04
                ldloc 1
                or
                stloc 1
                endfinally
            }
        } finally
        {
            ldc.i4 0x02
            ldloc 1
            or
            stloc 1
            endfinally
        }
    } finally
    {
        ldc.i4 0x01
        ldloc 1
        or
        stloc 1
        endfinally
    }

done:

    ldc.i4 0x07
    ldloc 0
    ceq
    brfalse FAIL

    ldc.i4 0x07
    ldloc 1
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
