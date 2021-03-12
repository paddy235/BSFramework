@echo off

rem 判断是否以管理员运行
fsutil>nul
if errorlevel 1 (echo 请关闭此窗口，使用上下文菜单（鼠标右键），选择“以管理员身份运行”
goto end)

cd /d %~dp0

MPFaceRecStandAloneServer.exe stop
MPFaceRecStandAloneServer.exe uninstall

:end
pause
