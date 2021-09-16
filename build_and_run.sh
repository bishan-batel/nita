#!/bin/bash

#! CHANGE THIS TO YOUR EXECUTABLES DIRECTORY PATH IF IT IS NOT IN YOUR HOME FOLDER

# set CONNECTED_TO_WIFI="err"
set CONNECTED_TO_WIFI="ok"

# Checks if connected to wifi
#if ping -q -w 1 -c 1 `ip r | grep default | cut -d ' ' -f 3` > /dev/null; then
	# CONNECTED_TO_WIFI="ok"
#fi

cd $HOME

if [ ! -d "$HOME/nita" ]
then

	# Checks that is connected to wifi
	if [ "$CONNECTED_TO_WIFI" != "ok" ] then
		echo "Please connect to the wifi to download the repository"
		exit
	fi

	echo "Getting nita from repository"
	git clone https://github.com/bishan-batel/nita

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

if [ -d "/Applications/Godot\ mono.app" ]
then
	/Applications/Godot\ mono.app/Contents/MacOS/Godot --build-solutions -e
	exit	
fi
