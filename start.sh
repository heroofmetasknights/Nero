#!/bin/sh

echo ""
echo "Nero bot"
echo "Release for patch 4.3"
echo "Rivercity Ransomware was here."
root=$HOME
nullout=/dev/null

cd "$root/Nero"
echo "Building Nero..."
dotnet restore
dotnet build --configuration release
cd "$root/Nero"
echo "Running Nero. Please wait."
dotnet run --configuration release
echo "Done"
