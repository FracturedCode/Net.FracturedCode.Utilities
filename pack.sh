dotnet pack -c Release --include-source --include-symbols Net.FracturedCode.Utilities/Net.FracturedCode.Utilities.csproj
echo Open Nuget Package Explorer
echo https://github.com/dotnet/sourcelink/issues/572 I used to remove the obj folder
echo Inspect to make sure everything looks good, and save
read -n 1 -p "read the above lines. waiting for input..."
read -p "Enter API Key (in password manager): " API_KEY
read -p "Enter the version so I don't have to read the csproj: " $PACKAGE_VERSION
dotnet nuget push "Net.FracturedCode.Utilities/Net.FracturedCode.Utilities/bin/Release/Net.FracturedCode.Utilities.${PACKAGE_VERSION}.symbols.nupkg" -k $API_KEY