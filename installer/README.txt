# How to install the NeoLoad Tosca Addon

## Automatic Install

You can either

1. Double-click on `Install-NeoLoadToscaAddon.bat`
2. Follow the prompted steps

OR

1. Open Windows PowerShell as an administrator
2. Change the current location to this folder
3. Execute the `Install-NeoLoadToscaAddon.ps1` script
4. Follow the prompted steps

> **Warning** : If you don't have the administrator rights, or your PowerShell Script Execution Policy doesn't allow you to run the script, you may choose to do a manual install.

## Manual Install

1. Copy the following folders to the root folder of the Tricentis Tosca installation directory (for example: C:\Program Files (x86)\TRICENTIS\Tosca Testsuite\).
    * TBox
    * ToscaCommander

3. Unblock the following 3 DLL files by right clicking the DLL > Properties and tick **Unblock**
	* TRICENTIS\Tosca Testsuite\TBox\NeoLoadTBoxAddOn.dll
	* TRICENTIS\Tosca Testsuite\TBox\NeoloadTBoxProxy.dll
	* TRICENTIS\Tosca Testsuite\ToscaCommander\NeoLoadToscaCommanderAddOn.dll

4. Relaunch the Tosca Commander.

