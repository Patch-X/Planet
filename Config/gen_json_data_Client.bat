set WORKSPACE=.
set LUBAN_DLL=%WORKSPACE%\Luban\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t all ^
    -c cs-simple-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputDataDir=..\Assets\GameRes\Config\Json ^
    -x outputCodeDir=..\Assets\GameScript\Game.HotUpdate\Config


pause
rem -c cs-simple-json ^ -x outputDataDir=..\Assets\Config\Json ^