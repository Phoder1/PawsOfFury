using System.Collections.Generic;
using System.IO;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using JetBrains.Annotations;

namespace Sisus.HierarchyFolders
{
	[InitializeOnLoad]
	public class HierarchyFolderBuildPrefabRemover : IPostprocessBuildWithReport
	{
		private static List<KeyValuePair<string, string>> paths = null;
		private static bool warnedAboutRemoveFromBuildDisabled;

		private static bool PrefabsStripped
		{
			get
			{
				return EditorPrefs.GetBool("HF.PrefabsStripped", false);
			}

			set
			{
				if(value)
				{
					EditorPrefs.SetBool("HF.PrefabsStripped", true);
				}
				else
				{
					EditorPrefs.DeleteKey("HF.PrefabsStripped");
				}
			}
		}

		public int callbackOrder
		{
			get
			{
				return 1000;
			}
		}

		static HierarchyFolderBuildPrefabRemover()
		{
			if(!PrefabsStripped || BuildPipeline.isBuildingPlayer)
			{
				return;
			}

			RestoreBackups();
		}

		[PostProcessScene(0), UsedImplicitly]
		private static void OnPostProcessScene()
		{
			// This will also get called when entering Playmode, when SceneManager.LoadScene is called,
			// but we only want to do stripping just after building the Scene.
			if(Application.isPlaying)
			{
				return;
			}

			if(PrefabsStripped)
			{
				return;
			}

			var preferences = HierarchyFolderPreferences.Get();
			if(preferences == null)
			{
				Debug.LogWarning("Failed to find Hierarchy Folder Preferences asset; will not strip hierarchy folders from build.");
				return;
			}

			StrippingType strippingType = StrippingType.FlattenHierarchyAndRemoveGameObject;

			switch(preferences.foldersInPrefabs)
			{
				case HierachyFoldersInPrefabs.NotAllowed:
					// No need to do any build stripping if no hierarchy folders are allowed to exist in prefabs.
					return;
				case HierachyFoldersInPrefabs.StrippedAtRuntime:
					// No need to do any build stripping if stripping occurs at runtime only.
					return;
				case HierachyFoldersInPrefabs.NotStripped:
					if(!preferences.warnWhenNotRemovedFromBuild || warnedAboutRemoveFromBuildDisabled)
					{
						return;
					}

					warnedAboutRemoveFromBuildDisabled = true;
					if(EditorUtility.DisplayDialog("Warning: Hierarchy Folder Prefab Stripping Disabled", "This is a reminder that you have disabled stripping of hierarchy folders from prefabs from builds. If you have any hierarchy folders inside prefabs this will result in suboptimal performance and is not recommended when making a release build.", "Continue Anyway", "Enable Stripping"))
					{
						return;
					}

					// If not stripped is true we need to disable all HierarchyFolders in prefabs to ensure that runtime stripping does no take place.
					strippingType = StrippingType.DisableComponent;
					break;
				case HierachyFoldersInPrefabs.StrippedAtBuildTime:
					break;
				default:
					Debug.LogWarning("Unrecognized HierachyFoldersInPrefabs value: " + preferences.foldersInPrefabs);
					return;
			}

			CreateBackups();
			StripHierarchyFoldersFromAllPrefabs(strippingType);

			PrefabsStripped = true;
		}

		public void OnPostprocessBuild(BuildReport report)
		{
			if(!PrefabsStripped)
			{
				return;
			}

			RestoreBackups();

			PrefabsStripped = false;
		}

		public static void CreateBackups()
		{
			paths = new List<KeyValuePair<string, string>>();

			string backupRootDir = Path.Combine(Application.persistentDataPath, "HierarchyFolders/PrefabBackups");
			var assets = AssetDatabase.FindAssets("t:GameObject");
			foreach(var guid in assets)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guid);
				var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
				if(gameObject == null)
				{
					continue;
				}
				var o = gameObject.GetComponentInChildren<HierarchyFolder>();
				if(o == null)
				{
					continue;
				}

