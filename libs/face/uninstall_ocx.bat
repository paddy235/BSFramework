@echo off
echo WIN7����ϵͳҪ���Թ���ԱȨ������
echo ����������Ҫע��Ŀؼ�����ͬĿ¼��
fsutil>nul
if errorlevel 1 (echo ��رմ˴��ڣ�ʹ�������Ĳ˵�������Ҽ�����ѡ���Թ���Ա������С�
goto end)

cd /d %~dp0
regsvr32 /u /s MPFaceRecStandAloneAndCmp.ocx

echo OCX��ע��ɹ�

:end
pause