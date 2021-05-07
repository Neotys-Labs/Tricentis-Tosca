$NEOLOAD_ADDON_NAME = 'NeoLoad Add-on for Tricentis Tosca'
$TOSCA_SOFTWARE_NAME = 'Tosca'
$FOLDERS_TO_PROCESS = @('TBox', 'ToscaCommander')
$FILES_TO_PROCESS = @('NeoLoadTBoxAddOn.dll', 'NeoloadTBoxProxy.dll', 'NeoLoadToscaCommanderAddOn.dll')

$ErrorActionPreference = 'Stop'

$INITIAL_DIR = $args[0]

# Step functions
function Invoke-Bootstrap {
    if($INITIAL_DIR) {
        Set-Location $INITIAL_DIR
    }
    Invoke-StepOne
}
function Invoke-StepOne {
    Write-Host "You are about to install $NEOLOAD_ADDON_NAME."
    Write-Host "This script requires administrator rights to be able to complete." -ForegroundColor Yellow
    $Choice = Read-Host -Prompt ("Would you like to proceed? (Y/n)")
    if (($Choice -eq 'Y') -or ($Choice -eq 'y')) {
        Invoke-StepTwo
    }
    else {
        Write-Host 'Operation aborted.' -ForegroundColor Red
    }
}

function Invoke-StepTwo {
    $RegistryPaths = @(
        "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\*"
        "HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*"
    )
    $Results = @((Get-ItemProperty $RegistryPaths) -Match $TOSCA_SOFTWARE_NAME | Where-Object { $_.InstallLocation })
    if ($Results.Length -gt 1) {
        $Title = ("We have found several installations of $TOSCA_SOFTWARE_NAME installed on your machine.")
        $Prompt = "Choose one installation path"
        $Counter = 0
        $Choices = $Results | ForEach-Object { New-Object System.Management.Automation.Host.ChoiceDescription ("&$Counter - " + $_.InstallLocation) ; $Counter++ }
        $Default = 0
        $Choice = $Host.UI.PromptForChoice($Title, $Prompt, $Choices, $Default)
        Invoke-StepThree($Results[$Choice].InstallLocation)
    }
    elseif ($Results.Length -eq 1) {
        Invoke-StepThree($Results[0].InstallLocation)
    }
    else {
        Write-Host ("No installation of $TOSCA_SOFTWARE_NAME could be found. Make sure it is correctly installed before running this script.") -ForegroundColor Red
    }
}

function Invoke-StepThree {
    param (
        [string]$InstallLocation
    )
    $FoldersToCopy = Get-ChildItem | Where-Object { $_ -in $FOLDERS_TO_PROCESS }
    if($FoldersToCopy.Length -eq $FOLDERS_TO_PROCESS.Length) {
        $FoldersToCopy | ForEach-Object { 
            Write-Host ("Copying folder '$_' to destination : '$InstallLocation'");
            Copy-Item -Path $_ -Destination $InstallLocation -Force -Recurse;
        }
        $FilesToUnblock = Get-ChildItem -Path $InstallLocation -Recurse | 
            Where-Object { $_ -in $FOLDERS_TO_PROCESS } | 
            Get-ChildItem | 
            Where-Object { $_ -in $FILES_TO_PROCESS } | 
            ForEach-Object { $_.FullName }
        if($FilesToUnblock.Length -eq $FILES_TO_PROCESS.Length) {
            $FilesToUnblock | ForEach-Object {
                Invoke-UnblockFile($_);
            }
            Invoke-StepFour            
        }
        else {
            Write-Error "Source files to copy could not be located"
        }         
    } else {
        Write-Error "Source folders to copy could not be located"
    }
}

function Invoke-StepFour {
    Write-Host ("$NEOLOAD_ADDON_NAME has been successfully installed. You must restart $TOSCA_SOFTWARE_NAME before you can see the changes being applied.") -ForegroundColor Green;
}

# Helper functions
function Invoke-UnblockFile {
    param(
        [string]$File
    )
    Write-Host ("Unblocking file '$File'");
    Unblock-File -Path $File
}

# Bootstrap
Invoke-Bootstrap

