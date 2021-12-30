# CSharp-80

This repo contains a .NET Core ahead of time compiler targetting the Z80. The compiler can compile a managed .NET Core application into an assembly language file that 
can be easily turned into a native Z80 binary file using an assembler such as [zmac](http://48k.ca/zmac.html). To learn more about CSharp-80, see the
[intro document](Documentation/intro-to-csharp-80.md)

## Goals

* Idea is to support minimal subset of C# with tiny runtime system and target Z80 machine code to run on TRS-80 model 1
* Be able to compile the [C# Snake game](https://github.com/MichalStrehovsky/SeeSharpSnake) with minimal modifications
* Learn more about AOT and the C# runtime

## Current Status

* Compiles a number of simple sample programs including a fibonacci calculator, very simple paint program, and a snake game
* Snake game is using bespoke graphics on the TRS-80 implemented via the System.Graphics.SetPixel api.
* Performance is okay - snake is quite playable. Lots of scope for improvement here
* No object allocation yet - arrays can be done but only by using stackalloc

## Demos

All of the demos here are recorded from running the programs in a TRS-80 emulator.

### Snake

The C# source for the game can be seen here [C# program running](https://github.com/drcjt/CSharp-80/tree/main/Samples/Snake) in a TRS-80 emulator:

![demo](/Documentation/snake.gif)

### Graphics Demo

### Netbot

### Continuous Integration status

| | |
| --- | --- |
| **Build** | [![Build status](https://img.shields.io/appveyor/ci/drcjt/csharp-80.svg)](https://ci.appveyor.com/project/drcjt/csharp-80) |
[![Build History](https://buildstats.info/appveyor/chart/drcjt/csharp-80)](https://ci.appveyor.com/project/drcjt/csharp-80)
