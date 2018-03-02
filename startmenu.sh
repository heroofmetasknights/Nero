#!/bin/sh
nullout=/dev/null

echo "Checking prerequisites for Nero : PM2 (for auto relaunch at startup & daemon mode) and Dotnet (to compile and run Nero)."

if hash dotnet 2>$nullout
then
  DOTNET_FOUND=0
  echo "Dotnet installed."
else
  echo "Dotnet must be installed before launching Nero."
  exit 1
fi

if hash pm2 2>$nullout
then
  PM2_FOUND=0
  echo "PM2 installed."
else
  echo "PM2 wasn't found on your system."
  echo "You won't be able to launch Nero using PM2."
fi

wget -N https://github.com/alink-tothepast/shell-scripts/raw/master/start.sh

if [ $? -eq 1 ]
then
  if [$1 -eq "PM2" && ${PM2_FOUND}]
  then
      wget -N https://github.com/alink-tothepast/shell-scripts/raw/master/pm2start.sh
      bash pm2start.sh
  elif [ $1 -eq "Normal" ]
   then
    #statements
      bash start.sh
  else
    exit -1
  fi
fi
