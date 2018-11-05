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
$coverageFile = '..\..\artifacts\test-coverage.xml';

# Build
Write-Host "Building"

& dotnet build $src -c Release --version-suffix=$buildSuffix;

# Package
if ($branch -eq "master"){
	
	Write-Host "Creating nuget packages"

	if ($suffix){
		& dotnet pack $src -c Release --no-build -o ..\..\artifacts --version-suffix=$suffix
	}
	else {
		& dotnet pack $src -c Release --no-build -o ..\..\artifacts
	}
}

# Test & collect code coverage
Write-Host "Running unit tests and collecting code coverage"

& dotnet test $test -c Debug /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$coverageFile

if ($LASTEXITCODE -ne 0) { exit 3 }

# Publish coverage file
if ($env:APPVEYOR_JOB_ID) {

	Write-Host "Publishing code coverage results"

	& dotnet tool install coveralls.net --version 1.0.0 --tool-path tools

	$coveralls = ".\tools\csmacnz.coveralls.exe"

	& $coveralls --opencover -i .\artifacts\test-coverage.xml `
		--repoToken $env:COVERALLS_API_TOKEN `
		--commitId $env:APPVEYOR_REPO_COMMIT `
		--commitBranch $env:APPVEYOR_REPO_BRANCH `
		--commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR `
		--commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE `
		--jobId $env:APPVEYOR_JOB_ID

}