				// Risk of path being too long to write to???
				string backupPath = Path.Combine(backupRootDir, assetPath);

				paths.Add(new KeyValuePair<string, string>(assetPath, backupPath));

				Directory.CreateDirectory(Path.GetDirectoryName(backupPath));

				#if DEV_MODE
				Debug.Log("Creating backup of prefab " + backupPath + "\n@ "+backupPath);
				#endif

				File.Copy(assetPath, backupPath, true);
			}
		}
		
		private static void StripHierarchyFoldersFromAllPrefabs(StrippingType strippingType)
		{
			if(paths != null)
			{
				foreach(var assetAndBackupPath in paths)
				{
					var assetPath = assetAndBackupPath.Key;
					var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
					StripPrefab(gameObject, strippingType);
				}
				return;
			}

			paths = new List<KeyValuePair<string, string>>();

			string backupRootDir = Path.Combine(Application.persistentDataPath, "HierarchyFolders/PrefabBackups");
			var assets = AssetDatabase.FindAssets("t:GameObject");
			foreach(var guid in assets)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guid);
				var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
				if(gameObject == null)
				{
					continue;
				}
				var o = gameObject.GetComponentInChildren<HierarchyFolder>();
				if(o == null)
				{
					continue;
				}

				// Risk of path being too long to write to???
				string backupPath = Path.Combine(backupRootDir, assetPath);

				#if DEV_MODE
				Debug.Log("Stripping prefab " + assetPath);
				#endif

				paths.Add(new KeyValuePair<string, string>(assetPath, backupPath));

				StripPrefab(gameObject, strippingType);
			}
		}

		private static void StripPrefab(GameObject root, StrippingType strippingType)
		{
			var transform = root.transform;
			int childCount = transform.childCount;
			var children = new Transform[childCount];
			for(int n = 0; n < childCount; n++)
			{
				children[n] = transform.GetChild(n);
			}
			for(int n = 0; n < childCount; n++)
			{
				HierarchyFolderUtility.CheckForAndRemoveHierarchyFoldersInChildren(children[n], strippingType, true);
			}
		}

		private static void RestoreBackups(bool delete = true)
		{
			if(paths != null)
			{
				if(paths.Count == 0)
				{
					return;
				}

				AssetDatabase.StartAssetEditing();

				foreach(var assetAndBackupPath in paths)
				{
					var assetPath = assetAndBackupPath.Key;
					var backupPath = assetAndBackupPath.Key;

					#if DEV_MODE
					Debug.Log("Restoring backup of prefab " + assetPath + "\nfrom " + backupPath);
					#endif

					File.Copy(backupPath, assetPath, true);

					if(delete)
					{
						File.Delete(backupPath);
					}
				}

				AssetDatabase.StopAssetEditing();
				AssetDatabase.Refresh();
				return;
			}

			string backupRootDir = Path.Combine(Application.persistentDataPath, "HierarchyFolders/PrefabBackups");
			int removePrefixLength = backupRootDir.Length + 1;
			if(!Directory.Exists(backupRootDir))
			{
				return;
			}

			var backupPaths = Directory.GetFiles(backupRootDir, "*.prefab", SearchOption.AllDirectories);

			if(backupPaths.Length == 0)
            {
				return;
            }

			AssetDatabase.StartAssetEditing();

			foreach(var backupPath in backupPaths)
			{
				string assetPath = backupPath.Substring(removePrefixLength);

				#if DEV_MODE
				Debug.Log("Restoring backup of prefab " + assetPath + "\nfrom " + backupPath);
				#endif

				File.Copy(backupPath, assetPath, true);

				if(delete)
				{
					File.Delete(backupPath);
				}
			}

			AssetDatabase.StopAssetEditing();
			AssetDatabase.Refresh();
		}
	}
}