# CSharp-80

![Build Status](https://github.com/drcjt/csharp-80/actions/workflows/build.yml/badge.svg)

This repo contains a .NET Core ahead of time compiler targetting the Z80 8-bit microprocessor. The compiler can compile a managed .NET Core application into an assembly language file that 
can be easily turned into a native Z80 binary file using an assembler such as [zmac](http://48k.ca/zmac.html). To learn more about CSharp-80, see the
[intro document](Documentation/intro-to-csharp-80.md)

## Credits
This project was initally inspired heavily by the [C# Snake game](https://github.com/MichalStrehovsky/SeeSharpSnake) written by Michal Strehovský, and the 
idea of being able to compile this for a retro 8 bit microcomputer. The ahead of time compilere here is influenced by the .NET Foundations 
[CoreRT project](https://github.com/dotnet/corert) and the newer [NativeAOT](https://github.com/dotnet/runtimelab/tree/feature/NativeAOT) work in the dot net 
runtime. Many ideas have been drawn from the [RyuJit just in time compiler](https://github.com/dotnet/runtime/blob/main/docs/design/coreclr/jit/ryujit-overview.md) 
in the dot net runtime. The z80 runtime code borrows heavily from the [z88dk project](https://z88dk.org/site/).

## Goals

* Idea is to support minimal subset of C# with tiny runtime system and target Z80 machine code to run on TRS-80 model 1 

[<p align="center"><img src="./Documentation/Images/trs-80-model-1.png" width="250"/></p>](trs-80-model-1.png)

* Be able to compile the [C# Snake game](https://github.com/MichalStrehovsky/SeeSharpSnake) with minimal modifications
* Learn more about AOT and the C# runtime

## Current Status

* Try out snake [here](https://drcjt.github.io/CSharp-80/index.html)
* Compiles a number of simple sample programs including a fibonacci calculator, very simple paint program, and a snake game
* Snake game is using bespoke graphics on the TRS-80 implemented via the System.Graphics.SetPixel api.
* Performance is okay - snake is quite playable. Lots of scope for improvement here
* All allocation is stack based - arrays can be done but only by using stackalloc
* On stack structs work including as return arguments
* Lots of IL opcodes not implemented yet

## Demos

|Program|Source Code|Demo|
|---|---|---|
|Snake|[snake code](https://github.com/drcjt/CSharp-80/tree/main/Samples/Snake)|[snake demo](https://github.com/drcjt/CSharp-80/blob/main/Documentation/snake.gif)|
|Netbot|[netbot](https://github.com/drcjt/CSharp-80/tree/main/Samples/NetBot)|[netbot demo](https://github.com/drcjt/CSharp-80/blob/main/Documentation/netbot.gif)|
|Graphics|[graphics demo](https://github.com/drcjt/CSharp-80/tree/main/Samples/GfxDemos)|[graphics demo](https://github.com/drcjt/CSharp-80/blob/main/Documentation/gfxdemos.gif)|

All of the demos here are recorded from running the programs in a TRS-80 emulator.