# SIG # Begin signature block
# MIIZbQYJKoZIhvcNAQcCoIIZXjCCGVoCAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUYOUXSO8cY58Y891czFpiUOyx
# ZjegghR7MIIE/jCCA+agAwIBAgIQDUJK4L46iP9gQCHOFADw3TANBgkqhkiG9w0B
# AQsFADByMQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYD
# VQQLExB3d3cuZGlnaWNlcnQuY29tMTEwLwYDVQQDEyhEaWdpQ2VydCBTSEEyIEFz
# c3VyZWQgSUQgVGltZXN0YW1waW5nIENBMB4XDTIxMDEwMTAwMDAwMFoXDTMxMDEw
# NjAwMDAwMFowSDELMAkGA1UEBhMCVVMxFzAVBgNVBAoTDkRpZ2lDZXJ0LCBJbmMu
# MSAwHgYDVQQDExdEaWdpQ2VydCBUaW1lc3RhbXAgMjAyMTCCASIwDQYJKoZIhvcN
# AQEBBQADggEPADCCAQoCggEBAMLmYYRnxYr1DQikRcpja1HXOhFCvQp1dU2UtAxQ
# tSYQ/h3Ib5FrDJbnGlxI70Tlv5thzRWRYlq4/2cLnGP9NmqB+in43Stwhd4CGPN4
# bbx9+cdtCT2+anaH6Yq9+IRdHnbJ5MZ2djpT0dHTWjaPxqPhLxs6t2HWc+xObTOK
# fF1FLUuxUOZBOjdWhtyTI433UCXoZObd048vV7WHIOsOjizVI9r0TXhG4wODMSlK
# XAwxikqMiMX3MFr5FK8VX2xDSQn9JiNT9o1j6BqrW7EdMMKbaYK02/xWVLwfoYer
# vnpbCiAvSwnJlaeNsvrWY4tOpXIc7p96AXP4Gdb+DUmEvQECAwEAAaOCAbgwggG0
# MA4GA1UdDwEB/wQEAwIHgDAMBgNVHRMBAf8EAjAAMBYGA1UdJQEB/wQMMAoGCCsG
# AQUFBwMIMEEGA1UdIAQ6MDgwNgYJYIZIAYb9bAcBMCkwJwYIKwYBBQUHAgEWG2h0
# dHA6Ly93d3cuZGlnaWNlcnQuY29tL0NQUzAfBgNVHSMEGDAWgBT0tuEgHf4prtLk
# YaWyoiWyyBc1bjAdBgNVHQ4EFgQUNkSGjqS6sGa+vCgtHUQ23eNqerwwcQYDVR0f
# BGowaDAyoDCgLoYsaHR0cDovL2NybDMuZGlnaWNlcnQuY29tL3NoYTItYXNzdXJl
# ZC10cy5jcmwwMqAwoC6GLGh0dHA6Ly9jcmw0LmRpZ2ljZXJ0LmNvbS9zaGEyLWFz
# c3VyZWQtdHMuY3JsMIGFBggrBgEFBQcBAQR5MHcwJAYIKwYBBQUHMAGGGGh0dHA6
# Ly9vY3NwLmRpZ2ljZXJ0LmNvbTBPBggrBgEFBQcwAoZDaHR0cDovL2NhY2VydHMu
# ZGlnaWNlcnQuY29tL0RpZ2lDZXJ0U0hBMkFzc3VyZWRJRFRpbWVzdGFtcGluZ0NB
# LmNydDANBgkqhkiG9w0BAQsFAAOCAQEASBzctemaI7znGucgDo5nRv1CclF0CiNH
# o6uS0iXEcFm+FKDlJ4GlTRQVGQd58NEEw4bZO73+RAJmTe1ppA/2uHDPYuj1UUp4
# eTZ6J7fz51Kfk6ftQ55757TdQSKJ+4eiRgNO/PT+t2R3Y18jUmmDgvoaU+2QzI2h
# F3MN9PNlOXBL85zWenvaDLw9MtAby/Vh/HUIAHa8gQ74wOFcz8QRcucbZEnYIpp1
# FUL1LTI4gdr0YKK6tFL7XOBhJCVPst/JKahzQ1HavWPWH1ub9y4bTxMd90oNcX6X
# t/Q/hOvB46NJofrOp79Wz7pZdmGJX36ntI5nePk2mOHLKNpbh6aKLzCCBQwwggP0
# oAMCAQICEAJjQrZe+YpD6kB79Qc4XpwwDQYJKoZIhvcNAQELBQAwcjELMAkGA1UE
# BhMCVVMxFTATBgNVBAoTDERpZ2lDZXJ0IEluYzEZMBcGA1UECxMQd3d3LmRpZ2lj
# ZXJ0LmNvbTExMC8GA1UEAxMoRGlnaUNlcnQgU0hBMiBBc3N1cmVkIElEIENvZGUg
# U2lnbmluZyBDQTAeFw0yMDA5MDEwMDAwMDBaFw0yMzA5MDYxMjAwMDBaMEkxCzAJ
# BgNVBAYTAkZSMRAwDgYDVQQHEwdHZW1lbm9zMRMwEQYDVQQKEwpOZW90eXMgU0FT
# MRMwEQYDVQQDEwpOZW90eXMgU0FTMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIB
# CgKCAQEArPl/MTJxlDC3h5PQU7RVp/iCjii9DsWzdPROOjw6Gpj25J3T1G2Vhe02
# QnbFs/Y7rxRe6tKYvRvvQ3ug7Z+PfnGuMa/QWwP9g8uhjF+qilImBHeQo+V0qcwR
# 0v33AZi40FgQorPHxDXyj+KII4HULENC2Qfh/sWAXYDDe44JMuLzEuzWM0Zq3kH6
# d3scQUG3IM/AsEv0bd0gWH6S33tPC9D4na12B+BpNVM22kipf0jpKb9vR7uU6CHu
# glE2KtmFH2NweIR2eL1FCJEuFowyTWZ1mmJP9nyq5VJBg/QMrx7d1bONmhKnPhMt
# CZOQDr66iRhdJwrItPse6bRybQ9W3QIDAQABo4IBxTCCAcEwHwYDVR0jBBgwFoAU
# WsS5eyoKo6XqcQPAYPkt9mV1DlgwHQYDVR0OBBYEFNC14/njjkJaFZZ9yNEXSo+O
# fhtDMA4GA1UdDwEB/wQEAwIHgDATBgNVHSUEDDAKBggrBgEFBQcDAzB3BgNVHR8E
# cDBuMDWgM6Axhi9odHRwOi8vY3JsMy5kaWdpY2VydC5jb20vc2hhMi1hc3N1cmVk
# LWNzLWcxLmNybDA1oDOgMYYvaHR0cDovL2NybDQuZGlnaWNlcnQuY29tL3NoYTIt
# YXNzdXJlZC1jcy1nMS5jcmwwTAYDVR0gBEUwQzA3BglghkgBhv1sAwEwKjAoBggr
# BgEFBQcCARYcaHR0cHM6Ly93d3cuZGlnaWNlcnQuY29tL0NQUzAIBgZngQwBBAEw
# gYQGCCsGAQUFBwEBBHgwdjAkBggrBgEFBQcwAYYYaHR0cDovL29jc3AuZGlnaWNl
# cnQuY29tME4GCCsGAQUFBzAChkJodHRwOi8vY2FjZXJ0cy5kaWdpY2VydC5jb20v
# RGlnaUNlcnRTSEEyQXNzdXJlZElEQ29kZVNpZ25pbmdDQS5jcnQwDAYDVR0TAQH/
# BAIwADANBgkqhkiG9w0BAQsFAAOCAQEAd5YUD5Fyatwe0KWRERkwPzha8uk7RWVS
# Tw4BkWh55tFjiEAdyzeNYwyoxMsHgFuoPmz/VBCQrDtBhqO6I/fX2PNvSdzPsEvl
# 7DvFSNW/uD/SJg5/6Q1rzEhkThbkuJ8FWBUnJ0GB4ZiGUrzSckp6P/xXkhzZp+uy
# JxQdLhwHaL4HI1fe1LRksdztmP/VUNKcT6+5A0ZFy8JytNb4EAsY6w0h7HxAXlvT
# ID1YnjqdX6xtaUyONncgX2Hhr6iUeJR9jcprz7dkn3EMYu5gm4/3ML5qAb/1UprV
# 47UgmwbC4w6fYBdcRITC6hj2XenIwj2lyV3auhhlLt8ZinwrqO+LCjCCBTAwggQY
# oAMCAQICEAQJGBtf1btmdVNDtW+VUAgwDQYJKoZIhvcNAQELBQAwZTELMAkGA1UE
# BhMCVVMxFTATBgNVBAoTDERpZ2lDZXJ0IEluYzEZMBcGA1UECxMQd3d3LmRpZ2lj
# ZXJ0LmNvbTEkMCIGA1UEAxMbRGlnaUNlcnQgQXNzdXJlZCBJRCBSb290IENBMB4X
# DTEzMTAyMjEyMDAwMFoXDTI4MTAyMjEyMDAwMFowcjELMAkGA1UEBhMCVVMxFTAT
# BgNVBAoTDERpZ2lDZXJ0IEluYzEZMBcGA1UECxMQd3d3LmRpZ2ljZXJ0LmNvbTEx
# MC8GA1UEAxMoRGlnaUNlcnQgU0hBMiBBc3N1cmVkIElEIENvZGUgU2lnbmluZyBD
# QTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAPjTsxx/DhGvZ3cH0wsx
# SRnP0PtFmbE620T1f+Wondsy13Hqdp0FLreP+pJDwKX5idQ3Gde2qvCchqXYJawO
# eSg6funRZ9PG+yknx9N7I5TkkSOWkHeC+aGEI2YSVDNQdLEoJrskacLCUvIUZ4qJ
# RdQtoaPpiCwgla4cSocI3wz14k1gGL6qxLKucDFmM3E+rHCiq85/6XzLkqHlOzEc
# z+ryCuRXu0q16XTmK/5sy350OTYNkO/ktU6kqepqCquE86xnTrXE94zRICUj6whk
# PlKWwfIPEvTFjg/BougsUfdzvL2FsWKDc0GCB+Q4i2pzINAPZHM8np+mM6n9Gd8l
# k9ECAwEAAaOCAc0wggHJMBIGA1UdEwEB/wQIMAYBAf8CAQAwDgYDVR0PAQH/BAQD
# AgGGMBMGA1UdJQQMMAoGCCsGAQUFBwMDMHkGCCsGAQUFBwEBBG0wazAkBggrBgEF
# BQcwAYYYaHR0cDovL29jc3AuZGlnaWNlcnQuY29tMEMGCCsGAQUFBzAChjdodHRw
# Oi8vY2FjZXJ0cy5kaWdpY2VydC5jb20vRGlnaUNlcnRBc3N1cmVkSURSb290Q0Eu
# Y3J0MIGBBgNVHR8EejB4MDqgOKA2hjRodHRwOi8vY3JsNC5kaWdpY2VydC5jb20v
# RGlnaUNlcnRBc3N1cmVkSURSb290Q0EuY3JsMDqgOKA2hjRodHRwOi8vY3JsMy5k
# aWdpY2VydC5jb20vRGlnaUNlcnRBc3N1cmVkSURSb290Q0EuY3JsME8GA1UdIARI
# MEYwOAYKYIZIAYb9bAACBDAqMCgGCCsGAQUFBwIBFhxodHRwczovL3d3dy5kaWdp
# Y2VydC5jb20vQ1BTMAoGCGCGSAGG/WwDMB0GA1UdDgQWBBRaxLl7KgqjpepxA8Bg
# +S32ZXUOWDAfBgNVHSMEGDAWgBRF66Kv9JLLgjEtUYunpyGd823IDzANBgkqhkiG
# 9w0BAQsFAAOCAQEAPuwNWiSz8yLRFcgsfCUpdqgdXRwtOhrE7zBh134LYP3DPQ/E
# r4v97yrfIFU3sOH20ZJ1D1G0bqWOWuJeJIFOEKTuP3GOYw4TS63XX0R58zYUBor3
# nEZOXP+QsRsHDpEV+7qvtVHCjSSuJMbHJyqhKSgaOnEoAjwukaPAJRHinBRHoXpo
# aK+bp1wgXNlxsQyPu6j4xRJon89Ay0BEpRPw5mQMJQhCMrI2iiQC/i9yfhzXSUWW
# 6Fkd6fp0ZGuy62ZD2rOwjNXpDd32ASDOmTFjPQgaGLOBm0/GkxAG/AeB+ova+YJJ
# 92JuoVP6EpQYhS6SkepobEQysmah5xikmmRR7zCCBTEwggQZoAMCAQICEAqhJdbW
# Mht+QeQF2jaXwhUwDQYJKoZIhvcNAQELBQAwZTELMAkGA1UEBhMCVVMxFTATBgNV
# BAoTDERpZ2lDZXJ0IEluYzEZMBcGA1UECxMQd3d3LmRpZ2ljZXJ0LmNvbTEkMCIG
# A1UEAxMbRGlnaUNlcnQgQXNzdXJlZCBJRCBSb290IENBMB4XDTE2MDEwNzEyMDAw
# MFoXDTMxMDEwNzEyMDAwMFowcjELMAkGA1UEBhMCVVMxFTATBgNVBAoTDERpZ2lD
# ZXJ0IEluYzEZMBcGA1UECxMQd3d3LmRpZ2ljZXJ0LmNvbTExMC8GA1UEAxMoRGln
# aUNlcnQgU0hBMiBBc3N1cmVkIElEIFRpbWVzdGFtcGluZyBDQTCCASIwDQYJKoZI
# hvcNAQEBBQADggEPADCCAQoCggEBAL3QMu5LzY9/3am6gpnFOVQoV7YjSsQOB0Uz
# URB90Pl9TWh+57ag9I2ziOSXv2MhkJi/E7xX08PhfgjWahQAOPcuHjvuzKb2Mln+
# X2U/4Jvr40ZHBhpVfgsnfsCi9aDg3iI/Dv9+lfvzo7oiPhisEeTwmQNtO4V8CdPu
# XciaC1TjqAlxa+DPIhAPdc9xck4Krd9AOly3UeGheRTGTSQjMF287DxgaqwvB8z9
# 8OpH2YhQXv1mblZhJymJhFHmgudGUP2UKiyn5HU+upgPhH+fMRTWrdXyZMt7HgXQ
# hBlyF/EXBu89zdZN7wZC/aJTKk+FHcQdPK/P2qwQ9d2srOlW/5MCAwEAAaOCAc4w
# ggHKMB0GA1UdDgQWBBT0tuEgHf4prtLkYaWyoiWyyBc1bjAfBgNVHSMEGDAWgBRF
# 66Kv9JLLgjEtUYunpyGd823IDzASBgNVHRMBAf8ECDAGAQH/AgEAMA4GA1UdDwEB
# /wQEAwIBhjATBgNVHSUEDDAKBggrBgEFBQcDCDB5BggrBgEFBQcBAQRtMGswJAYI
# KwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmRpZ2ljZXJ0LmNvbTBDBggrBgEFBQcwAoY3
# aHR0cDovL2NhY2VydHMuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0QXNzdXJlZElEUm9v
# dENBLmNydDCBgQYDVR0fBHoweDA6oDigNoY0aHR0cDovL2NybDQuZGlnaWNlcnQu
# Y29tL0RpZ2lDZXJ0QXNzdXJlZElEUm9vdENBLmNybDA6oDigNoY0aHR0cDovL2Ny
# bDMuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0QXNzdXJlZElEUm9vdENBLmNybDBQBgNV
# HSAESTBHMDgGCmCGSAGG/WwAAgQwKjAoBggrBgEFBQcCARYcaHR0cHM6Ly93d3cu
# ZGlnaWNlcnQuY29tL0NQUzALBglghkgBhv1sBwEwDQYJKoZIhvcNAQELBQADggEB
# AHGVEulRh1Zpze/d2nyqY3qzeM8GN0CE70uEv8rPAwL9xafDDiBCLK938ysfDCFa
# KrcFNB1qrpn4J6JmvwmqYN92pDqTD/iy0dh8GWLoXoIlHsS6HHssIeLWWywUNUME
# aLLbdQLgcseY1jxk5R9IEBhfiThhTWJGJIdjjJFSLK8pieV4H9YLFKWA1xJHcLN1
# 1ZOFk362kmf7U2GJqPVrlsD0WGkNfMgBsbkodbeZY4UijGHKeZR+WfyMD+NvtQEm
# tmyl7odRIeRYYJu6DC0rbaLEfrvEJStHAgh8Sa4TtuF8QkIoxhhWz0E0tmZdtnR7
# 9VYzIi8iNrJLokqV2PWmjlIxggRcMIIEWAIBATCBhjByMQswCQYDVQQGEwJVUzEV
# MBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3d3cuZGlnaWNlcnQuY29t
# MTEwLwYDVQQDEyhEaWdpQ2VydCBTSEEyIEFzc3VyZWQgSUQgQ29kZSBTaWduaW5n
# IENBAhACY0K2XvmKQ+pAe/UHOF6cMAkGBSsOAwIaBQCgeDAYBgorBgEEAYI3AgEM
# MQowCKACgAChAoAAMBkGCSqGSIb3DQEJAzEMBgorBgEEAYI3AgEEMBwGCisGAQQB
# gjcCAQsxDjAMBgorBgEEAYI3AgEVMCMGCSqGSIb3DQEJBDEWBBTEqlNEiID1bE2P
# nIm63MDGHj19/DANBgkqhkiG9w0BAQEFAASCAQAYwQd8D7SLc3ITESa9siJENfTf
# 7bPFCXuy8Rn6wGJenGp3/hw7SHXbCeAFveU+sAXzyoNwiKfjmTbcg19ybmBMaqfP
# IRTHT2CJx0fCCUuulhfX/m3oRK83jJqOOVshHYqABJdIDv9KW750o5Dp2SaxNZMN
# 1Z3LTzVWP0f0ptJOTDSNtHWgV5skLUd0+Z+Z37uRLhcG3IrLe6BJ+MrAYO0AKpBc
# +ZLN/ZK4p6JQnqMIW6edmuI1F6zEiVH5g6RAdePJDH7TxtWRKdAcmF8LPGkOWonk
# D3RjV+6V+L3nKksLgthYa1QEgBhAQyMDvJwvoTGvdJwp2667+9Q46WfH7xHGoYIC
# MDCCAiwGCSqGSIb3DQEJBjGCAh0wggIZAgEBMIGGMHIxCzAJBgNVBAYTAlVTMRUw
# EwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20x
# MTAvBgNVBAMTKERpZ2lDZXJ0IFNIQTIgQXNzdXJlZCBJRCBUaW1lc3RhbXBpbmcg
# Q0ECEA1CSuC+Ooj/YEAhzhQA8N0wDQYJYIZIAWUDBAIBBQCgaTAYBgkqhkiG9w0B
# CQMxCwYJKoZIhvcNAQcBMBwGCSqGSIb3DQEJBTEPFw0yMTA1MDcxNTA4MzNaMC8G
# CSqGSIb3DQEJBDEiBCA1oOpadALkZIoyyJAkMqydvsGNAfVnObHP1UOQ+28rQzAN
# BgkqhkiG9w0BAQEFAASCAQBV1zc3MVmOpewN7EGfWQ0Le6S3SKMOqgtTggBYlAPM
# nCNmAnh+ZViyA5ukwbOHYp6z9YgZgcxHaYx7LaHijdMr4c1h3zBxRM6RKLSBLXvP
# XFHk1aapFvDPQlHclrPL/qH4U2kGTGoKkV4Cbjgh9G+BokfiZ143cHbgkIWmDC3f
# 4Z5pLGG0zO7Ca8xp9Fjh8PUVInYSd57JpwsX9WWGakTVXw4OdmEhPk1lBbLrBQ0b
# 7ZIfSisd48kPCmwRxK9708frnSQaFShsHkseUvniZgrSHZjvUXvxddbYTxJurnFE
# FUK9ONgC7A5xDbmsmgdJIBu5LlTd5GfTpRanqfCYzahv
# SIG # End signature block
