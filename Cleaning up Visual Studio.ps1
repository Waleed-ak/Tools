Clear-Host 

# Define files and directories to delete

#####################################################
#####################################################
###                                               ###
###  ===========    Files    ===========          ###
###  *.rdl.data                                   ###
###  *.dbmdl                                      ###
###  *.suo                                        ###
###  *.user                                       ###
###  *.cache                                      ###
###  *.docstates                                  ###
###  *.refactorlog                                ###
###  ===========    Folders  ===========          ###
###  bin                                          ###
###  obj                                          ###
###  build                                        ###
###                                               ###
#####################################################
#####################################################

$Include = @("*.rdl.data","*.dbmdl","*.suo","*.user","*.cache","*.docstates","*.refactorlog","bin","obj","build")

# Define files and directories to exclude
$Exclude = @()

$Path =".."

$Items = Get-ChildItem -Path $Path -recurse -force -include $Include -exclude $Exclude

if ($items) {
    foreach ($Item in $Items) {
        Remove-Item $Item.FullName -Force -Recurse -ErrorAction SilentlyContinue
        Write-Host "Deleted" $Item.FullName
    }
}
Pause "Press any key to continue . . ."

