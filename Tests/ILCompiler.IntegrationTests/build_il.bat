REM Creates input files for the nunit tests
REM Runs ilasm on the il_bvt il files
REM Runs ilcompiler on the exe's produced by ilasm
REM Runs zmac on the asm's produced by the ilcompiler

set solutiondir=%1
set outdir=%2
set targetdir=%3
set configuration=%4

call %1\tools\get-tools.cmd

mkdir %outdir%\il_bvt
robocopy /njh /njs /ns /nc /ndl /np .\il_bvt %outdir%\il_bvt *.il

for /f "tokens=* delims= " %%F in ('dir /b .\il_bvt\*.il') do (
@echo Preparing Bvt Il: %%F
%USERPROFILE%\.nuget\packages\microsoft.netcore.ilasm\6.0.0\runtimes\native\ilasm.exe /quiet .\il_bvt\%%F
%solutiondir%\ILCompiler\%outdir%\ILCompiler.exe --ignoreUnknownCil false --printReturnCode false --integrationTests true --corelibPath %solutiondir%\System.Private.CoreLib\bin\%configuration%\net6.0\System.Private.CoreLib.dll --outputFile %outdir%\il_bvt\%%~nF.asm .\il_bvt\%%~nF.exe
%solutiondir%\tools\zmac.exe --oo cim -o %outdir%\il_bvt\%%~nF.cim %outdir%\il_bvt\%%~nF.asm
)