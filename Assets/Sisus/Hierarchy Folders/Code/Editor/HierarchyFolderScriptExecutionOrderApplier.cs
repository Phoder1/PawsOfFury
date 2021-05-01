using UnityEditor;
using JetBrains.Annotations;

namespace Sisus.HierarchyFolders
{
	[UsedImplicitly, InitializeOnLoad]
	public static class HierarchyFolderScriptExecutionOrderApplier
	{
		private const int ScriptExecutionOrder = -32000;

		static HierarchyFolderScriptExecutionOrderApplier()
		{
			EditorApplication.delayCall += ApplyScriptExecutionOrder;
		}

		private static void ApplyScriptExecutionOrder()
		{
			if(EditorApplication.isPlayingOrWillChangePlaymode || BuildPipeline.isBuildingPlayer)
			{
				return;
			}

			if(EditorApplication.isUpdating || EditorApplication.isCompiling)
			{
				EditorApplication.delayCall += ApplyScriptExecutionOrder;
				return;
			}

			ScriptExecutionOrderUtility.Set<HierarchyFolder>(ScriptExecutionOrder);
		}
	}
}