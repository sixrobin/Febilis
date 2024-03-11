namespace RSLib.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public static class SolutionSynchronizer
	{
		private static object s_synchronizerObject;
		private static MethodInfo s_syncSolutionMethodInfo;
		private static MethodInfo s_synchronizerSyncMethodInfo;
		
		static SolutionSynchronizer()
		{
			Type syncVSType = Type.GetType("UnityEditor.SyncVS,UnityEditor");
			FieldInfo synchronizerField = syncVSType.GetField("Synchronizer", BindingFlags.NonPublic | BindingFlags.Static);
			s_syncSolutionMethodInfo = syncVSType.GetMethod("SyncSolution", BindingFlags.Public | BindingFlags.Static);
			
			s_synchronizerObject = synchronizerField.GetValue(syncVSType);
			Type synchronizerType = s_synchronizerObject.GetType();
			s_synchronizerSyncMethodInfo = synchronizerType.GetMethod("Sync", BindingFlags.Public | BindingFlags.Instance);
		}

		[MenuItem("Assets/Sync C# Solution", priority = 1000000)]
		public static void Sync()
        {
			CleanOldFiles();
			CallSyncSolution();
			CallSynchronizerSync();
		}

		private static void CleanOldFiles()
		{
			DirectoryInfo assetsDirectoryInfo = new DirectoryInfo(Application.dataPath);
			DirectoryInfo projectDirectoryInfo = assetsDirectoryInfo.Parent;

			IEnumerable<FileInfo> files = GetFilesByExtensions(projectDirectoryInfo, "*.sln", "*.csproj");
			foreach (FileInfo file in files)
			{
				Debug.Log($"Removing old solution file: {file.Name}.");
				file.Delete();
			}
		}

		private static void CallSyncSolution()
		{
		    Debug.Log($"Call method: SyncVS.Sync()");
			s_syncSolutionMethodInfo.Invoke(null, null);
		}

		private static void CallSynchronizerSync()
		{
			Debug.Log($"Call method: SyncVS.Synchronizer.Sync()");
			s_synchronizerSyncMethodInfo.Invoke(s_synchronizerObject, null);
		}

		private static IEnumerable<FileInfo> GetFilesByExtensions(DirectoryInfo dir, params string[] extensions)
		{
			extensions = extensions ?? new[] {"*"};
			IEnumerable<FileInfo> files = Enumerable.Empty<FileInfo>();
			foreach (string ext in extensions)
				files = files.Concat(dir.GetFiles(ext));

			return files;
		}
	}
}