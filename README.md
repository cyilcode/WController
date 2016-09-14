# WirelessControl
Wireless control is a cross-platform server module for remote applications that uses TCP/IP Protocol.
# Features (to be implemented) 
* OS Controls like: sound, input controls(mouse/keyboard etc)
* Application executions
* Operation timers
* Setup OS Tasks
* Custom code execution (plugin development)
* 100% Free

# How to install ?

Currently, WController is not operational yet. However, if you want to get into the developmental stage you need to download prerequisites for your platform

**Windows**

On windows you don't have that much of a work. You can follow [this](https://blogs.msdn.microsoft.com/sujitdmello/2015/04/23/step-by-step-installation-instructions-for-getting-dnx-on-your-windows-machine/) tutorial to make WController work.

**Linux**

Setup on linux is fairly simple

* Clone this repository
* Open up a terminal and get into the WController main directory
* Execute ```chmod x+ linux_installscript.sh``` to assign proper permissions to setup script
* Just simply execute ```./linux_installscript.sh``` and done !!
* Restart your terminal and done !!

**OSX**

On Mac OSX, setup will require homebrew. If you haven't installed it yet,

Run

 ```/usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)" ```

on your terminal.

After,

* Clone this repository
* Open up a terminal and get into the WController main directory
* Execute ```chmod x+ osx_installscript.sh``` to assign proper permissions to setup script
* Just simply execute ```./osx_installscript.sh```
* Restart your terminal and done !!

# How to run ?

I you have run the installation script for your platform, you should be able to build or run the program with:

 ```dnu build ./src/WCS.MAIN/project.json ``` to build or  ```dnx -p ./src/WCS.MAIN ``` to run.
 
or you can build or run the test project with:

 ```dnu build ./test/WCS.TEST/project.json ``` to build or  ```dnx -p ./test/WCS.TEST test ``` to run.

# Development Progress - 41%
