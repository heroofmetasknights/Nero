#!/bin/sh
nullout=/dev/null

echo "Checking prerequisites for Nero : PM2 (for auto relaunch at startup & daemon mode) and Dotnet (to compile and run Nero).\n"

if hash dotnet 2>$nullout
then
  DOTNET_FOUND=0
  echo "Dotnet installed.\n"
else
  echo "Dotnet must be installed before launching Nero.\n"
  exit 1
fi

if hash pm2 2>$nullout
then
  PM2_FOUND=0
  echo "PM2 installed.\n"
else
  echo "PM2 wasn't found on your system.\n"
  echo "You won't be able to launch Nero using PM2.\n"
fi

if [ $# -eq 1 ]
then
  if [ $1 -eq 1 ] #1 = PM2 mode
  then
      if ${PM2_FOUND}
      then
        wget -N https://github.com/alink-tothepast/shell-scripts/raw/master/start.sh
        wget -N https://github.com/alink-tothepast/shell-scripts/raw/master/pm2start.sh
        bash pm2start.sh
      else
        wget -N https://github.com/alink-tothepast/shell-scripts/raw/master/start.sh
        bash start.sh
      fi
  elif [ $1 -eq 2 ] #2 = Normal mode
   then
    #statements
      wget -N https://github.com/alink-tothepast/shell-scripts/raw/master/start.sh
      bash start.sh
  else
    rm -rf $HOME/Nero/start.sh
    exit -1
  fi
fi
