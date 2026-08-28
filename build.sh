#!/bin/sh

cd /srv/marvin/repo/marvin2
git add .
git stash
git fetch
git pull

cd discord
sudo dotnet build ./discord.csproj -c Release

sudo cp -u ./bin/Release/net8.0/* /srv/marvin/prodsrv/

sudo systemctl restart marvin-discord.service