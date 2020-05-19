@echo off

rmdir /Q /S dist
del dist.zip

set /A step=1
dotnet publish ..\..\ -o .\dist --self-contained true --runtime linux-x64 -c Release

if %errorlevel% NEQ 0 (
  echo "Build failed!"
  exit %step%
)

set /A step=step+1
7z a dist.zip .\dist\*

if %errorlevel% NEQ 0 (
  echo "Error packing files into archive"

  exit %step%
)

set /A step=step+1
pscp -i "deploy-key.ppk" -P %DEV_SSH_PORT% .\dist.zip rules-bot@mtfarkas-dev:/home/rules-bot/payload

if %errorlevel% NEQ 0 (
  echo "Error while transferring files to remote"
  
  exit %step%
)

rmdir /Q /S dist
del dist.zip

set /A step=step+1
plink.exe -batch -P %DEV_SSH_PORT% -i "deploy-key.ppk" -ssh rules-bot@mtfarkas-dev -m "deploy.sh"

if %errorlevel% NEQ 0 (
  echo "Error executing commands on remote server"
  
  exit %step%
)


pause