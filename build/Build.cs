using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;

[GitHubActions("publish",
	GitHubActionsImage.UbuntuLatest,
	On = [GitHubActionsTrigger.WorkflowDispatch],
	InvokedTargets = [nameof(Pack)],
	ImportSecrets = [nameof(NugetApiKey)]
)]
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
		.Produces(ArtifactsDirectory / "*.trx")
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
		.DependsOn(Compile)
		.Produces(ArtifactsDirectory / "*.nupkg", ArtifactsDirectory / "*.snupkg")
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
		.DependsOn(Clean, Test, Pack)
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