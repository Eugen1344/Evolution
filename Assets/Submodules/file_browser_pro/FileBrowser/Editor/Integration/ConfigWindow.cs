#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Crosstales.FB.EditorUtil;

namespace Crosstales.FB.EditorIntegration
{
   /// <summary>Editor window extension.</summary>
   //[InitializeOnLoad]
   public class ConfigWindow : ConfigBase
   {
      #region Variables

      private int tab;
      private int lastTab;
      private string path;

      private Vector2 scrollPosPrefabs;

      #endregion


      #region EditorWindow methods

      //[MenuItem("Window/" + Util.Constants.ASSET_NAME, false, 1010)]
      [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Configuration...", false, EditorHelper.MENU_ID + 1)]
      public static void ShowWindow()
      {
         GetWindow(typeof(ConfigWindow));
      }

      public static void ShowWindow(int tab)
      {
         ConfigWindow window = GetWindow(typeof(ConfigWindow)) as ConfigWindow;
         if (window != null) window.tab = tab;
      }

      public void OnEnable()
      {
         titleContent = new GUIContent(Util.Constants.ASSET_NAME_SHORT, EditorHelper.Logo_Asset_Small);
      }

      public void OnDestroy()
      {
         save();
      }

      public void OnLostFocus()
      {
         save();
      }

      public void OnGUI()
      {
         tab = GUILayout.Toolbar(tab, new[] {"Config", "Prefabs", "TD", "Help", "About"});

         if (tab != lastTab)
         {
            lastTab = tab;
            GUI.FocusControl(null);
         }

         switch (tab)
         {
            case 0:
            {
               showConfiguration();

               EditorHelper.SeparatorUI();

               GUILayout.BeginHorizontal();
               {
                  if (GUILayout.Button(new GUIContent(" Save", EditorHelper.Icon_Save, "Saves the configuration settings for this project.")))
                  {
                     save();
                  }

                  if (GUILayout.Button(new GUIContent(" Reset", EditorHelper.Icon_Reset, "Resets the configuration settings for this project.")))
                  {
                     if (EditorUtility.DisplayDialog("Reset configuration?", $"Reset the configuration of {Util.Constants.ASSET_NAME}?", "Yes", "No"))
                     {
                        Util.Config.Reset();
                        EditorConfig.Reset();
                        save();
                     }
                  }
               }
               GUILayout.EndHorizontal();

               GUILayout.Space(6);
               break;
            }
            case 1:
               showPrefabs();
               break;
            case 2:
               showTestDrive();
               break;
            case 3:
               showHelp();
               break;
            default:
               showAbout();
               break;
         }
      }

      public void OnInspectorUpdate()
      {
         Repaint();
      }

      #endregion

      private void showPrefabs()
      {
         scrollPosPrefabs = EditorGUILayout.BeginScrollView(scrollPosPrefabs, false, false);
         {
            GUILayout.Label("Available Prefabs", EditorStyles.boldLabel);

            GUILayout.Space(6);

            GUI.enabled = !EditorHelper.isFileBrowserInScene;

            GUILayout.Label(Util.Constants.FB_SCENE_OBJECT_NAME);

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, $"Adds a '{Util.Constants.FB_SCENE_OBJECT_NAME}'-prefab to the scene.")))
            {
               EditorHelper.InstantiatePrefab(Util.Constants.FB_SCENE_OBJECT_NAME);
            }

            GUI.enabled = true;

            if (EditorHelper.isFileBrowserInScene)
            {
               GUILayout.Space(6);
               EditorGUILayout.HelpBox("All available prefabs are already in the scene.", MessageType.Info);
            }

            GUILayout.Space(6);
         }
         EditorGUILayout.EndScrollView();
      }

      private void showTestDrive()
      {
         GUILayout.Space(3);
         GUILayout.Label("Test-Drive", EditorStyles.boldLabel);

         if (Util.Helper.isEditorMode)
         {
            if (EditorHelper.isFileBrowserInScene)
            {
               if (FileBrowser.Instance.isWorkingInEditor)
               {
                  GUILayout.Space(6);

                  if (GUILayout.Button(new GUIContent(" Open Single File", EditorUtil.EditorHelper.Icon_File, "Opens a single file.")))
                     path = FileBrowser.Instance.OpenSingleFile();

                  GUILayout.Space(6);

                  if (GUILayout.Button(new GUIContent(" Open Single Folder", EditorUtil.EditorHelper.Icon_Folder, "Opens a single folder.")))
                     path = FileBrowser.Instance.OpenSingleFolder();

                  GUILayout.Space(6);

                  if (GUILayout.Button(new GUIContent(" Save File", EditorUtil.EditorHelper.Icon_Save, "Saves a file.")))
                     path = FileBrowser.Instance.SaveFile();

                  GUILayout.Space(6);

                  //GUILayout.Label($"Path: {(string.IsNullOrEmpty(path) ? "nothing selected" : path)}");
                  EditorGUILayout.SelectableLabel($"{(string.IsNullOrEmpty(path) ? "Path: nothing selected" : path)}");

                  GUILayout.Space(6);
               }
               else
               {
                  EditorGUILayout.HelpBox("Test-Drive is not supported for the current wrapper/platform.", MessageType.Info);
               }
            }
            else
            {
               EditorHelper.FBUnavailable();
            }
         }
         else
         {
            EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
         }
      }
   }
}
#endif
// © 2019-2021 crosstales LLC (https://www.crosstales.com)