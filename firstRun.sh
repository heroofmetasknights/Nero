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
if [ $? - eq 0 ]
then
	echo "Build completed successfully"
	exit 0
else
	exit $?
fi


