#if UNITY_EDITOR
using UnityEditor;
using Crosstales.FB.EditorUtil;

namespace Crosstales.FB.EditorIntegration
{
   /// <summary>Editor component for the "Hierarchy"-menu.</summary>
   public static class FileBrowserGameObject
   {
      [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/" + Util.Constants.FB_SCENE_OBJECT_NAME, false, EditorHelper.GO_ID)]
      private static void AddFB()
      {
         EditorHelper.InstantiatePrefab(Util.Constants.FB_SCENE_OBJECT_NAME);
      }

      [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/" + Util.Constants.FB_SCENE_OBJECT_NAME, true)]
      private static bool AddFBValidator()
      {
         return !EditorHelper.isFileBrowserInScene;
      }
   }
}
#endif
// © 2020-2021 crosstales LLC (https://www.crosstales.com)