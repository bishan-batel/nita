#!/bin/bash

#! CHANGE THIS TO YOUR EXECUTABLES DIRECTORY PATH IF IT IS NOT IN YOUR HOME FOLDER

set CONNECTED_TO_WIFI="err"

# Checks if connected to wifi
if ping -q -w 1 -c 1 `ip r | grep default | cut -d ' ' -f 3` > /dev/null; then
	CONNECTED_TO_WIFI="ok"
fi

cd $HOME

if [ ! -d "$HOME/nita" ]
then

	# Checks that is connected to wifi
	if [ "$CONNECTED_TO_WIFI" != "ok" ]
	then
		echo "Please connect to the wifi to download the repository"
		exit
	fi

	echo "Getting nita from repository"
	git clone https://github.com/bishan-batel/nita

fi


if [ ! -f "$HOME/nita/.gitignore" ]
then
	echo "Issue with git clone, try to delete ~/nita folder and run script again"
	exit
fi


cd "$HOME/nita"

git reset HEAD --hard
git clean -fd

if [ "$CONNECTED_TO_WIFI" != "ok" ]
then
	echo "Not connected to the wifi, unable to check if codebase is updated"
else
	git pull
fi

chmod +x $HOME/godot/godo*

$HOME/godot/godo* --build-solutions -q --quiet

echo "Build complete, attempting run"

$HOME/godot/godo* --quiet
