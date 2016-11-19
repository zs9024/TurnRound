//-------------------------------------------------------------------------
/**
* file   : DoTweenNode.cs
* author : kadu
* created: 2016-6-2 10:13
* purpose: 
*/
//-------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Kadu.Tool
{
    public enum DoTweenType
    {
        Color,
        Alpha,
        Position,
        Rotation,
        Scale,
        Material,
    }

    public enum DoTweenValFlag
    {
        MySelf = 0x01
    }

    [System.Serializable]
    public class DoTweenNode
    {
        public DoTweenType type;
        public Ease ease = Ease.Linear;
        public LoopType loopType;
        public int loops;
        public bool isIndependentUpdate = false;
        public bool isInfluenceChild = false;      // 包括孩子节点
        public bool isLocal = false;               // 是否是局部的
        public float delay = 0;
        public float duration = 1f;
        public DoTweenValFlag valFlagF;        // 位置信息属性
        public DoTweenValFlag valFlagT;        // 位置信息属性

        // Finish Event
        public GameObject parent;
        public GameObject completer;
        public string completeFun = "OnTweenComplete";
        public System.Action<GameObject, object[]> onComplete;
        public object[] agrs;

        // Vector3
        public Vector3 fVector3;
        public Vector3 tVector3;

        // Color
        public Color fColor;
        public Color tColor;

        // float
        public float fFloat;
        public float tFloat;

        public void OnComplete()
        {
            if (completer && !string.IsNullOrEmpty(completeFun))
                completer.SendMessage(completeFun, SendMessageOptions.DontRequireReceiver);

            if (onComplete != null)
            {
                onComplete(parent, agrs);
                onComplete = null;
            }
        }

        public void OnComplete(System.Action<GameObject, object[]> _onComplete, params object[] _agrs)
        {
            onComplete = _onComplete;
            agrs = _agrs;
        }
    }
}