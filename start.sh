#!/bin/sh

echo ""
echo "Nero version 4.2"
echo "Rivercity Ransomware was here."
root=$HOME
nullout=/dev/null

cd $root/Nero
dotnet restore
dotnet build --configuration release
cd "$root/Nero"
echo "Running Nero (public version). Please wait."
dotnet run --configuration release
echo "Done"
