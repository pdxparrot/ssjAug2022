#! /bin/sh

# TODO: this should probably be a python script

cargo build

echo "Copying library ..."
cp target/debug/ssjaug2022.dll .
cp target/debug/libssjaug2022.so .
