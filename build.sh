#!/bin/sh
# filepath: marvin2/build.sh
set -e

install_discord(){
    SERVICE_FILE=/etc/systemd/system/marvin-discord.service
    EXPECTED_CONTENT=$(cat <<'EOF'
[Unit]
Description=Discord bot for handling information for the household.

[Service]
WorkingDirectory=/srv/marvin/prodsrv
ExecStart=/usr/bin/dotnet /srv/marvin/prodsrv/discord.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=marvin-discord
User=malechus
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
EOF
)

    if [ ! -f "$SERVICE_FILE" ] || [ "$(cat "$SERVICE_FILE")" != "$EXPECTED_CONTENT" ]; then
        echo "Updating $SERVICE_FILE"
        echo "$EXPECTED_CONTENT" | sudo tee "$SERVICE_FILE" > /dev/null
        sudo systemctl daemon-reload
    fi
}

install_web(){
    SERVICE_FILE=/etc/systemd/system/marvin-web.service
    EXPECTED_CONTENT=$(cat <<'EOF'
[Unit]
Description=Web dashboard for handling information for the household.

[Service]
WorkingDirectory=/srv/marvin/web
ExecStart=/usr/bin/dotnet /srv/marvin/web/web.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=marvin-web
User=malechus
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
EOF
)

    if [ ! -f "$SERVICE_FILE" ] || [ "$(cat "$SERVICE_FILE")" != "$EXPECTED_CONTENT" ]; then
        echo "Updating $SERVICE_FILE"
        echo "$EXPECTED_CONTENT" | sudo tee "$SERVICE_FILE" > /dev/null
        sudo systemctl daemon-reload
    fi
}

cd /srv/marvin/repo/marvin2 || exit 1
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

echo "Building discord application"

cd /srv/marvin/ || exit 2
sudo mkdir -p prodsrv
cd /srv/marvin/repo/marvin2/discord || exit 2
sudo dotnet build ./discord.csproj -c Release

sudo cp -u -r ./bin/Release/net8.0/* /srv/marvin/prodsrv/

echo "Installing discord application"
install_discord

echo "Starting discord application"
sudo systemctl restart marvin-discord.service

echo "Building web application"

cd /srv/marvin/ || exit 2
sudo mkdir -p web
cd /srv/marvin/repo/marvin2/web || exit 2
sudo dotnet build ./web.csproj -c Release

sudo cp -u -r ./bin/Release/net8.0/* /srv/marvin/web/

echo "Installing web application"
install_web

echo "Starting web application"
sudo systemctl restart marvin-web.service