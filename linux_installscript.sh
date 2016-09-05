#Author: Cem YILMAZ
#05/09/2016
if [ ! -f global.json ]; then # a really simple project directory check
    echo "You must be in WController directory to run this script."
    exit 1
fi
GREEN_LINE='\033[0;32m'
RED_LINE='\033[0;33m'
pkgs=("libasound2-dev", "libxdo-dev", "curl", "mono-complete")
for (( i=0; i<${#pkgs[@]}; i++))
do
  pkgname=${pkgs[i]%","}
  PKG_OK=$(dpkg-query -W --showformat='${Status}\n' $pkgname|grep "install ok installed")
  echo Checking for $pkgname: $PKG_OK
  if [ "" == "$PKG_OK" ]; then
    echo "No $pkgname. Setting up $pkgname."
    sudo apt-get --force-yes --yes install $pkgname
  fi
done

echo -e "${GREEN_LINE}Dependency installation is complete. Checking DNVM now...."

if [ ! -f $HOME/.dnx/dnvm/dnvm.sh ]; then
	echo "DNVM is not installed. Installing DNVM now..."
	dnvm_install=$(curl -sSL https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.sh | DNX_BRANCH=dev sh && source ~/.dnx/dnvm/dnvm.sh)
	echo -e Executing curl $dnvm_install
fi

dnu_restore=$(dnu restore)
printf "Executing .NET Package Restore %s\n" "$dnu_restore"
echo -e "${GREEN_LINE}Installation is done. Please refer to https://github.com/cyilcode/WController/blob/master/README.md for further information"
echo -e "${RED_LINE}Please consider the Warning message on Project README.md file !!"