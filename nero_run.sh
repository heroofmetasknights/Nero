#!/bin/sh

echo ""
echo "Nero version 4.2"
echo "Rivercity Ransomware was here."
root=$HOME
nullout=/dev/null

echo "Checking prerequisites : PM2 & dotnet"
if hash dotnet 2>$nullout
then
  echo "Dotnet installed."
else
  echo "Dotnet must be installed before launching Nero."
  exit 1
fi

if hash pm2 2>$nullout
then
  echo "PM2 installed."
else
  echo "PM2 wasn't found on your system."
  exit 1
fi

cd $root/Nero
dotnet restore
dotnet build --configuration Release
cd "$root/Nero"
echo "Running Nero. Please wait."
dotnet run --configuration Release
echo "Done"
