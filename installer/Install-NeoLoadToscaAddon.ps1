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
		"HKLM:\SOFTWARE\*"
		"HKLM:\Software\Wow6432Node\*"
    )
    $Results = @((Get-ItemProperty $RegistryPaths) -Match $TOSCA_SOFTWARE_NAME  | Where-Object { $_.Home_Long })
    if ($Results.Length -gt 1) {
        $Title = ("We have found several installations of $TOSCA_SOFTWARE_NAME installed on your machine.")
        $Prompt = "Choose one installation path"
        $Counter = 0
        $Choices = $Results | ForEach-Object { New-Object System.Management.Automation.Host.ChoiceDescription ("&$Counter - " + $_.Home_Long) ; $Counter++ }
        $Default = 0
        $Choice = $Host.UI.PromptForChoice($Title, $Prompt, $Choices, $Default)
        Invoke-StepThree($Results[$Choice].Home_Long)
    }
    elseif ($Results.Length -eq 1) {
        Invoke-StepThree($Results[0].Home_Long)
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
# MIISFQYJKoZIhvcNAQcCoIISBjCCEgICAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUxWhApVqMIH87WObscJY2SwEE
# G1Sggg5hMIIGsDCCBJigAwIBAgIQCK1AsmDSnEyfXs2pvZOu2TANBgkqhkiG9w0B
# AQwFADBiMQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYD
# VQQLExB3d3cuZGlnaWNlcnQuY29tMSEwHwYDVQQDExhEaWdpQ2VydCBUcnVzdGVk
# IFJvb3QgRzQwHhcNMjEwNDI5MDAwMDAwWhcNMzYwNDI4MjM1OTU5WjBpMQswCQYD
# VQQGEwJVUzEXMBUGA1UEChMORGlnaUNlcnQsIEluYy4xQTA/BgNVBAMTOERpZ2lD
# ZXJ0IFRydXN0ZWQgRzQgQ29kZSBTaWduaW5nIFJTQTQwOTYgU0hBMzg0IDIwMjEg
# Q0ExMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEA1bQvQtAorXi3XdU5
# WRuxiEL1M4zrPYGXcMW7xIUmMJ+kjmjYXPXrNCQH4UtP03hD9BfXHtr50tVnGlJP
# DqFX/IiZwZHMgQM+TXAkZLON4gh9NH1MgFcSa0OamfLFOx/y78tHWhOmTLMBICXz
# ENOLsvsI8IrgnQnAZaf6mIBJNYc9URnokCF4RS6hnyzhGMIazMXuk0lwQjKP+8bq
# HPNlaJGiTUyCEUhSaN4QvRRXXegYE2XFf7JPhSxIpFaENdb5LpyqABXRN/4aBpTC
# fMjqGzLmysL0p6MDDnSlrzm2q2AS4+jWufcx4dyt5Big2MEjR0ezoQ9uo6ttmAaD
# G7dqZy3SvUQakhCBj7A7CdfHmzJawv9qYFSLScGT7eG0XOBv6yb5jNWy+TgQ5urO
# kfW+0/tvk2E0XLyTRSiDNipmKF+wc86LJiUGsoPUXPYVGUztYuBeM/Lo6OwKp7AD
# K5GyNnm+960IHnWmZcy740hQ83eRGv7bUKJGyGFYmPV8AhY8gyitOYbs1LcNU9D4
# R+Z1MI3sMJN2FKZbS110YU0/EpF23r9Yy3IQKUHw1cVtJnZoEUETWJrcJisB9IlN
# Wdt4z4FKPkBHX8mBUHOFECMhWWCKZFTBzCEa6DgZfGYczXg4RTCZT/9jT0y7qg0I
# U0F8WD1Hs/q27IwyCQLMbDwMVhECAwEAAaOCAVkwggFVMBIGA1UdEwEB/wQIMAYB
# Af8CAQAwHQYDVR0OBBYEFGg34Ou2O/hfEYb7/mF7CIhl9E5CMB8GA1UdIwQYMBaA
# FOzX44LScV1kTN8uZz/nupiuHA9PMA4GA1UdDwEB/wQEAwIBhjATBgNVHSUEDDAK
# BggrBgEFBQcDAzB3BggrBgEFBQcBAQRrMGkwJAYIKwYBBQUHMAGGGGh0dHA6Ly9v
# Y3NwLmRpZ2ljZXJ0LmNvbTBBBggrBgEFBQcwAoY1aHR0cDovL2NhY2VydHMuZGln
# aWNlcnQuY29tL0RpZ2lDZXJ0VHJ1c3RlZFJvb3RHNC5jcnQwQwYDVR0fBDwwOjA4
# oDagNIYyaHR0cDovL2NybDMuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0VHJ1c3RlZFJv
# b3RHNC5jcmwwHAYDVR0gBBUwEzAHBgVngQwBAzAIBgZngQwBBAEwDQYJKoZIhvcN
# AQEMBQADggIBADojRD2NCHbuj7w6mdNW4AIapfhINPMstuZ0ZveUcrEAyq9sMCcT
# Ep6QRJ9L/Z6jfCbVN7w6XUhtldU/SfQnuxaBRVD9nL22heB2fjdxyyL3WqqQz/WT
# auPrINHVUHmImoqKwba9oUgYftzYgBoRGRjNYZmBVvbJ43bnxOQbX0P4PpT/djk9
# ntSZz0rdKOtfJqGVWEjVGv7XJz/9kNF2ht0csGBc8w2o7uCJob054ThO2m67Np37
# 5SFTWsPK6Wrxoj7bQ7gzyE84FJKZ9d3OVG3ZXQIUH0AzfAPilbLCIXVzUstG2MQ0
# HKKlS43Nb3Y3LIU/Gs4m6Ri+kAewQ3+ViCCCcPDMyu/9KTVcH4k4Vfc3iosJocsL
# 6TEa/y4ZXDlx4b6cpwoG1iZnt5LmTl/eeqxJzy6kdJKt2zyknIYf48FWGysj/4+1
# 6oh7cGvmoLr9Oj9FpsToFpFSi0HASIRLlk2rREDjjfAVKM7t8RhWByovEMQMCGQ8
# M4+uKIw8y4+ICw2/O/TOHnuO77Xry7fwdxPm5yg/rBKupS8ibEH5glwVZsxsDsrF
# hsP2JjMMB0ug0wcCampAMEhLNKhRILutG4UI4lkNbcoFUCvqShyepf2gpx8GdOfy
# 1lKQ/a+FSCH5Vzu0nAPthkX0tGFuv2jiJmCG6sivqf6UHedjGzqGVnhOMIIHqTCC
# BZGgAwIBAgIQCzbMhpu0BiPPPytOmXEiUTANBgkqhkiG9w0BAQsFADBpMQswCQYD
# VQQGEwJVUzEXMBUGA1UEChMORGlnaUNlcnQsIEluYy4xQTA/BgNVBAMTOERpZ2lD
# ZXJ0IFRydXN0ZWQgRzQgQ29kZSBTaWduaW5nIFJTQTQwOTYgU0hBMzg0IDIwMjEg
# Q0ExMB4XDTIzMDIxNjAwMDAwMFoXDTI0MDMxMzIzNTk1OVowgY8xCzAJBgNVBAYT
# AkFUMQ0wCwYDVQQIEwRXaWVuMQ0wCwYDVQQHEwRXaWVuMRcwFQYDVQQKEw5Ucmlj
# ZW50aXMgR21iSDELMAkGA1UECxMCSVQxFzAVBgNVBAMTDlRyaWNlbnRpcyBHbWJI
# MSMwIQYJKoZIhvcNAQkBFhRvZmZpY2VAdHJpY2VudGlzLmNvbTCCAiIwDQYJKoZI
# hvcNAQEBBQADggIPADCCAgoCggIBALhhcynzRibNDNicsHfIsGkQVETqZi+mRQZc
# MPoheDe6kLcWmusgkSLLETv/zzTYxbe8Gk5I3jabX9K34bs3Zq+rg5PeV6r/qOpK
# E1PMw1MNvXsJE/nBnz+4M06CwuMlcc9meXd99I8MYlclKReZS4qoMpgOV2Rilp2k
# 1R1y5fTL/q7+y5msIowbm1tj6EMhDvPa4S0dCdda/u8kHhxa4+RQ07xt/x0Q1o6S
# 7cgqbOEquHfF1u4z/YoCCK8zyNfQZ4NnbmOy/TwftDb3SfdHJatSx3SSbcD8pDq5
# Nu5pIyPY/tAzEklxiodjd65xfMPQWYJFmdOgczj04bbkvz5zNOhjARbbabBMdjP8
# oN14q11qLgvxwex0oXovsDx+NiSJd8zba5NC+pB2N+atu4jbfm/K7qmudA6/XoS3
# U1+Jya+id7RJCddyrBxwLtMjPgRHeUagOR+ruvhSRV0JY0mO1oIAFGqudKU8g15I
# Ls+encbj0h7Esw76uNz6a9C+C2azrEwAVJVbzSD4JnvVCbap22tgT7flRD7za/aC
# +rMZcCQvnuta6WLAhSYKaEtAgXYhlLpq6smisUles+4Q3jS4Jx4Qk5cnsgXoGgf0
# tU7xuJ/tWOv+FNBGEY5Pl4aTKomv3mYxyw/IrpWETpo9u9PMXGzGQ5SdLfdZMVe+
# W55zyfuBAgMBAAGjggIkMIICIDAfBgNVHSMEGDAWgBRoN+Drtjv4XxGG+/5hewiI
# ZfROQjAdBgNVHQ4EFgQUWBcQWvijYSKjeQof3N58u3tYQoMwHwYDVR0RBBgwFoEU
# b2ZmaWNlQHRyaWNlbnRpcy5jb20wDgYDVR0PAQH/BAQDAgeAMBMGA1UdJQQMMAoG
# CCsGAQUFBwMDMIG1BgNVHR8Ega0wgaowU6BRoE+GTWh0dHA6Ly9jcmwzLmRpZ2lj
# ZXJ0LmNvbS9EaWdpQ2VydFRydXN0ZWRHNENvZGVTaWduaW5nUlNBNDA5NlNIQTM4
# NDIwMjFDQTEuY3JsMFOgUaBPhk1odHRwOi8vY3JsNC5kaWdpY2VydC5jb20vRGln
# aUNlcnRUcnVzdGVkRzRDb2RlU2lnbmluZ1JTQTQwOTZTSEEzODQyMDIxQ0ExLmNy
# bDA+BgNVHSAENzA1MDMGBmeBDAEEATApMCcGCCsGAQUFBwIBFhtodHRwOi8vd3d3
# LmRpZ2ljZXJ0LmNvbS9DUFMwgZQGCCsGAQUFBwEBBIGHMIGEMCQGCCsGAQUFBzAB
# hhhodHRwOi8vb2NzcC5kaWdpY2VydC5jb20wXAYIKwYBBQUHMAKGUGh0dHA6Ly9j
# YWNlcnRzLmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydFRydXN0ZWRHNENvZGVTaWduaW5n
# UlNBNDA5NlNIQTM4NDIwMjFDQTEuY3J0MAkGA1UdEwQCMAAwDQYJKoZIhvcNAQEL
# BQADggIBADdNMd83U51bmHgnWGYsVApIRopsBjodycvDlDQvV3ksUnpE6q9Du3AQ
# Y/rV8I+yUTcUU9jHTOgVl4c6ylh62J0blF1RsLi5ttpJJQDf0C+orf0GTYTURtlK
# flXMFAvcZ9XyFh79kW1wfXe69pLnPF+c3oyuDUZIdg3P4bSpE4Gkkbxjlcyf/23G
# OB7hJh72yHvaanVs30oQnPAfmhDxLz6LPgNekxJDYDRtwttWHuHvL+oIZ9WjEMm1
# qikCiKnGYO/hLchf0et9/96yq6fQ/QA46Irb5MI/Ey4SiJahXanoRZlCoU9wn0eH
# Y9sVP4/kE+Cgy2/01r43Nl6cRi6IgbV5EsSWBbL3tEKJhCHvRVgVc+aaq0AN5pVe
# 5XbLUXJRm+E8a5XczQOskFpNkNz8acnoUaaQ8S2wc3KwC0FFiu1zpHxGDKvkFnj1
# tzPIbi0r909MWHEnTx2GhckKj7pDqa0J5DTQUy0vZJXSFVSxqM9Fjp/Ponbh7Noq
# mJBsZB8yqZP+jBR5h5pt+AviX+V+raLP8ZXURwn6fr7euBXHhxJ+xdVemmqy2I0n
# TimkIedsIC01zp4/7YDgcLNnbOGRq7HM/z/X/0365HgNKmQuPhByDRKqgxOgVTl2
# jwAkPr6UBlWAKu24yCj4Irn/M4L09E8B5wKCq5QObPThDc/zolGWMYIDHjCCAxoC
# AQEwfTBpMQswCQYDVQQGEwJVUzEXMBUGA1UEChMORGlnaUNlcnQsIEluYy4xQTA/
# BgNVBAMTOERpZ2lDZXJ0IFRydXN0ZWQgRzQgQ29kZSBTaWduaW5nIFJTQTQwOTYg
# U0hBMzg0IDIwMjEgQ0ExAhALNsyGm7QGI88/K06ZcSJRMAkGBSsOAwIaBQCgeDAY
# BgorBgEEAYI3AgEMMQowCKACgAChAoAAMBkGCSqGSIb3DQEJAzEMBgorBgEEAYI3
# AgEEMBwGCisGAQQBgjcCAQsxDjAMBgorBgEEAYI3AgEVMCMGCSqGSIb3DQEJBDEW
# BBSRaQ8bEcA7ruSA1Mig0LyoCuK+njANBgkqhkiG9w0BAQEFAASCAgBXEFNDMKkQ
# vwygCsYH3SAbFR831qrKZ8c1bS58aWq8tzDRfQN53J9hC2Esl30qQFj2mp8bmtPH
# vGCSM6yo9RNQF8/hI52SLWQ+FAFM1BSaufilsYtZUxQqkKmgdcSGEwlMOMcU3gKg
# +ohg8Iu6T3eKZBvkt80/kivyar0qJFh/zb3aIHPgdF89Bpaay/W+TzrzlQryMg1s
# sg12BfI25IyRJpW4N1VDZrQzlRJpvyn4fGs4Tt9R2CzNP9v6CSpTIF8eBwWdrQKG
# At7BZ5Rip2UZVFmW3uFFwThQJnpV5SOfSj10ZjZW55dM7IudrTfr/LtGN0aUpB8L
# Pyc1pILZTiYhc9ahdt3IaBnEr434WzqZcB/VOTZd+dpyoaDk/YmjD8fvW34r8Ml5
# kjAYE96OvX7o8QBg3gMk7RYdM077FHnJg117O27ekn/xAq5Tm1fxf9UjqHXQ4yP1
# ynFotrxn/dosEjFbasT7zmwUNJ7R2DTz+GQjonXqqjUisUmNK2ZtZAp98T4EgXxs
# AZ3ZIivKbA8mBTDZlIsbr0ohfzajYeFrbfPCJ9GAcASPyMn05HwdYTdoCwgQjxwW
# hSP7mgg7VCLxTzc08CMSjVNP0r8kVl7n/GV/k+erDSXuVRjmn6aERmaOOgqwiXn3
# k6UfCYQdnTTQLap8uqrt3EPvv/+3yXi+eQ==
# SIG # End signature block
