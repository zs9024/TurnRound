using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace Kadu.Tool
{
    [CustomEditor(typeof(DoTween))]
    public class DoTweenInspector : Editor
    {
        private DoTween mTarget;

        void OnEnable()
        {
            mTarget = (DoTween)target;
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Space(Screen.width - 80);
            if (GUILayout.Button("添加动画", GUILayout.Width(60)))
            {
                if (mTarget.nodes == null)
                    mTarget.nodes = new List<DoTweenNode>();
                mTarget.nodes.Add(new DoTweenNode());
            }
            GUILayout.EndHorizontal();

            if (mTarget.nodes != null)
            {
                for (int i = 0; i < mTarget.nodes.Count;++i )
                {
                    DoTweenNode node = mTarget.nodes[i];

                    bool isOpen = DoTweenTool.DrawHeader("anim", "anim" + i.ToString(), false, false);
                    

                    if(isOpen)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(Screen.width - 60);
                        if (GUILayout.Button("x", GUILayout.Width(40)))
                        {
                            mTarget.nodes.RemoveAt(i);
                            EditorGUILayout.EndHorizontal();
                            break;
                        }
                        EditorGUILayout.EndHorizontal();

                        GUI.changed = false;

                        node.type = (DoTweenType)EditorGUILayout.EnumPopup("类型", node.type);
                        node.ease = (DG.Tweening.Ease)EditorGUILayout.EnumPopup("插值类型", node.ease);
                        node.loopType = (DG.Tweening.LoopType)EditorGUILayout.EnumPopup("循环类型", node.loopType);
                        node.loops = EditorGUILayout.IntField("循环的次数", node.loops);
                        node.isIndependentUpdate = EditorGUILayout.Toggle("不受时间暂停影响", node.isIndependentUpdate);

                        node.delay = EditorGUILayout.FloatField("延迟", node.delay);
                        node.duration = EditorGUILayout.FloatField("时间", node.duration);
                        node.completer = (GameObject)EditorGUILayout.ObjectField("完成回调者", node.completer, typeof(GameObject), true);
                        node.completeFun = EditorGUILayout.TextField("   回调函数", node.completeFun);

                        switch (node.type)
                        {
                            case DoTweenType.Position:
                                {
                                    node.isLocal = EditorGUILayout.Toggle("局部坐标", node.isLocal);
                                    DoTweenTool.DrawVector3(node);
                                    break;
                                }
                            case DoTweenType.Scale:
                                {
                                    DoTweenTool.DrawVector3(node);
                                    break;
                                }
                            case DoTweenType.Rotation:
                                {
                                    node.isLocal = EditorGUILayout.Toggle("局部坐标", node.isLocal);
                                    DoTweenTool.DrawVector3(node);
                                    break;
                                }
                            case DoTweenType.Color:
                                {
                                    DoTweenTool.DrawColor(node);
                                    break;
                                }
                            case DoTweenType.Alpha:
                                {
                                    node.isInfluenceChild = EditorGUILayout.Toggle("影响子节点", node.isInfluenceChild);
                                    DoTweenTool.DrawFloat(node);
                                    break;
                                }
                            case DoTweenType.Material:
                                {
                                    DoTweenTool.DrawFloat(node);
                                    break;
                                }
                        }

                        if(GUI.changed)
                        {
                            EditorUtility.SetDirty(mTarget);
                        }
                    }
                }
            }
        }
    }
}