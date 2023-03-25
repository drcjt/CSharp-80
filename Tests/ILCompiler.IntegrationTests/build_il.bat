REM Ensures the zmac assembler has been downloaded
REM copies test il files to appropriate location for integration tests to run

set outdir=%2

call %1\tools\get-tools.cmd

mkdir %outdir%\il_bvt
robocopy /njh /njs /ns /nc /ndl /np .\il_bvt %outdir%\il_bvt *.il