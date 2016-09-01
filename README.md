# WirelessControl
Wireless control is a cross-platform server module for remote applications that uses TCP/IP Protocol.
# Features (to be implemented) 
* OS Controls like: sound, input controls(mouse/keyboard etc)
* Application executions
* Operation timers
* Setup OS Tasks
* Custom code execution (plugin development)
* 100% Free

# How to install and run ?

Currently, WController is not operational. However, if you want to get into the developmental stage you need to download prerequisites for your platform

**Windows**

On windows you don't have that much of a work. You can follow [this](https://blogs.msdn.microsoft.com/sujitdmello/2015/04/23/step-by-step-installation-instructions-for-getting-dnx-on-your-windows-machine/) tutorial to make WController work.

**Linux**

Linux needs a few packages to work:

* libasound2-data
* libasound2
* libasound2-dev
* mono-complete
* curl

After you get done with packages, download/clone this repo and execute these commands on your terminal
 
```curl -sSL https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.sh | DNX_BRANCH=dev sh && source ~/.dnx/dnvm/dnvm.sh ```

```dnvm upgrade ```

get to the WController main directory

```dnu restore ```

now you can build or run the program with

```dnu build ./src/WCS.MAIN/project.json ```to build or ```dnx -p ./src/WCS.MAIN``` to run.

or you can build or run the test project with: 

```dnu build ./test/WCS.TEST/project.json``` to build or ```dnx -p ./test/WCS.TEST test``` to run.

**OSX**

Coming soon

# Development Progress - 25%
