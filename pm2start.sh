#!/bin/sh
BASE=$HOME
NERO_LAUNCHSCRIPT=$BASE/Nero/start.sh
pm2 start ${NERO_LAUNCHSCRIPT} --name "Nero"
pm2 save
pm2 startup
