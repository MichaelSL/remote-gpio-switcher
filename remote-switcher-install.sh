echo 'Will need sudo to install a service'
sudo echo 'sudo OK'
echo 'dotnet version'
dotnet --version
mkdir /etc/rs
unzip rs.zip -d /etc/rs
cd /etc/rs
chmod +x GPIO.Service.Cmd.dll
sudo ln -s /etc/rs/remote-switcher.service /etc/systemd/system/remote-switcher.service
sudo systemctl restart remote-switcher.service