@echo off

rem �ж��Ƿ��Թ���Ա����
fsutil>nul
if errorlevel 1 (echo ��رմ˴��ڣ�ʹ�������Ĳ˵�������Ҽ�����ѡ���Թ���Ա������С�
goto end)

cd /d %~dp0

MPFaceRecStandAloneServer.exe stop
MPFaceRecStandAloneServer.exe uninstall

:end
pause
