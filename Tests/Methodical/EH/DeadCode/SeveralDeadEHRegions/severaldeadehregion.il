.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}

.assembly TestAssembly
{
	.ver 0:0:0:0
}

.class public Test_severaldeadehregion
{

.method public static int32 Main() {
.entrypoint
.maxstack  1

    .try
    {
        ldc.i4.1
        call void Log::Mark(int32)  // In try

        br skip
        
        .try
        {
        L:
            ldc.i4.2
            call void Log::Mark(int32)  // In Inner try, unreached
            x:
            leave   done

        try_start:
            ldc.i4.3
            call void Log::Mark(int32)  // In try
            leave   done

            .try
            {
                ldc.i4.4
                call void Log::Mark(int32)  // In try
                leave   done
            }
            finally
            {
                ldc.i4.5
                call void Log::Mark(int32)  // In finally
                endfinally
            }

        filter_begin:
            pop
            ldc.i4.6
            call void Log::Mark(int32)  // In filter
            ldc.i4.1
            endfilter

        handler_begin:
            ldc.i4.7
            call void Log::Mark(int32)  // In handler
            leave.s     done
        handler_end:

        .try try_start to filter_begin filter filter_begin handler handler_begin to handler_end

        leave.s     done
        }
        finally
        {
            endfinally
            .try
            {
                ldc.i4.8
                call void Log::Mark(int32)  // In finally, unreached
              L2:
                br.s L2
            }
            catch [mscorlib]System.Object
            {
                leave.s L3
            }
            L3:
            endfinally               
        }

        .try {
            ldc.i4 9
            call void Log::Mark(int32)  // Unreached
            leave.s done
        } catch [mscorlib]System.Exception {
            leave.s done
        }

        .try {
            ldc.i4 10
            call void Log::Mark(int32)  // Unreached
            leave.s done
        } catch [mscorlib]System.Exception {
            leave.s done
        }

        .try {
            ldc.i4 11
            call void Log::Mark(int32)  // Unreached
            leave.s done
        } catch [mscorlib]System.Exception {
            leave.s done
        }

        .try {
            ldc.i4 12
            call void Log::Mark(int32)  // Unreached
            leave.s done
        } catch [mscorlib]System.Exception {
            leave.s done
        }

        .try {
            ldc.i4 13
            call void Log::Mark(int32)  // Unreached
            leave.s done
        } catch [mscorlib]System.Exception {
            leave.s done
        }

        .try {
            ldc.i4 14
            call void Log::Mark(int32)  // Unreached
            leave.s done
        } catch [mscorlib]System.Exception {
            leave.s done
        }

      skip:
        leave.s    done

    }
    catch [mscorlib]System.Object
    {
        ldc.i4 15
        call void Log::Mark(int32)  // In catch, Unreached
        leave.s    done
    }

done:
    ldc.i4 16
    call void Log::Mark(int32)  // Done

    ldc.i4.2
    newarr     [mscorlib]System.Int32
    dup
    ldc.i4.0
    ldc.i4.1    // In try
    stelem.i4
    dup
    ldc.i4.1
    ldc.i4 16    // Done
    stelem.i4

    call int32 Log::AssertSequence(int32[])
    ret
}
}

.class public auto ansi beforefieldinit Class1
       extends [mscorlib]System.Object
{
    .method public hidebysig static void inTry() cil managed
    {
        .maxstack 8

        ldc.i4.1
        call void Log::Mark(int32)  // In try

        ret
    }
}

.class public auto ansi beforefieldinit Log
       extends [mscorlib]System.Object
{
    .field public static int32[] Events
    .field public static int32 Index

    .method private static void .cctor() cil managed
    {
        .maxstack 8

        ldc.i4.s    10
        newarr     [mscorlib]System.Int32
        stsfld     int32[] Log::Events
        ret
    }

    .method public static void Mark(int32 id) cil managed
    {
        .maxstack 8

        ldsfld  int32[] Log::Events
        ldsfld  int32 Log::Index
        dup
        ldc.i4.1
        add
        stsfld  int32 Log::Index
        ldarg.0
        stelem.i4
        ret
    }

    .method public hidebysig static 
        int32 AssertSequence (
            int32[] expected
        ) cil managed 
    {
        // Method begins at RVA 0x2050
        // Code size 42 (0x2a)
        .maxstack 3
        .locals init (
            [0] int32 i
        )

        ldsfld int32 Log::Index
        ldarg.0
        ldlen
        conv.i4
        beq.s beforeloop

        ldc.i4.1
        ret

        beforeloop:
        ldc.i4.0
        stloc.0
        br.s loopcondition

        loophead:

            ldsfld int32[] Log::Events
            ldloc.0
            ldelem.i4
            ldarg.0
            ldloc.0
            ldelem.i4
            beq.s movetonextelem

            ldc.i4.1
            ret

            movetonextelem:

            ldloc.0
            ldc.i4.1
            add
            stloc.0

            loopcondition:
            ldloc.0
            ldarg.0
            ldlen
            conv.i4
            blt.s loophead

        ldc.i4.0
        ret
    } // end of method Log::AssertSequence
}
