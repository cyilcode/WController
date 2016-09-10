#Author: Cem YILMAZ
#10/09/2016
#!/bin/sh
GREEN_LINE='\033[0;32m'
RED_LINE='\033[0;31m'
ORANGE_LINE='\033[0;33m'
DEFAULT_LINE='\033[0m'

if [ ! -f global.json ]; then # a really simple project directory check
    echo "You must be in WController directory to run this script."
    exit 1
fi

echo -e ${GREEN_LINE}Checking and installing Mono…${DEFAULT_LINE}
brew update
brew install mono

echo -e ${GREEN_LINE}Dependency installation is complete. Checking DNVM now....${DEFAULT_LINE}

brew tap aspnet/dnx
brew update
brew install dnvm

echo -e ${ORANGE_LINE}Checking and configuring zshrc file${DEFAULT_LINE}
   if [ ! -f ~/.bash_profile ]; then
     echo -e ${RED_LINE}Couldn’t find .bash_profile file.${GREEN_LINE} Creating now.${DEFAULT_LINE}
     touch ~/.bash_profile   
   fi

(echo ""; echo "source dnvm.sh") >> ~/.bash_profile
source ~/.bash_profile
echo -e ${ORANGE_LINE}Executing dnvm upgrade${DEFAULT_LINE}
dnvm upgrade
echo -e ${GREEN_LINE}Executing dnu restore now..${DEFAULT_LINE}
dnu restore
echo -e ${GREEN_LINE}Please refer to https://github.com/cyilcode/WController/blob/master/README.md for further information${DEFAULT_LINE}
echo -e ${ORANGE_LINE}Please restart your terminal to complete the installation${DEFAULT_LINE}


