<p align="left"><img src="./Documentation/Images/CSharp80Logo.png" width="784"/></p>

![Build Status](https://github.com/drcjt/csharp-80/actions/workflows/build.yml/badge.svg)

This repo contains a .NET ahead of time compiler targetting the Z80 8-bit microprocessor. The compiler converts managed .NET assemblies into Z80 assembly language 
which is subsequently assembled using the [zmac](http://48k.ca/zmac.html) assembler. To learn more about CSharp-80, see the
[intro document](Documentation/intro-to-csharp-80.md). Currently there is support for the Trs-80, ZX Spectrum and CPM platforms.

## How to use

Try out the sample applications in an online TRS-80 emulator [here](https://drcjt.github.io/CSharp-80/index.html)

Currently the easiest way to use the compiler is to clone the repo, load the solution in Visual Studio, build, and then alter the code in one of the samples like Hello World.
Rebuild and then you can see the z80 assembly in the Hello.lst file, look in Samples/Hello/bin/Trs80/Debug/net7.0. Note that by default the solution targets the TRS-80 platform 
and so you'll also get a Hello.cmd file which is the binary file you can then use with either a real TRS-80 or an emulator.

Note that the csproj files for the samples contain some specific configuration settings to disable the normal .NET SDK libs. The samples reference the System.Private.CoreLib 
project which is a cut down version of the normal .NET SDK libs.

## Credits
* [SeeSharpSnake](https://github.com/MichalStrehovsky/SeeSharpSnake) written by Michal Strehovský
* [CoreRT project](https://github.com/dotnet/corert) from Microsoft
* [NativeAOT](https://github.com/dotnet/runtimelab/tree/feature/NativeAOT) from Microsoft
* [RyuJit just in time compiler](https://github.com/dotnet/runtime/blob/main/docs/design/coreclr/jit/ryujit-overview.md) from Microsoft
* [z88dk z80 development kit](https://z88dk.org/site/)
* [zmac - Z80 Macro Cross Assembler](http://48k.ca/zmac.html) by George Phillips

## Goals

* Idea is to support minimal subset of C# with tiny runtime system and target Z80 processor
* Runtime support for a number of computers including
  - TRS-80 Model I/III
  - ZX Spectrum
  - CPM 
* Compile the [C# Snake game](https://github.com/MichalStrehovsky/SeeSharpSnake) with minimal modifications
* Have a range of sample applications for classic 8-bit computer programs
* Learn more about AOT and the C# runtime

## Current Status

* Compiles a number of sample programs including Snake, Life, Towers of Hanoi, Hunt the Wumpus.
* Bespoke graphics for the TRS-80
* Performance is okay - snake is quite playable. Lots of scope for improvement here
* Allocation on both stack and heap is supported
* No Garbage collection
* Lots of IL opcodes not implemented yet
* Platform support is limited for ZX Spectrum and CPM but text based samples should work fine