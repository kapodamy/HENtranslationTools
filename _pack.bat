echo off
cls
HEN_GEN PS3HEN.BIN stage2.bin stage0.bin HENplugin.sprx
if %ERRORLEVEL% EQU 0 echo ... The file PS3HEN.BIN was updated ...
pause