#!/bin/sh

help() {
cat << __EOF

  binaries:

    /usr/bin/picocom
    /usr/bin/ascii-xfr

  packages:

    /var/lib/dpkg/info/picocom.list:/usr/bin/picocom
    /var/lib/dpkg/info/minicom.list:/usr/bin/ascii-xfr
__EOF
}

if [ $# -lt 1 ]; then
    echo "Not enough arguments"
    echo "Use:  ./picot.sh /dev/ttyACM0"
    exit 1
fi

if [ $1 = "-help" ] || [ $1 = "--help" ]; then
    help
    exit 0
fi

echo "Control A, Control Q to quit"
echo "Control A, Control S to send a file from the local directory"
# used to be may 2020: picocom -f n -p n -d 8 -b 115200 --omap delbs --send-cmd "ascii-xfr -sn -l 150 -c 25" ${1}
picocom -f n -p n -d 8 -b 115200 --omap delbs --send-cmd "ascii-xfr -s -l 1850 -c 10" ${1}

exit 0

