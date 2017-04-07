:: Copyright 2017 - Refael Ackermann
:: Distributed under MIT style license
:: See accompanying file LICENSE at https://github.com/node4good/windows-autoconf
:: version: 1.12.1

@IF NOT DEFINED DEBUG_GETTER @ECHO OFF
SETLOCAL
PUSHD %~dp0
powershell -NoProfile -ExecutionPolicy Unrestricted ".\GetKey.ps1" %*
IF ERRORLEVEL 1 SET RET=1
POPD
EXIT /B %RET%
