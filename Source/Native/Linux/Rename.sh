#!/bin/sh

# Match all instances of ZEDTEMPLATE and ZEDTemplate, replacing with the user's
# own project name
# Debug messages will use the name with spaces (use double-quotes for project
# names with spaces), a second parameter will determine the delimiter to use.
# If this is not entered, the space shall be removed.  A third parameter will
# specify whether or not to capitalise every starting letter after a space.

PROJECTNAME=`echo $1`
shopt -s nocasematch

if [[ $3 == "upper" ]]
then
	Array=($1)
	TMPNAME=""
	for s in "${Array[@]}";do
		String=${s^}
		TMPNAME+=$String
	done
	PROJECTNAME=$TMPNAME
fi

if [[ $3 == "lower" ]]
then
	Array=($1)
	TMPNAME=""
	for s in "${Array[@]}";do
		String=${s,}
		TMPNAME+=$String
	done
	PROJECTNAME=$TMPNAME
fi

if [[ $2 == "" ]]
then
	PROJECTNAME=`echo $PROJECTNAME | sed "s/ //g"`
else
	PROJECTNAME=`echo $PROJECTNAME | sed "s/ /$2/g"`
fi

echo Renaming all instances of ZEDTEMPLATE to `echo $PROJECTNAME | tr '[:lower:]' '[:upper:]'`
echo Renaming all instances of ZEDTemplate to $PROJECTNAME
echo Renaming all instances of ZED Template to $1

sed -i "s/ZEDTEMPLATE/`echo $PROJECTNAME | tr '[:lower:]' '[:upper:]'`/g" ./Headers/*.hpp
sed -i "s/ZEDTemplate/$PROJECTNAME/g" ./Headers/*.hpp
sed -i "s/ZED\ Template/$1/g" ./Headers/*.hpp
sed -i "s/ZEDTEMPLATE/`echo $PROJECTNAME | tr '[:lower:]' '[:upper:]'`/g" ./Source/*.cpp
sed -i "s/ZEDTemplate/$PROJECTNAME/g" ./Source/*.cpp
sed -i "s/ZED\ Template/$1/g" ./Source/*.cpp
sed -i "s/ZEDTEMPLATE/`echo $PROJECTNAME | tr '[:lower:]' '[:upper:]'`/g" ./Makefile
sed -i "s/ZEDTemplate/$PROJECTNAME/g" ./Makefile
sed -i "s/ZED\ Template/$1/g" ./Makefile
sed -i "s/ZEDTEMPLATE/`echo $FORMATTED | tr '[:lower:]' '[:upper:]'`/g" ../Common/Headers/*.hpp
sed -i "s/ZEDTemplate/$PROJECTNAME/g" ../Common/Headers/*.hpp
sed -i "s/ZED\ Template/$1/g" ../Common/Headers/*.hpp
sed -i "s/ZEDTEMPLATE/`echo $PROJECTNAME | tr '[:lower:]' '[:upper:]'`/g" ../Common/Source/*.cpp
sed -i "s/ZEDTemplate/$PROJECTNAME/g" ../Common/Source/*.cpp
sed -i "s/ZED\ Template/$1/g" ../Common/Source/*.cpp
sed -i "s/zedtemplate\.config/`echo $PROJECTNAME.config | tr '[:upper:]' '[:lower:]'`/g" ../Common/Source/Configuration.cpp
