<p align="center"><img src="/screenshots/tricentis-logo.png" width="40%" alt="Tricentis Logo" /></p>

# NeoLoad Add-on for Tricentis Tosca

## Overview

C# extension to integrate [Tricentis Tosca](https://www.tricentis.com/) with [NeoLoad](https://www.neotys.com/neoload/overview) for SAP GUI Script maintenance.
It allows you to interact with the NeoLoad [Design API](https://www.neotys.com/documents/doc/neoload/latest/en/html/#11265.htm) to convert a Tricentis Tosca SAP GUI script to a NeoLoad SAP GUI User Path or update an existing SAP User Path.



| Property | Value |
| ----------------    | ----------------   |
| Maturity | Experimental |
| Author | Neotys |
| License           | [BSD 2-Clause "Simplified"](https://github.com/Neotys-Labs/Tricentis-Tosca/blob/master/LICENSE) |
| NeoLoad Licensing | License FREE edition, or Enterprise edition, or Professional with Integration & Advanced Usage|
| Supported versions | Tested with Tricentis Tosca version 11.3 and 12.0 and NeoLoad from version [6.6.0](https://www.neotys.com/support/download-neoload) version 32 bits
| Download Binaries | See the [latest release](https://github.com/Neotys-Labs/Tricentis-Tosca/releases/latest)|

## Setting up the NeoLoad Tricentis Tosca Add-on

### For version 12.2

1. Download the [latest release](https://github.com/Neotys-Labs/Tricentis-Tosca/releases/latest).

2. Unzip in the Tosca Commander directory of the Tricentis Tosca installation directory (for example: C:\Program Files (x86)\TRICENTIS\Tosca Testsuite\ToscaCommander).

3. Unzip in the TBox directory of the Tricentis Tosca installation directory (for example: C:\Program Files (x86)\TRICENTIS\Tosca Testsuite\TBox).

4. Unblock "ToscaCommander\NeoLoadAddOn.dll" (Right click the DLL > Properties and tick **Unblock**).

5. Unblock "TBox\NeoLoadAddOn.dll" (Right click the DLL > Properties and tick **Unblock**).

6. Edit the file **TBox\Tricentis.Automation.Agent.exe.config** in the installation directory of Tricentis Tosca:

* Add the following nodes at the end of the **runtime** node:
```xml
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
        <dependentAssembly>
            <assemblyIdentity name="Microsoft.Data.Edm" publicKeyToken="31bf3856ad364e35" culture="neutral" />
            <bindingRedirect oldVersion="0.0.0.0-5.8.4.0" newVersion="5.8.4.0" />
    	 </dependentAssembly>
    </assemblyBinding>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
        <dependentAssembly>
            <assemblyIdentity name="Microsoft.Data.Odata" publicKeyToken="31bf3856ad364e35" culture="neutral" />
            <bindingRedirect oldVersion="0.0.0.0-5.8.4.0" newVersion="5.8.4.0" />
        </dependentAssembly>
    </assemblyBinding>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
        <dependentAssembly>
            <assemblyIdentity name="Microsoft.OData.Core" publicKeyToken="31bf3856ad364e35" culture="neutral" />
            <bindingRedirect oldVersion="0.0.0.0-6.14.0.0" newVersion="6.14.0.0" />
    		<bindingRedirect oldVersion="6.15.0.0-7.3.0.0" newVersion="7.3.0.0" />
    		<codeBase version="6.14.0.0" href="nl-lib\Microsoft.OData.Core.dll" />
            <codeBase version="7.3.0.0" href="Microsoft.OData.Core.dll" />
        </dependentAssembly>
    </assemblyBinding>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
        <dependentAssembly>
            <assemblyIdentity name="System.Spatial" publicKeyToken="31bf3856ad364e35" culture="neutral" />
            <bindingRedirect oldVersion="0.0.0.0-5.8.4.0" newVersion="5.8.4.0" />
        </dependentAssembly>
    </assemblyBinding>
```
7. Relaunch the Tosca Commander.

### For version 11.3 and 12.0

1. Download the [latest release](https://github.com/Neotys-Labs/Tricentis-Tosca/releases/latest)

2. Unzip in the root folder of the Tricentis Tosca installation directory (for example: C:\Program Files (x86)\TRICENTIS\Tosca Testsuite\).

3. Unblock "NeoLoadAddOn.dll" (Right click the DLL > Properties and tick **Unblock**).

4. Edit the file **Tricentis.Automation.Agent.exe.config** in the installation directory of Tricentis Tosca:

* In the following node, replace **0.0.0.0-5.8.1.0** by **0.0.0.0-5.8.4.0**:
```xml
    <dependentAssembly>
        <assemblyIdentity name="Microsoft.Data.Edm" publicKeyToken="31bf3856ad364e35" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-5.8.1.0" newVersion="5.8.1.0" />
    </dependentAssembly>
```
* Add the following nodes at the end of the **assemblyBinding** node:
```xml
    <dependentAssembly>
        <assemblyIdentity name="Microsoft.OData.Core" publicKeyToken="31bf3856ad364e35" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-6.14.0.0" newVersion="6.14.0.0" />
		<bindingRedirect oldVersion="6.15.0.0-7.3.0.0" newVersion="7.3.0.0" />
		<codeBase version="6.14.0.0" href="nl-lib\Microsoft.OData.Core.dll" />
        <codeBase version="7.3.0.0" href="Microsoft.OData.Core.dll" />
     </dependentAssembly>
	 <dependentAssembly>
        <assemblyIdentity name="System.Spatial" publicKeyToken="31bf3856ad364e35" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-5.8.4.0" newVersion="5.8.1.0" />
     </dependentAssembly>
```
4. Relaunch the Tosca Commander.

**Warning**: You might need to launch both Tosca and SAP Logon as administrator in order to convert the Tosca Script to NeoLoad.

## Global Configuration

Go to **PROJECT** > **Options**, to define the NeoLoad Add-on settings:

<p align="center"><img src="/screenshots/options.png" alt="Options" /></p>

Parameters: 
* **NeoLoadApiPort**: The port of the NeoLoad API, by default it is 7400. 
* **NeoLoadApiKey**: The API Key specified in the NeoLoad project when identification is required. If no identification is required, this parameter can be left blank.
* **NeoLoadApiHostname**: The hostname of the machine that contains NeoLoad, by default it is localhost. It should be localhost for SAP GUI test case.

To access these values, go to the NeoLoad **Preferences**, then the **Project settings** tab, then select the **REST API** category.
<p align="center"><img src="/screenshots/designapi.png" alt="Design API" /></p>

## How to convert a Tricentis Tosca SAP script to a NeoLoad SAP User Path or update an existing SAP User Path.

In Tricentis Tosca, right click on an execution of an SAP Test Case and then **NeoLoad Add-on > Transfer to NeoLoad**

<p align="center"><img src="/screenshots/transfertoneoload.png" alt="transfer" /></p>

During the execution of the Tricentis Tosca test case, if the NeoLoad User Path does not exist, it will be created. Otherwise, the existing User Path will be updated thanks to the User Path Update feature.
The User Path Update feature merge the original User Path with a newer recording, copying variable extractors and variables. Below the SAP GUI User Path in NeoLoad.

<p align="center"><img src="/screenshots/userpath.png" alt="user path" /></p>

**Warning**: In Tosca 12.2, if executions errors are not displayed in Tosca Commander, you can see them in file neoload-add-on-error.txt located in you user profile directory. 

## ChangeLog

* Version 1.0.0 (November 30, 2018): Initial release.
* Version 1.1.0 (July 24, 2019): Support of Tosca version 12.2
