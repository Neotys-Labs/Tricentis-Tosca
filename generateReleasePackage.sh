#!/bin/bash

zip="/c/Program\ Files/7-Zip/7z.exe"

# Package for Tosca >= 12.2
rm -f Neoload_Add-on_Tosca12.2AndAbove.zip

cd installer
eval $zip a ../Neoload_Add-on_Tosca12.2AndAbove.zip ./*
cd ..

cd NeoloadTBoxProxy/bin/Release
mkdir -p TBox
cp NeoloadTBoxProxy.dll TBox
cp ../../../NeoLoadTBoxAddOn/bin/Release/ILMerge/NeoLoadTBoxAddOn.dll TBox
eval $zip a ../../../Neoload_Add-on_Tosca12.2AndAbove.zip TBox/
rm -rf TBox
cd ../../..

cd NeoLoadToscaCommanderAddOn/bin/Release
mkdir -p ToscaCommander
cp NeoLoadToscaCommanderAddOn.dll ToscaCommander
eval $zip a ../../../Neoload_Add-on_Tosca12.2AndAbove.zip ToscaCommander/
rm -rf ToscaCommander
