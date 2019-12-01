# Implement solutions that us virtual machines(VMs)

## Provision VMs


The azure cloud shell is a free interactive shell provided within the Azure Portal.

1. Create a resource group using the Azure PowerShell New-AzResourceGroup command

``` powershell
New-AzResourceGroup `
   -ResourceGroupName "myResourceGroupVM" `
   -Location "EastUS"
   ```
   A resource group is a logical container into which Azure resources are deployed and managed.

2. Set the username and password for the admin account on the VM with the Get-Credential command. 

```powershell

$cred = Get-Credential

```
