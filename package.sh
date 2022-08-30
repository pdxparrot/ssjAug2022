#! /bin/sh

BUILDS=builds
ZIPFILE="Crimson Crescendo.zip"

rm -f "$ZIPFILE"

cd $BUILDS
zip -r "../$ZIPFILE" *
