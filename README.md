<p align="center"><img src="/screenshots/tricentis-logo.png" width="40%" alt="Tricentis Logo" /></p>

# NeoLoad Add-on for Tricentis Tosca

## Overview

C# extension to integrate [Tricentis Tosca](https://www.tricentis.com/) with [NeoLoad](https://www.neotys.com/neoload/overview) for SAP GUI and Web Script maintenance.
It allows you to interact with the NeoLoad [Design API](https://www.neotys.com/documents/doc/neoload/latest/en/html/#11265.htm) to convert a Tricentis Tosca SAP GUI or Web script to a NeoLoad User Path or update an existing User Path.



| Property | Value |
| ----------------    | ----------------   |
| Maturity | Stable |
| Author | Neotys |
| License           | [BSD 2-Clause "Simplified"](https://github.com/Neotys-Labs/Tricentis-Tosca/blob/master/LICENSE) |
| NeoLoad Licensing | License FREE edition, or Enterprise edition, or Professional with Integration & Advanced Usage|
| Supported versions | Tested with Tricentis Tosca version 11.3, 12.0, 12.2, 12.3, 13.0, 13.1, 13.2, 13.3 and NeoLoad from version [6.6.0](https://www.neotys.com/support/download-neoload) version 32 bits
| Download Binaries | See the [latest release](https://github.com/Neotys-Labs/Tricentis-Tosca/releases/latest)|

## Setting up the NeoLoad Tricentis Tosca Add-on

1. Download the [latest release](https://github.com/Neotys-Labs/Tricentis-Tosca/releases/latest) for your Tricentis Tosca version (either 12.1 and below or 12.2 and above).

2. Unzip it in the root folder of the Tricentis Tosca installation directory (for example: C:\Program Files (x86)\TRICENTIS\Tosca Testsuite\).

3. Unblock the following 3 DLL files by right clicking the DLL > Properties and tick **Unblock**
	* TRICENTIS\Tosca Testsuite\TBox\NeoLoadTBoxAddOn.dll
	* TRICENTIS\Tosca Testsuite\TBox\NeoloadTBoxProxy.dll
	* TRICENTIS\Tosca Testsuite\ToscaCommander\NeoLoadToscaCommanderAddOn.dll

4. Relaunch the Tosca Commander.

**Warning**: For SAP test case, you might need to launch both Tosca and SAP Logon as administrator in order to convert the Tosca Script to NeoLoad.

## Global Configuration

Go to **PROJECT** > **Options**, to define the NeoLoad Add-on settings:

<p align="center"><img src="/screenshots/options.png" alt="Options" /></p>

Parameters: 
* **NeoLoadApiPort**: The port of the NeoLoad API, by default it is 7400. 
* **NeoLoadApiKey**: The API Key specified in the NeoLoad project when identification is required. If no identification is required, this parameter can be left blank.
* **NeoLoadApiHostname**: The hostname of the machine that contains NeoLoad, by default it is localhost. It should be localhost for SAP GUI test case.
* **CreateTransactionBySapTCode**: Enable/Disable the creation of transaction in Neoload for each SAP TCode.

To access these values, go to the NeoLoad **Preferences**, then the **Project settings** tab, then select the **REST API** category.
<p align="center"><img src="/screenshots/designapi.png" alt="Design API" /></p>

### When enterprise proxy is set
During any transfert from Tosca to Neoload, the enterprise proxy settings must be disabled.
Before the transfert, go to windows proxy settings. Set "Automatically detect settings" and "Use setup script" to Off as shown below.
After the transfert complete, reset the settings at their initial value.
<p align="center"><img src="/screenshots/disable-enterprise-proxy.png" alt="Windows proxy seetings"/></p>

## How to convert a Tricentis Tosca SAP script to a NeoLoad User Path

In Tricentis Tosca, right click on an execution of a Test Case and then **NeoLoad Add-on > Transfer SAP test case to NeoLoad**
Neoload starts the **SAP recording** at the first step named "SAP" or "SAP Login", and stops it at the end of the test case.

<p align="center"><img src="/screenshots/transfertSAPtoNeoload.png" alt="transfer" /></p>

<p align="center"><img src="/screenshots/userpath.png" alt="user path" /></p>

## How to convert a Tricentis Tosca Web or API script to a NeoLoad User Path

In Tricentis Tosca, right click on an execution of a Test Case and then **NeoLoad Add-on > Transfer Web test case to NeoLoad**
Neoload starts the **Web recording** at the beginning of the test case, and stops it at the end.

<p align="center"><img src="/screenshots/transfertWEBtoNeoload.png" alt="transfer" /></p>

## User Path Update

During the execution of the Tricentis Tosca test case, if the NeoLoad User Path does not exist, it will be created. Otherwise, the existing User Path will be updated thanks to the User Path Update feature.
The User Path Update feature merges the original User Path with a newer recording, copying variable extractors and variables. Below the SAP GUI User Path in NeoLoad.

**Warning**: In Tosca > 12.2, if Execution errors are not displayed in Tosca Commander, they can be found in the **neoload-add-on-error.txt** file located in your user profile directory. 

## ChangeLog
* Version 2.2.0 (July 15, 2020): API test recording.
   * Support API test case recording
   * Support of Tosca version 13.3

* Version 2.1.0 (May 08, 2020): Make transactions when recording web test case.
   * Support of Tosca version 13.1 and 13.2

* Version 2.0.0 (April 27, 2020): Stabilization.
   * Support of Tosca version 13.0
   * Support Web test case recording
   * Create a new transaction in Neoload at each SAP TCode encountered during the recoding. This feature is available since Neoload 7.3
   * Make installation procedure easier

* Version 1.0.0 (November 30, 2018): Initial release.
   * July 24, 2019: Support of Tosca version 12.2
