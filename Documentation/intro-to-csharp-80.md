Intro to CSharp-80
==================

Microsoft has experimented with Native (AOT) compilation for .NET core via a number of projects, specifically [CoreRT](https://github.com/dotnet/corert). One of the
main goals in this experimentation has been to see how much of the .NET runtime can be implemented in C# itself. In so doing this has opened the door for fun
experiments such as the minimal [C# snake game](https://github.com/MichalStrehovsky/SeeSharpSnake) in 8Kb.

CSharp-80 was inspired by the C# snake game and is an attempt to build a AOT compiler for .NET targetting a retro 8 bit microcomputer, specifically the TRS-80 Model 1, 
which can compile the snake game.

Architecture
============

[CSharp-80 Architecture](https://github.com/drcjt/CSharp-80/blob/main/Documentation/csharp-80-architecture.md) is similar to CoreRT in consuming CIL as input but
generated Z80 Assembly language as output which can easily be turned into a native TRS-80 command file using an assembler such as [zmac](http://48k.ca/zmac.html).

Supported CIL instructions
==========================

Instruction | Supported types
----------- | ---------------
ldc.i4.0 |
ldc.i4.1 |
ldc.i4.2 | 
ldc.i4.3 |
ldc.i4.4 |
ldc.i4.5 |
ldc.i4.6 |
ldc.i4.7 |
ldc.i4.8 |
ldc.i4 |
ldc.i4.s |
stloc.0 | int32
stloc.1 | int32
stloc.2 | int32
stloc.3 | int32
ldloc.0 | int32
ldloc.1 | int32
ldloc.2 | int32
ldloc.3 | int32
stind.i1 | 
add	| int32
sub | int32
mul | int32
div | int32
div.un | int32
rem.un | int32
neg | int32
br.s | int32
blt.s | int32
bgt.s | int32
ble.s | int32
bge.s | int32
beq.s | int32
bne.un.s | int32
brfalse.s | int32
brtrue.s | int32
br | int32
blt | int32
bgt | int32
ble | int32
bg | int32
beq | int32
bne.un | int32
brfalse | int32
brtrue | int32
ldarg.0 | int32
ldarg.1 | int32
ldarg.2 | int32
ldarg.3 | int32
ldstr | Very simple implementation not really using proper object representation for string
ret |
call |
