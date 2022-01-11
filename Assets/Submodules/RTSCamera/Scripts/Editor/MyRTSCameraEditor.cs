using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RTSCamera))]
public class MyRTSCameraEditor : Editor
{
    Texture2D logoTex;
    Texture2D backGroundTex;
    public SerializedProperty orbitalCam, mouseButtonMove, mouseButtonRotate, cameraBaseHeight, startingOffset, baseCameraSpeed, fastCameraSpeed, movementSmoothness, dragSpeed, maxDragSpeed, borderPan, borderThickness, hideCursor, orbitalSpeed, orbitalSmoothness,
        keysRotationFactor, zoomAmt, zoomSpeed, zoomSmoothness, maxInZoom, maxOutZoom, focusMask;
    private int currentTab;

    private void OnEnable()
    {
        orbitalCam = serializedObject.FindProperty("orbitalCam");
        mouseButtonMove = serializedObject.FindProperty("mouseButtonMove");
        mouseButtonRotate = serializedObject.FindProperty("mouseButtonRotate");
        cameraBaseHeight = serializedObject.FindProperty("cameraBaseHeight");
        startingOffset = serializedObject.FindProperty("startingOffset");
        baseCameraSpeed = serializedObject.FindProperty("baseCameraSpeed");
        fastCameraSpeed = serializedObject.FindProperty("fastCameraSpeed");
        movementSmoothness = serializedObject.FindProperty("movementSmoothness");
        dragSpeed = serializedObject.FindProperty("dragSpeed");
        maxDragSpeed = serializedObject.FindProperty("maxDragSpeed");
        borderPan = serializedObject.FindProperty("borderPan");
        borderThickness = serializedObject.FindProperty("borderThickness");
        hideCursor = serializedObject.FindProperty("hideCursor");
        orbitalSpeed = serializedObject.FindProperty("orbitalSpeed");
        orbitalSmoothness = serializedObject.FindProperty("orbitalSmoothness");
        keysRotationFactor = serializedObject.FindProperty("keysRotationFactor");
        zoomAmt = serializedObject.FindProperty("zoomAmt");
        zoomSpeed = serializedObject.FindProperty("zoomSpeed");
        zoomSmoothness = serializedObject.FindProperty("zoomSmoothness");
        maxInZoom = serializedObject.FindProperty("maxInZoom");
        maxOutZoom = serializedObject.FindProperty("maxOutZoom");
        focusMask = serializedObject.FindProperty("focusMask");

        logoTex = (Texture2D)EditorGUIUtility.Load("Assets/MyRTSCamera/Scripts/Editor/Images/RTSCameraHeader.png");
        backGroundTex = (Texture2D)EditorGUIUtility.Load("Assets/MyRTSCamera/Scripts/Editor/Images/RTSCameraBg.png");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUI.color = Color.white;
        EditorGUILayout.LabelField("");
        DrawBackground();
        DrawLogo();
        GUILayout.Space(10);

        currentTab = GUILayout.Toolbar(currentTab, new string[] { "General", "Move", "Rotation", "Zoom" });

        switch (currentTab)
        {
            case 0:
                EditorGUILayout.PropertyField(orbitalCam, new GUIContent("Orbital Cam"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(startingOffset, new GUIContent("Starting Offset"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(mouseButtonMove, new GUIContent("Mouse Button Move"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(mouseButtonRotate, new GUIContent("Mouse Button Rotate"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(focusMask, new GUIContent("Focus Mask"));
                EditorGUILayout.Space(2f);
                break;
            case 1:
                EditorGUILayout.PropertyField(cameraBaseHeight, new GUIContent("Camera Base Height"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(baseCameraSpeed, new GUIContent("Base Camera Speed"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(fastCameraSpeed, new GUIContent("Fast Camera Speed"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(movementSmoothness, new GUIContent("Movement Smoothness"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(dragSpeed, new GUIContent("Drag Speed"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(maxDragSpeed, new GUIContent("Max Drag Speed"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(borderPan, new GUIContent("Border Pan"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(borderThickness, new GUIContent("Border Thickness"));
                EditorGUILayout.Space(2f);
                break;
            case 2:
                EditorGUILayout.PropertyField(hideCursor, new GUIContent("Hide Cursor"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(orbitalSpeed, new GUIContent("Orbital Speed"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(orbitalSmoothness, new GUIContent("Orbital Smoothness"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(keysRotationFactor, new GUIContent("Keys Rotation Factor"));
                EditorGUILayout.Space(2f);
                break;
            case 3:
                EditorGUILayout.PropertyField(zoomAmt, new GUIContent("Zoom Amount"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(zoomSpeed, new GUIContent("Zoom Speed"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(zoomSmoothness, new GUIContent("Zoom Smoothness"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(maxInZoom, new GUIContent("Max In Zoom"));
                EditorGUILayout.Space(2f);
                EditorGUILayout.PropertyField(maxOutZoom, new GUIContent("Max Out Zoom"));
                EditorGUILayout.Space(2f);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawLogo()
    {
        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.DrawTexture(new Rect(0, rect.yMin + 20, EditorGUIUtility.currentViewWidth, 130), logoTex, ScaleMode.ScaleToFit);
        GUILayout.Space(145);
    }
    private void DrawBackground()
    {
        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = GUI.color = Color.white;
        GUI.DrawTexture(new Rect(0, rect.yMin, EditorGUIUtility.currentViewWidth, 420), backGroundTex);
    }
}
