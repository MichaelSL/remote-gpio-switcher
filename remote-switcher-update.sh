echo 'Will need sudo to update a service'
sudo echo 'sudo OK'
echo 'dotnet version'
dotnet --version
sudo systemctl stop remote-switcher.service
mkdir -p /etc/rs
find /etc/rs ! -name 'remote-switcher.service' -type f,d -exec rm -rf {} +
unzip -n rs.zip -d /etc/rs
cd /etc/rs
chmod +x GPIO.Service.Cmd.dll
sudo systemctl restart remote-switcher.service