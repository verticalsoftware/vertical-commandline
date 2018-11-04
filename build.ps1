Write-Host "Build started"

Push-Location $PSScriptRoot

if (Test-Path .\artifacts) {
    Write-Host "Cleaning up artifacts"
    Remove-Item .\artifacts -Force -Recurse
}

& dotnet restore --no-cache

$branch = @{ $true = $env:APPVEYOR_REPO_BRANCH; $false = $(git symbolic-ref --short -q HEAD) }[$env:APPVEYOR_REPO_BRANCH -ne $NULL];
$revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $env:APPVEYOR_BUILD_NUMBER, 10); $false = "local" }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
$suffix = @{ $true = ""; $false = "$($branch.Substring(0, [math]::Min(10, $branch.Length)))-$revision"}[$branch -eq "master" -and $revision -ne "local"];
$commitHash = $(git rev-parse --short HEAD)
$buildSuffix = @{ $true = "$($suffix)-$($commitHash)"; $false = "$($branch)-$($commitHash)" }[$suffix -ne ""];

Write-Host "Branch:   $branch";
Write-Host "Revision: $revision";
Write-Host "Suffix:   $suffix";
Write-Host "Commit:   $commitHash";
Write-Host "Build:    $buildSuffix";

$src = ".\src\Vertical.CommandLine\Vertical.CommandLine.csproj";
$test = ".\test\Vertical.CommandLine.Tests\Vertical.CommandLine.Tests.csproj";

# Build
& dotnet build $src -c Release --version-suffix=$buildSuffix;

# Package
if ($suffix){
    & dotnet pack $src -c Release --no-build -o ..\..\artifacts --version-suffix=$suffix
}
else {
    & dotnet pack $src -c Release --no-build -o ..\..\artifacts
}

# Test
& dotnet test $test -c Release

if ($LASTEXITCODE -ne 0) { exit 3 }

