﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Crosstales.FB.EditorTask
{
   /// <summary>Moves all resources to 'Editor Default Resources'.</summary>
   [InitializeOnLoad]
   public abstract class SetupResources : Common.EditorTask.BaseSetupResources
   {
      #region Constructor

      static SetupResources()
      {
         Setup();
      }

      #endregion


      #region Public methods

      public static void Setup()
      {
#if !CT_DEVELOP
         string path = Application.dataPath;
         string assetpath = $"Assets{EditorUtil.EditorConfig.ASSET_PATH}";

         string sourceFolder = $"{path}{EditorUtil.EditorConfig.ASSET_PATH}Icons/";
         string source = $"{assetpath}Icons/";

         string targetFolder = $"{path}/Editor Default Resources/crosstales/FileBrowser/";
         string target = "Assets/Editor Default Resources/crosstales/FileBrowser/";
         string metafile = $"{assetpath}Icons.meta";

         setupResources(source, sourceFolder, target, targetFolder, metafile);
#endif
      }

      #endregion
   }
}
#endif
// © 2019-2021 crosstales LLC (https://www.crosstales.com)