using System;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Build.Reporting;
#endif

public class BuildHelper:MonoBehaviour
{
	#if UNITY_EDITOR
	[MenuItem("Build/Client")]
	public static void BuildClient(){Build(false,false);}
	[MenuItem("Build/and Run Client")]
	public static void BuildAndRunClient(){Build(false,false,true);}
	[MenuItem("Build/and Debug Client")]
	public static void BuildAndDebugClient(){Build(false,true);}
	[MenuItem("Build/Server")]
	public static void BuildServer(){Build(true,false);}
	[MenuItem("Build/and Run Server")]
	public static void BuildAndRunServer(){Build(true,false,true);}
	[MenuItem("Build/and Debug Server")]
	public static void BuildAndDebugServer(){Build(true,true);}
	[MenuItem("Build/Release (Server and Client)")]
	public static void BuildRelease()
	{
		Debug.Log("Building Client...");
		Build(false,false,false,true);
		Debug.Log("Client finished. Building Server...");
		Build(true,false,false,true);
		Debug.Log("Release builds finished.");
	}

	public static void Build(bool server,bool debug,bool autorun=false,bool compress=false)
	{
		Debug.Log(String.Format("Building {0}; Autorun {1}, Debug {2}",server?"Server":"Client",autorun,debug));
		autorun|=debug;
		compress&=!debug;
		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		buildPlayerOptions.scenes = new[]{"Assets/TestScene.unity"};
		buildPlayerOptions.locationPathName = "Somnium"+(server?"Server":"Client");
		buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
		buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;
		buildPlayerOptions.options = BuildOptions.StrictMode;
		buildPlayerOptions.locationPathName = String.Format("build/{0}/{1}.x86_64",server?"server":"client",buildPlayerOptions.locationPathName);
		if(server){buildPlayerOptions.options |= BuildOptions.EnableHeadlessMode;}
		if(autorun){buildPlayerOptions.options |= BuildOptions.AutoRunPlayer;}
		if(debug)
		{
			buildPlayerOptions.options
				|= BuildOptions.Development
				|BuildOptions.ConnectWithProfiler
				|BuildOptions.AllowDebugging
				|BuildOptions.ConnectToHost;
		}
		if(compress){buildPlayerOptions.options |= BuildOptions.CompressWithLz4HC;}
		else{buildPlayerOptions.options |= BuildOptions.UncompressedAssetBundle;}

		BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
		BuildSummary summary = report.summary;

		if(summary.result==BuildResult.Succeeded)
		{
			Debug.Log(String.Format("Build succeeded with {0} warnings.",summary.totalWarnings));
		}
		else
		{
			Debug.Log(String.Format("Build failed with {0} errors and {1} warnings.",summary.totalErrors,summary.totalWarnings));
		}
	}
	#endif
}