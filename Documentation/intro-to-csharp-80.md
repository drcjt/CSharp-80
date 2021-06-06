Intro to CSharp-80
==================

Microsoft has experimented with Native (AOT) compilation for .NET core via a number of projects, specifically [CoreRT](https://github.com/dotnet/corert). One of the
main goals in this experimentation has been to see how much of the .NET runtime can be implemented in C# itself. In so doing this has opened the door for fun
experiments such as the minimal [C# snake game](https://github.com/MichalStrehovsky/SeeSharpSnake) in 8Kb.

CSharp-80 was inspired by the C# snake game and is an attempt to build a AOT compiler for .NET targetting a retro 16 bit microcomputer, specifically the TRS-80 Model 1, 
which can compile the snake game.

Architecture
============

[CSharp-80 Architecture](https://github.com/drcjt/CSharp-80/blob/main/Documentation/csharp-80-architecture.md) is similar to CoreRT in consuming CIL as input but
generated Z80 Assembly language as output which can easily be turned into a native TRS-80 command file using an assembler such as [Z80Asm](http://www.trs-80emulators.com/z80asm/).