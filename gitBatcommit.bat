@echo off

:: 获取当前脚本的路径
cd /d %~dp0
:: 自动提交
@REM git init 
git status
git add . 
 git commit -m "bat commit Auto:%date:~0,10%,%time:~0,8%" 
::  git commit -m "%commitMessage%" 
@REM git push origin master
git push 
@echo 已经完成,

SET daoTime=60
:dao
set /a daoTime=daoTime-1
ping -n 2 -w 500 127.1>nul
cls
echo commit Auto finished !!!: %daoTime%秒
if %daoTime%==0 (exit) else (goto dao)


