#!/bin/bash

ts=`date +%Y%m%d`

dir="ROSCO_${ts}"

[[ -d $dir ]] && rm -rf $dir

mkdir $dir

cp -r RockCollect/bin/Release/* $dir

zip -rp "${dir}.zip" $dir
