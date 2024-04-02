using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Octokit;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
	/// Support plugins are available for:
	///   - JetBrains ReSharper        https://nuke.build/resharper
	///   - JetBrains Rider            https://nuke.build/rider
	///   - Microsoft VisualStudio     https://nuke.build/visualstudio
	///   - Microsoft VSCode           https://nuke.build/vscode
	public static int Main() => Execute<Build>(x => x.Compile);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;


	[Solution(GenerateProjects = true)] readonly Solution Solution;
	readonly AbsolutePath ArtifactsDirectory = RootDirectory / ".nuke" / "artifacts";

	[Parameter] [Secret] readonly string NugetApiKey;

	Target Clean => _ => _
		.Before(Restore)
		.Executes(() =>
		{
			DotNetTasks.DotNetClean();
			ArtifactsDirectory.CreateOrCleanDirectory();
		});

	Target Restore => _ => _
		.Executes(() =>
		{
			DotNetTasks.DotNetRestore();
		});

	Target Compile => _ => _
		.DependsOn(Restore)
		.Executes(() =>
		{
			DotNetTasks.DotNetBuild(_ => _
				.EnableNoRestore()
				.SetConfiguration(Configuration)
			);
		});

	Target Test => _ => _
		.DependsOn(Compile)
		.Executes(() =>
		{
			DotNetTasks.DotNetTest(_ => _
				.SetProjectFile(Solution.Net_FracturedCode_Utilities_Tests)
				.EnableNoRestore()
				.EnableNoBuild()
				.SetLoggers("trx")
				.SetResultsDirectory(ArtifactsDirectory)
			);
		});

	Target Pack => _ => _
		.DependsOn(Clean, Test)
		.Executes(() =>
		{
			DotNetTasks.DotNetPack(_ => _
				.SetProject(Solution.Net_FracturedCode_Utilities)
				.SetConfiguration(Configuration)
				.EnableNoRestore()
				.EnableNoBuild()
				.EnableIncludeSource()
				.EnableIncludeSymbols()
				.SetSymbolPackageFormat(DotNetSymbolPackageFormat.snupkg)
				.SetOutputDirectory(ArtifactsDirectory)
			);
		});

	Target Publish => _ => _
		.Requires(() => NugetApiKey)
		.DependsOn(Pack)
		.Executes(() =>
		{
			string sourceEndpoint = "https://" + (IsServerBuild && Configuration == Configuration.Release
				? "api.nuget.org"
				: "apiint.nugettest.org"
			) + "/v3/index.json";
			// Publishing the snupkg is implicit, so we're checking that it exists.
			if (!ArtifactsDirectory.GlobFiles("*.snupkg").Any())
			{
				throw new Exception($"This should not be possible because the {nameof(Pack)} target should always create a snupkg");
			}
			DotNetTasks.DotNetNuGetPush(_ => _
				.SetApiKey(NugetApiKey)
				.SetSource(sourceEndpoint)
				.SetTargetPath(ArtifactsDirectory.GlobFiles("*.nupkg").Single())
			);
		});
}