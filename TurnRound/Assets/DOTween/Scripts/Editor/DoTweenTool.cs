using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Kadu.Tool
{
    public static class DoTweenTool
    {
        static public bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
        {
            bool state = EditorPrefs.GetBool(key, true);

            if (!minimalistic) GUILayout.Space(3f);
            if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal();
            GUI.changed = false;

            if (minimalistic)
            {
                if (state) text = "\u25BC" + (char)0x200a + text;
                else text = "\u25BA" + (char)0x200a + text;

                GUILayout.BeginHorizontal();
                GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
                if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
                GUI.contentColor = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                text = "<b><size=11>" + text + "</size></b>";
                if (state) text = "\u25BC " + text;
                else text = "\u25BA " + text;
                if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
            }

            if (GUI.changed) EditorPrefs.SetBool(key, state);

            if (!minimalistic) GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            if (!forceOn && !state) GUILayout.Space(3f);
            return state;
        }

        static public void DrawVector3(DoTweenNode node)
        {
            EditorGUILayout.BeginHorizontal();
            node.fVector3 = EditorGUILayout.Vector3Field("开始值", node.fVector3);
            node.valFlagF = (DoTweenValFlag)EditorGUILayout.EnumMaskField(node.valFlagF, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            node.tVector3 = EditorGUILayout.Vector3Field("结束值", node.tVector3);
            node.valFlagT = (DoTweenValFlag)EditorGUILayout.EnumMaskField(node.valFlagT, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();
        }

        //static public void DrawVector3(DoTweenNode node)
        //{
        //    node.fPositon = EditorGUILayout.Vector3Field("开始值", node.fPositon);
        //    node.tPositon = EditorGUILayout.Vector3Field("结束值", node.tPositon);
        //}

        static public void DrawColor(DoTweenNode node)
        {
            node.fColor = EditorGUILayout.ColorField("开始值", node.fColor);
            node.tColor = EditorGUILayout.ColorField("结束值", node.tColor);
        }

        static public void DrawFloat(DoTweenNode node)
        {
            node.fFloat = EditorGUILayout.FloatField("开始值", node.fFloat);
            node.tFloat = EditorGUILayout.FloatField("结束值", node.tFloat);
        }

    }
}