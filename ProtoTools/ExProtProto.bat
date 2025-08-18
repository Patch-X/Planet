@echo off

rem 设置路径变量
set PROTOC_PATH="E:\MyFramework\my_framework\ProtoTools\protoc-23.3-win64\bin\protoc.exe"
set PROTO_DIR="E:\MyFramework\my_framework\ProtoTools\Protos"
set OUTPUT_DIR="E:\MyFramework\my_framework\Assets\GameScript\Game.HotUpdate\Protos"

rem 检查目录是否存在
if not exist %PROTO_DIR% (
    echo Error: protos directory does not exist.
    exit /b
)

rem 创建输出目录
if not exist %OUTPUT_DIR% mkdir %OUTPUT_DIR%

rem 批量处理 .proto 文件
for %%f in (%PROTO_DIR%\*.proto) do (
    %PROTOC_PATH% --proto_path=%PROTO_DIR% --csharp_out=%OUTPUT_DIR% %%f
)

echo Exprot Successed
pause
