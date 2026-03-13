#!/bin/bash

if [[ $# -gt 0 ]]; then
  ts=$1
else
  ts=`date +%Y%m%d`
fi

dir="ROSCO_${ts}"

[[ -d $dir ]] && rm -rf $dir

mkdir $dir

cp -r RockCollect/bin/Release/* $dir

zip -rp "${dir}.zip" $dir
