// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------

(*
    This file handles the complete build process of Qowaiv

    The first step is handled in build.sh and build.cmd by bootstrapping a NuGet.exe and 
    executing NuGet to resolve all build dependencies (dependencies required for the build to work, for example FAKE)

    The second step is executing this file which resolves all dependencies, builds the solution and executes all unit tests
*)


// Supended until FAKE supports custom mono parameters
#I @".nuget/Build/FAKE/tools/" // FAKE
#r @"FakeLib.dll"  //FAKE

open System.Collections.Generic
open System.IO

open Fake
open Fake.Git
open Fake.FSharpFormatting
open AssemblyInfoFile

// properties
let projectName = "Qowaiv"
let projectSummary = "Single Value Domain Objects"
let projectDescription = "Qowaiv implements common, universal domain objects. These types form the base of your domain model."
let authors = ["Corniel Nobel"; "Jack Kester"; "Wilko Frieke";]
let mail = "qowaiv@corniel.nl"
let version = "2.0.2.186"
let commitHash = Information.getCurrentSHA1(".")

//let buildTargets = environVarOrDefault "BUILDTARGETS" ""
//let buildPlatform = environVarOrDefault "BUILDPLATFORM" "MONO"
let buildDir = "./build/"
let releaseDir = "./release/"
let outLibDir = "./release/lib/"
let outDocDir = "./release/documentation/"
let docTemplatesDir = "./doc/templates/"
let testDir  = "./test/"
let nugetDir  = "./nuget/"
let packageDir  = "./src/packages"

let github_user = "Corniel"
let github_project = "Qowaiv"

let tags = "Domain modeling postal code country IBAN BIC gender email address percentage"

let buildMode = if isMono then "Release" else "Debug"

// Where to look for *.cshtml templates (in this order)
let layoutRoots =
    [ docTemplatesDir; 
      docTemplatesDir @@ "reference" ]

if isMono then
    monoArguments <- "--runtime=v4.0 --debug"
    //monoArguments <- "--runtime=v4.0"

let github_url = sprintf "https://github.com/%s/%s" github_user github_project
    
// Ensure the ./src/.nuget/NuGet.exe file exists (required by xbuild)
let nuget = findToolInSubPath "NuGet.exe" "./.nuget/Build/NuGet.CommandLine/tools/NuGet.exe"
System.IO.Directory.CreateDirectory("./src/.nuget")
System.IO.File.Copy(nuget, "./src/.nuget/NuGet.exe", true)

// Read release notes document
let release = ReleaseNotesHelper.parseReleaseNotes (File.ReadLines "ReleaseNotes.md")

let MyTarget name body =
    Target name body
    Target (sprintf "%s_single" name) body 


type BuildParams =
    {
        OutDirName : string
        TargetName : string
        DefineConstants : string
    }

let buildApp (buildParams:BuildParams) =
    let buildDir = buildDir @@ buildParams.OutDirName
    CleanDirs [ buildDir ]
    // build app
    let files = !! "src/**/*.csproj" -- "src/**/*Tests.csproj"
    files |> Log "Build: "
    files
        |> MSBuild buildDir "Build" 
            [   "Configuration", buildMode
                "TargetFrameworkVersion", buildParams.TargetName 
                "DefineConstants", buildParams.DefineConstants ]
        |> Log "AppBuild-Output: "

let buildTests (buildParams:BuildParams) =
    let testDir = testDir @@ buildParams.OutDirName
    CleanDirs [ testDir ]
    // build tests
    let files = !! "src/**/*Tests.csproj"
    files
        |> MSBuild testDir "Build" 
            [   "Configuration", buildMode
                "TargetFrameworkVersion", buildParams.TargetName 
                "DefineConstants", buildParams.DefineConstants ]
        |> Log "TestBuild-Output: "
    
let runTests  (buildParams:BuildParams) =
    let testDir = testDir @@ buildParams.OutDirName
    let logs = System.IO.Path.Combine(testDir, "logs")
    System.IO.Directory.CreateDirectory(logs) |> ignore
    !! (testDir + "/*Tests.dll") 
        |> NUnit (fun p ->
            {p with
                ToolName = "nunit-console-x86.exe"
                ExcludeCategory = if isMono then "VBNET" else ""
                ProcessModel =
                    // Because the default nunit-console.exe.config doesn't use .net 4...
                    if isMono then NUnitProcessModel.SingleProcessModel else NUnitProcessModel.DefaultProcessModel
                WorkingDir = testDir
                StopOnError = false
                TimeOut = System.TimeSpan.FromMinutes 30.0
                Framework = "4.5"
                DisableShadowCopy = true;
                OutputFile = "logs/TestResults.xml" })

    
let net40Params = { 
    OutDirName = "net40"; 
    TargetName = "v4.0"; 
    DefineConstants = if isMono then "NET40 MONO" else "NET40" }
let net45Params = { 
    OutDirName = "net45"; 
    TargetName = "v4.5";
    DefineConstants = if isMono then "NET45 MONO" else "NET45" }

// Documentation 
let buildDocumentationTarget target =
    trace (sprintf "Building documentation (%s), this could take some time, please wait..." target)
    let b, s = executeFSI "." "generateDocs.fsx" ["target", target]
    for l in s do
        (if l.IsError then traceError else trace) (sprintf "DOCS: %s" l.Message)
    if not b then
        failwith "documentation failed"
    ()
