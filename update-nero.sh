#!/usr/bin/env bash
BASE=$HOME
PUBLIC_NERO_DIR=$BASE/Nero
PUBLIC_NERO_PM2_INSTANCEID="Nero-Public"
cd ${PUBLIC_NERO_DIR}
`which pm2` stop ${PUBLIC_NERO_PM2_INSTANCEID}
`which git` pull
`which pm2` start ${PUBLIC_NERO_PM2_INSTANCEID}
