
## Debugging

Press the spacebar while running to view a debug output. For better formatting, it's recommended to view console output using Powershell.

To view Unity console logs using Powershell (handy because of the monospaced font), open the directory `%UserProfile%\AppData\Local\Unity\Editor` in Powershell, then run the command `Get-Content .\Editor.log -Tail 0 -Wait` to print the debug log's changes as they happen. See [this page](https://blogs.technet.microsoft.com/rmilne/2016/06/03/powershell-tail-command/) for more detail.
