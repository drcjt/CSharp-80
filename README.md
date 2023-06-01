<p align="left"><img src="./Documentation/Images/CSharp80Logo.png" width="784"/></p>

![Build Status](https://github.com/drcjt/csharp-80/actions/workflows/build.yml/badge.svg)

This repo contains a .NET ahead of time compiler targetting the Z80 8-bit microprocessor. The compiler converts managed .NET applications into assembly language which is subsequently assembled using the [zmac](http://48k.ca/zmac.html) assembler. To learn more about CSharp-80, see the
[intro document](Documentation/intro-to-csharp-80.md) and read the [blog](https://csharpdot80.wordpress.com/)

Try out the samples applications [here](https://drcjt.github.io/CSharp-80/index.html)

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