@echo off

pushd %~dp0

if not exist zmac.exe goto download
if not exist trswrite.exe goto download

goto end

:download

curl -L --output zmac.exe https://github.com/gp48k/zmac/raw/master/zmac.exe
curl --output trsread.zip http://www.trs-80emulators.com/trsread-4.35.zip
tar -xf zmac.zip
tar -xf trsread.zip

:end

popd