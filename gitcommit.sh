
# unset msg

# read -p "请输入commit提交的描述: " msg
# git status
# git add -A
# git status
# git commit -m $msg
# git push
# git status

#!/bin/bash
read -p "请输入commit提交的描述: " msg
currDir=$(pwd)
remark=$1
if [ ${remark}x = ""x ];then
remark=$(date +"%Y-%m-%d %H:%M:%S") 
fi
echo ${currDir}
git add .
# git commit -m "\"${remark}\""
git commit -m $msg
git push