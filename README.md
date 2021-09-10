# Nita

## How to Build & Run

### Installing Godot
<b><a href="https://godotengine.org/download">Download Here</a></b> 
Make sure to download the "Godot Mono x64" (or x32) version

After downloading, you need to unzip the contents of the .zip file into a 
secure directory and you need to make sure you keep where you store this file
consistent. I recommend in your home folder (filepath: `/home/your_username/`)
<br>

Make sure to put both the 'GodotSharp' folder and the 'Godot_v3.3.3-stable_mono_x11_64' file into a folder
named 'godot' (my example, inside `/home/your_username/godot/`)

After, rename the executable to just 'godot'

### Build & Run Script
To get the build and run script either download 'build_and_run.sh' from the repo above
<br><b>OR</b><br>
Create a nita.sh file and paste the follow text inside

```sh
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

$HOME/godot/godo* --build-solutions -e
```

After you get your run .sh file, run it with the terminal to launch the project.
<br>It should open the godot window with it saying "building solutions...", After it is finished with build solutions (and/or reimporting assets), click the play button in the top right corner of the screen to run the project (5 icons to the left from 'Build').


## Resource Credits

- https://somepx.itch.io/humble-fonts-free
- https://godotshaders.com/shader/energy-beams/
