<p align="center"><img src="/screenshots/tricentis-logo.png" width="40%" alt="Tricentis Logo" /></p>

# NeoLoad Add On for Tricentis Tosca

## Overview

C# extension to integrate [Tricentis Tosca](https://www.tricentis.com/) with [NeoLoad](https://www.neotys.com/neoload/overview) for SAP Script maintenance.
It allows you to interact with the NeoLoad [Design API](https://www.neotys.com/documents/doc/neoload/latest/en/html/#11265.htm) to convert a Tricentis Tosca SAP script to a NeoLoad SAP User Path or update an existing SAP User Path.



| Property | Value |
| ----------------    | ----------------   |
| Maturity | Experimental |
| Author | Neotys |
| License           | [BSD 2-Clause "Simplified"](https://github.com/Neotys-Labs/Tricentis-Tosca/blob/master/LICENSE) |
| NeoLoad Licensing | License FREE edition, or Enterprise edition, or Professional with Integration & Advanced Usage|
| Tricentis Licensing | [TODO Tricentis Tosca license]() or a free [15-day trial](https://www.tricentis.com/testing-tool-trial/) |
| Supported versions | Tested with Tricentis Tosca version 11.3 and 12.0 and NeoLoad version [6.7.0](https://www.neotys.com/support/download-neoload)|
| Download Binaries | See the [latest release](https://github.com/Neotys-Labs/Tricentis-Tosca/releases/latest)|

## Setting up the NeoLoad Tricentis Tosca Add On.

1. Download the [latest release](https://github.com/Neotys-Labs/Tricentis-Tosca/releases/latest)

2. Unzip in the root folder of the Tricentis Tosca installation directory (for example: C:\Program Files (x86)\TRICENTIS\Tosca Testsuite\).

3. Edit the file **Tricentis.Automation.Agent.exe.config** in the installation directory of Tricentis Tosca :

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
4. Relaunch the Tosca Commander

## Global Configuration

Go to PROJECT > Options, to define the NeoLoad Add On settings:

<p align="center"><img src="/screenshots/options.png" alt="Options" /></p>

Parameters: 
* **NeoLoadApiPort**: The port of the NeoLoad API, by default it is 7400. 
* **NeoLoadApiKey**: The API Key specified in the NeoLoad project when identification is required. If no identification is required, this parameter can be left blank.
* **NeoLoadApiHostname**: The hostname of the machine that contains NeoLoad, by default it is localhost.

To access these values, go to the NeoLoad **Preferences**, then the **Project settings** tab, then select the **REST API** category.
<p align="center"><img src="/screenshots/designapi.png" alt="Design API" /></p>

## How to convert a Tricentis Tosca SAP script to a NeoLoad SAP User Path or update an existing SAP User Path.

In Tricentis Tosca, right click on an execution of a SAP Test Case and then **NeoLoad AddOn > Transfert to NeoLoad**

<p align="center"><img src="/screenshots/transferttoneoload.png" alt="Design API" /></p>

During the execution of the Tricentis Tosca test case, if the NeoLoad User Path does not exist, it will be created. Otherwise, the existing User Path will be updated. 

## ChangeLog

* Version 1.0.0 (November 30, 2018): Initial release.
