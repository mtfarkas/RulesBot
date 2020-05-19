#!/bin/bash

sudo /bin/systemctl disable rules-bot
sudo /bin/systemctl stop rules-bot

rm -rf ~/proj/rulesbot/*
unzip ~/payload/dist.zip -d ~/proj/rulesbot
chmod +x ~/proj/rulesbot/RulesBot
rm -rf ~/payload/*

sudo /bin/systemctl start rules-bot
sudo /bin/systemctl enable rules-bot