@echo off
echo WIN7以上系统要求以管理员权限运行
echo 本批处理与要注册的控件放在同目录下
fsutil>nul
if errorlevel 1 (echo 请关闭此窗口，使用上下文菜单（鼠标右键），选择“以管理员身份运行”
goto end)

cd /d %~dp0
regsvr32 /u /s MPFaceRecStandAloneAndCmp.ocx

echo OCX解注册成功

:end
pause