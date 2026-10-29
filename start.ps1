<#
.SYNOPSIS
  Starts the Java gRPC server, the WebApi and the Blazor client in one Windows Terminal window (3 tabs).

.EXAMPLE
  .\start.ps1
#>
param(
    [ValidateSet('all', 'java', 'webapi', 'client')]
    [string]$Service = 'all'
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot

function Import-DotEnv([string]$path) {
    if (-not (Test-Path $path)) {
        throw "Missing $path. Copy .env.example to .env and fill in the database settings."
    }
    foreach ($line in Get-Content $path) {
        if ($line -match '^\s*([^#=\s][^=]*?)\s*=\s*(.*)\s*$') {
            Set-Item -Path "Env:$($Matches[1])" -Value $Matches[2].Trim('"', "'")
        }
    }
}

function Start-Java {
    Import-DotEnv (Join-Path $root '.env')

    if (-not $env:JAVA_HOME) { throw 'JAVA_HOME is not set. Point it at your JDK 21 install.' }
    $env:JAVA_HOME = $env:JAVA_HOME.Trim('"', "'")
    if (-not (Get-Command mvn -ErrorAction SilentlyContinue)) { throw 'mvn was not found on PATH. Install Maven and add its bin folder to PATH.' }

    $javaExe = Join-Path $env:JAVA_HOME 'bin\java.exe'
    if (-not (Test-Path $javaExe)) { throw "No java.exe found under JAVA_HOME ($env:JAVA_HOME)." }
    # Read the version from the file metadata: `java -version` writes to stderr, which PowerShell 5.1 turns into an error.
    $javaMajor = (Get-Item $javaExe).VersionInfo.ProductMajorPart
    if ($javaMajor -lt 21) { throw "JAVA_HOME points at Java $javaMajor; the server needs 21 or newer." }

    Set-Location (Join-Path $root 'Java\TagalongJavaServer')
    mvn compile org.codehaus.mojo:exec-maven-plugin:3.5.0:java '-Dexec.mainClass=tagalong.server.TagalongServer'
}

function Start-WebApi {
    dotnet run --project (Join-Path $root 'C#\WebApi\WebApi.csproj') --launch-profile https
}

function Start-Client {
    dotnet run --project (Join-Path $root 'C#\Tagalong\Tagalong\Tagalong.csproj') --launch-profile https
}

if ($Service -eq 'all') {
    if (-not (Get-Command wt -ErrorAction SilentlyContinue)) { throw 'Windows Terminal (wt) was not found. Install it from the Microsoft Store.' }
    if (-not (Test-Path (Join-Path $root '.env'))) { throw "Missing $(Join-Path $root '.env'). Copy .env.example to .env and fill in the database settings." }

    $self = $MyInvocation.MyCommand.Path
    function Tab([string]$title, [string]$svc) {
        "new-tab --title `"$title`" -d `"$root`" powershell -NoExit -ExecutionPolicy Bypass -File `"$self`" -Service $svc"
    }

    # -w new forces a fresh window instead of attaching tabs to an existing one.
    $wtArgs = "-w new " + ((Tab 'Java server' 'java'), (Tab 'WebApi' 'webapi'), (Tab 'Blazor client' 'client') -join ' ; ')
    Start-Process wt -ArgumentList $wtArgs
    return
}

switch ($Service) {
    'java'   { Start-Java }
    'webapi' { Start-WebApi }
    'client' { Start-Client }
}
