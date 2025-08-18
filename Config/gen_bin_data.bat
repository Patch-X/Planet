set WORKSPACE=.
set LUBAN_DLL=%WORKSPACE%\Luban\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t all ^
    -c cs-bin ^
    -d bin  ^
    --conf %CONF_ROOT%\luban.conf ^
	-x outputDataDir=output\bytes	^
	-x outputCodeDir=output\code


pause
rem -c cs-bin ^ -x outputDataDir=..\Assets\Config\Bytes ^