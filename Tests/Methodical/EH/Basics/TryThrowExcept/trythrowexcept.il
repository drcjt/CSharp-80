.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}

.assembly TestAssembly
{
	.ver 0:0:0:0
}

.class public Test_trythrowexcept
{

.method public static int32 Main() {
.entrypoint
.maxstack  1

    try_start:
        ldc.i4.1
        call void Log::Mark(int32)  // In try, throwing first obj
        newobj instance void [mscorlib]System.Exception::.ctor()
        throw
        leave.s done

    filter_begin:
        pop
        ldc.i4.2
        call void Log::Mark(int32)  // In filter
        ldc.i4.1
        endfilter

    handler_begin:
        pop
        ldc.i4.3
        call void Log::Mark(int32)  // In handler
        leave.s done

    done:
        ldc.i4.4
        call void Log::Mark(int32)  // Done

        ldc.i4.4
        newarr     [mscorlib]System.Int32
        dup
        ldc.i4.0
        ldc.i4.1    // In try
        stelem.i4
        dup
        ldc.i4.1
        ldc.i4.2    // In filter
        stelem.i4
        dup
        ldc.i4.2
        ldc.i4.3    // In handler
        stelem.i4
        dup
        ldc.i4.3
        ldc.i4.4    // Done
        stelem.i4

        call int32 Log::AssertSequence(int32[])
        ret
        
   .try try_start to filter_begin filter filter_begin handler handler_begin to done
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
