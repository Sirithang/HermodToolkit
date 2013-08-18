using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(AnimatedSprite))]
public class AnimatedSpriteInspector : SpriteInspector
{


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AnimatedSprite spr = target as AnimatedSprite;

        if (spr.spriteSheet == null)
            return;

        GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

        spr.play = EditorGUILayout.Toggle("Play?", spr.play);
        GUILayout.Label("Current : ");

        string[] names = new string[spr.spriteAnimListInternal.Count];
        int idx = 0;
        int current = 0;
        foreach (SpriteAnimation a in spr.spriteAnimListInternal)
        {
            names[idx] = a.name;

            if (a == spr.current)
            {
                current = idx;
            }
            ++idx;
        }

        int selec = EditorGUILayout.Popup(current, names);
        if (selec != current)
        {
            spr.currentName = names[selec];
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        GUILayout.Label("Name", GUILayout.Width(60));
        GUILayout.Label("Start", GUILayout.Width(60));
        GUILayout.Label("End", GUILayout.Width(60));
        GUILayout.Label("Speed", GUILayout.Width(60));
        GUILayout.Label("WrapMode", GUILayout.Width(60));
        GUILayout.Space(60);

        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();

        List<SpriteAnimation> toRemove = new List<SpriteAnimation>();

        if (spr.spriteAnimListInternal != null)
        {
            List<SpriteAnimation> copyAnim = new List<SpriteAnimation>(spr.spriteAnimListInternal);
            int i = 0;
            foreach (SpriteAnimation anim in copyAnim)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                EditorGUILayout.Space();

                anim.name = GUILayout.TextField(anim.name, GUILayout.Width(60));

                anim.startFrame = EditorGUILayout.IntField(anim.startFrame, GUILayout.Width(60));
                anim.endFrame = EditorGUILayout.IntField(anim.endFrame, GUILayout.Width(60));
                anim.duration = EditorGUILayout.FloatField(anim.duration, GUILayout.Width(60));
                anim.wrapMode = (WrapMode)EditorGUILayout.EnumPopup(anim.wrapMode, GUILayout.Width(60));

                if (GUILayout.Button("-", GUILayout.Width(60)))
                {
                    toRemove.Add(anim);
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();

                ++i;
            }

            foreach (SpriteAnimation a in toRemove)
            {
                copyAnim.Remove(a);
            }

            spr.spriteAnimListInternal.Clear();
            spr.spriteAnimListInternal.AddRange(copyAnim);
            spr.RebuildAnimDictionnary();
        }

        if (GUILayout.Button("+"))
        {
            spr.AddAnimation("NewAnim", new int[] { 0, 1 });
        }

        SpriteAnimation addedAnim = EditorGUILayout.ObjectField("Drag or choose here an anim to add",null, typeof(SpriteAnimation), false) as SpriteAnimation;
        if (addedAnim != null)
        {
            spr.spriteAnimListInternal.Add(addedAnim);
            spr.RebuildAnimDictionnary();
        }

        EditorGUILayout.EndVertical();
    }

    //-----------------------

    [MenuItem("Assets/Create/SpriteAnimation")]
    static void CreateAnimationAsset()
    {
        SpriteAnimation a = SpriteAnimation.CreateInstance<SpriteAnimation>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (path == "")
        {
            path = "Assets";
        }
        else if (System.IO.Path.GetExtension(path) != "")
        {
            path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        AssetDatabase.CreateAsset(a, AssetDatabase.GenerateUniqueAssetPath(path + "/SpriteAnimation.asset"));
    }
}