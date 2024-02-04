# jitdiff - CSharp-80 Compiler codegen disassembly analysis tool

This is a utility to summarize changes in generated disassembly code from the CSharp-80 ILCompiler.
The tool will produce the total bytes of difference and list of files and methods sorted by size in bytes of regression/improvement

To use:

* Clone CSharp-80 into a base folder and build the configurations you are interested in.
* Clone CSharp-80 into a diff folder, make changes here and build the same configurations.
* Run jitdiff `<base path>` `<diff path>`` to produce a summary of the  differences.
  
The output of analyze looks like the following:
```
$ jitdiff c:\source\CSharp-80-Base c:\source\CSharp-80-Diff

Summary:
(Note: Lower is better)

Total bytes of diff: -115
        diff is an improvement.

Top file regressions by size (bytes).
             133 : Samples\Wumpus\bin\Trs80\Debug\net8.0\Wumpus.dasm

Top file improvements by size (bytes).
             -52 : Samples\Life\bin\Trs80\Debug\net8.0\Life.dasm
             -46 : Samples\Mines\bin\Trs80\Debug\net8.0\Mines.dasm
             -43 : Samples\Matrix\bin\Trs80\Debug\net8.0\Matrix.dasm
             -34 : Samples\Chess\bin\Trs80\Debug\net8.0\Chess.dasm
             -26 : Samples\Snake\bin\Trs80\Debug\net8.0\Snake.dasm

12 total files with size differences.

Top method regressions by size (bytes):
              94 : Samples\Wumpus\bin\Trs80\Debug\net8.0\Wumpus.dasm - Wumpus.Room[] Wumpus.CaveBuilder::Build()
              49 : Samples\Wumpus\bin\Trs80\Debug\net8.0\Wumpus.dasm - System.Void Wumpus.Game::ResetGame()

Top method improvements by size (bytes):
             -18 : Samples\Life\bin\Trs80\Debug\net8.0\Life.dasm - System.Void Life.CellMap::ClearPixel(System.Int32,System.Int32)
             -18 : Samples\Life\bin\Trs80\Debug\net8.0\Life.dasm - System.Void Life.CellMap::SetPixel(System.Int32,System.Int32)
             -16 : Samples\Matrix\bin\Trs80\Debug\net8.0\Matrix.dasm - System.Void System.Console::SetCursorPosition(System.Int32,System.Int32)
             -16 : Samples\Snake\bin\Trs80\Debug\net8.0\Snake.dasm - System.Void System.Console::SetCursorPosition(System.Int32,System.Int32)
             -16 : Samples\Life\bin\Trs80\Debug\net8.0\Life.dasm - System.Void System.Console::SetCursorPosition(System.Int32,System.Int32)

32 total methods with size differences.
```