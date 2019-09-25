# JustAddWater Logic Assembly

In order to improve testability, the logic of the game has been split out into a separate, external .Net Standard assembly called JustAddWater.Logic in the JustAddWaterTestableLogic folder.

## Building

Using the [.Net Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x), run the following command from the root of the JustAddWaterTestableLogic/Logic project folder:

`dotnet watch build -c Release -p:DebugType=None`

Every time the code in the Logic project is changed, the Logic assembly will be rebuilt and copied into the appropriate folder at JustAddWater/Assets/Packages/

## Testing

Using the [.Net Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x), run the following command from the root of the JustAddWaterTestableLogic/LogicTests project folder:

`dotnet watch test`

Every time the code in in the Logic or LogicTests projects are changed, the full suite of unit tests will be run and the results will be outputted to the command line window.
