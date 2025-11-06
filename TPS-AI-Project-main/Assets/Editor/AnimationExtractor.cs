using UnityEngine;
using UnityEditor;
using System.IO;

public class AnimationExtractor : EditorWindow
{
    private AnimationClip sourceClip;
    private string savePath = "Assets/ExtractedAnimations/";

    [MenuItem("Tools/Extract Animation Clip")]
    static void Init()
    {
        AnimationExtractor window = (AnimationExtractor)EditorWindow.GetWindow(typeof(AnimationExtractor));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Extract Animation from FBX", EditorStyles.boldLabel);

        sourceClip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", sourceClip, typeof(AnimationClip), false);

        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Extract Animation"))
        {
            if (sourceClip != null)
            {
                ExtractAnimation();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please select an animation clip first!", "OK");
            }
        }
    }

    void ExtractAnimation()
    {
        // Create directory if it doesn't exist
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        // Create new animation clip
        AnimationClip newClip = new AnimationClip();
        newClip.name = sourceClip.name + "_Editable";

        // Copy animation curves
        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(sourceClip);

        foreach (EditorCurveBinding binding in curveBindings)
        {
            AnimationCurve curve = AnimationUtility.GetEditorCurve(sourceClip, binding);
            AnimationUtility.SetEditorCurve(newClip, binding, curve);
        }

        // Copy object reference curves
        EditorCurveBinding[] objectCurveBindings = AnimationUtility.GetObjectReferenceCurveBindings(sourceClip);

        foreach (EditorCurveBinding binding in objectCurveBindings)
        {
            ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(sourceClip, binding);
            AnimationUtility.SetObjectReferenceCurve(newClip, binding, keyframes);
        }

        // Copy animation events
        AnimationEvent[] events = AnimationUtility.GetAnimationEvents(sourceClip);
        AnimationUtility.SetAnimationEvents(newClip, events);

        // Copy animation settings
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(sourceClip);
        AnimationUtility.SetAnimationClipSettings(newClip, settings);

        // Save the new clip
        string assetPath = savePath + newClip.name + ".anim";
        AssetDatabase.CreateAsset(newClip, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success", "Animation extracted to: " + assetPath, "OK");

        // Select the new clip
        Selection.activeObject = newClip;
    }
}