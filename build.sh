#!/bin/sh
# filepath: marvin2/build.sh

cd /srv/marvin/repo/marvin2
echo "Cleaning repo"
git add .
git stash

echo "Updating repo"
git fetch

# Record current commit before pulling
OLD_HEAD=$(git rev-parse HEAD)

git pull

echo "Checking build script version"
# Record new commit after pulling
NEW_HEAD=$(git rev-parse HEAD)

# Check if build.sh changed between the two commits
if git diff --name-only "$OLD_HEAD" "$NEW_HEAD" | grep -q '^build\.sh$'; then
    echo "build.sh was updated in this pull. Exiting so you can review changes before rebuilding."
    exit 1
fi

echo "Building discord project"

cd discord
sudo dotnet build ./discord.csproj -c Release

sudo cp -u -r ./bin/Release/net8.0/* /srv/marvin/prodsrv/

echo "Starting discord application"
sudo systemctl restart marvin-discord.service