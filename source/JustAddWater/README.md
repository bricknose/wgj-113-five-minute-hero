
## Debugging

### View Cauldron Brew

Press the **spacebar** while running to view a debug output. For better formatting, it's recommended to view console output using Powershell.

To view Unity console logs using Powershell (handy because of the monospaced font), open the directory `%UserProfile%\AppData\Local\Unity\Editor` in Powershell, then run the command `Get-Content .\Editor.log -Tail 0 -Wait` to print the debug log's changes as they happen. See [this page](https://blogs.technet.microsoft.com/rmilne/2016/06/03/powershell-tail-command/) for more detail.

### Stirring the Brew

Enter the desired stir row and column by pressing the **1-4 keys** on the keyboard. Stirring always occurs in a 2x2 grid with the entered row and column at the top-left of the selection. Note that the entered numbers are 1-indexed, so 1,1 corresponds to index 0,0 in the brew.

Once the desired indexes have been entered, press the **E key** to stir clockwise or the **Q key** to stir counter-clockwise.

For example, to stir a region from row 1 to row 2 and column 3 to column 4 clockwise, enter **1** and **3**, then press **E**.

Be sure to then press **spacebar** to view the cauldron brew's output and see how things have changed.
