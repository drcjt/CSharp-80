REM copies test il files to appropriate location for integration tests to run

set outdir=%2

mkdir %outdir%\il_bvt
robocopy /njh /njs /ns /nc /ndl /np .\il_bvt %outdir%\il_bvt *.il