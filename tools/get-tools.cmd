@echo off

pushd %~dp0

if not exist trswrite.exe goto download

goto end

:download

curl --output trsread.zip http://www.trs-80emulators.com/trsread-4.35.zip
tar -xf trsread.zip

:end

popd