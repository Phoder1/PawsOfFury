using UnityEngine;
using UnityEditor;

namespace Sisus.HierarchyFolders
{
	public static class ScriptExecutionOrderUtility
	{
		public static void Set<TMonoBehaviour>(int executionOrder) where TMonoBehaviour : MonoBehaviour
		{
			var hierarchyFolderScript = FileUtility.FindScriptAssetForType(typeof(TMonoBehaviour));
			if(MonoImporter.GetExecutionOrder(hierarchyFolderScript) != executionOrder)
			{
				MonoImporter.SetExecutionOrder(hierarchyFolderScript, executionOrder);
			}
		}
	}
}