//-------------------------------------------------------------------------
/**
* file   : DoTween.cs
* author : kadu
* created: 2016-6-2 10:07
* purpose: 
*/
//-------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

namespace Kadu.Tool
{
    public class DoTween : MonoBehaviour
    {
        public bool PlayInEnable;
        [HideInInspector]
        public List<DoTweenNode> nodes;
        private List<Tweener> mTweeners = new List<Tweener>();

        void OnEnable()
        {
            if (PlayInEnable)
                Play();
        }

        public bool IsPlaying()
        {
            foreach (Tweener node in mTweeners)
            {
                if (node.IsActive())
                    return true;
            }
            return false;
        }

        public void Play()
        {
            for (int i = 0; i < mTweeners.Count; ++i)
            {
                Tweener node = mTweeners[i];
                if (node.IsActive())
                    node.Kill();
            }
            mTweeners.Clear();

            for (int j = 0; j < nodes.Count; ++j)
            {
                DoTweenNode node = nodes[j];
                Tweener tweener = null;

                node.parent = gameObject;

                if (node.type == DoTweenType.Position)
                {
                    Vector3 tVector3;

                    if ((node.valFlagF & DoTweenValFlag.MySelf) == 0)
                    {
                        if (node.isLocal)
                            transform.localPosition = node.fVector3;
                        else
                            transform.position = node.fVector3;
                    }

                    if ((node.valFlagT & DoTweenValFlag.MySelf) > 0)
                    {
                        if (node.isLocal)
                            tVector3 = transform.localPosition;
                        else
                            tVector3 = transform.position;
                    }
                    else
                    {
                        tVector3 = node.tVector3;
                    }
                    
                    if (node.isLocal)
                    {
                        tweener = DOTween.To(
                            () => transform.localPosition,
                            position =>
                            {
                                transform.localPosition = position;
                            },
                            tVector3,
                            node.duration);
                    }
                    else
                    {
                        tweener = DOTween.To(
                            () => transform.position,
                            position =>
                            {
                                transform.position = position;
                            },
                            tVector3,
                            node.duration);
                    }
                }
                else if (node.type == DoTweenType.Scale)
                {
                    transform.localScale = node.fVector3;
                    tweener = transform.DOScale(node.tVector3, node.duration);
                }
                else if (node.type == DoTweenType.Rotation)
                {
                    Vector3 tVector3;

                    if ((node.valFlagF & DoTweenValFlag.MySelf) == 0)
                    {
                        if (node.isLocal)
                            transform.localRotation = Quaternion.Euler(node.fVector3);
                        else
                            transform.rotation = Quaternion.Euler(node.fVector3);
                    }

                    if ((node.valFlagT & DoTweenValFlag.MySelf) > 0)
                    {
                        if (node.isLocal)
                            tVector3 = transform.localRotation.eulerAngles;
                        else
                            tVector3 = transform.rotation.eulerAngles;
                    }
                    else
                    {
                        tVector3 = node.tVector3;
                    }

                    if (node.isLocal)
                    {
                        tweener = DOTween.To(
                            () => transform.localRotation.eulerAngles,
                            angle =>
                            {
                                transform.localRotation = Quaternion.Euler(angle);
                            },
                            tVector3,
                            node.duration);
                    }
                    else
                    {
                        tweener = DOTween.To(
                            () => transform.rotation.eulerAngles,
                            angle =>
                            {
                                transform.rotation = Quaternion.Euler(angle);
                            },
                            tVector3,
                            node.duration);
                    }
                }
                else if (node.type == DoTweenType.Color)
                {
                    Graphic graphic = GetComponent<Graphic>();
                    if (graphic != null)
                    {
                        graphic.color = node.fColor;
                        tweener = graphic.DOColor(node.tColor, node.duration);
                    }

                    
                }
                else if (node.type == DoTweenType.Alpha)
                {
                    Graphic[] comps = null;
                    if (node.isInfluenceChild)
                        comps = GetComponentsInChildren<Graphic>(true);
                    else
                        comps = GetComponents<Graphic>();

                    for (int i = 0; i < comps.Length; i++)
                    {
                        Color color = comps[i].color;
                        color.a = node.fFloat;
                        comps[i].color = color;
                    }

                    if (comps != null)
                        tweener = DOTween.To(alpha =>
                        {
                            for (int i = 0; i < comps.Length; i++)
                            {
                                Color color = comps[i].color;
                                color.a = alpha;
                                comps[i].color = color;
                            }
                        }, node.fFloat, node.tFloat, node.duration);
                }
                else if (node.type == DoTweenType.Material)
                {
                    Graphic graphic = GetComponent<Graphic>();
                    if (graphic != null)
                    {
                        graphic.material = new Material(Shader.Find("Kadu/UI/Bloom"));
                        graphic.material.SetColor("_Color", Color.white);
                        tweener = DOTween.To(value =>
                        {
                            graphic.material.SetFloat("_Strength", value);
                        }, node.fFloat, node.tFloat, node.duration);
                    }
                }

                if (tweener != null)
                {
                    mTweeners.Add(tweener);

                    // 循环
                    tweener.SetLoops(node.loops, node.loopType);

                    // 延迟
                    tweener.SetDelay(node.delay);

                    //设置这个Tween不受Time.scale影响
                    tweener.SetUpdate(node.isIndependentUpdate);

                    //设置移动类型
                    tweener.SetEase(node.ease);

                    // 自动删除
                    tweener.SetAutoKill();

                    // 完成回调
                    tweener.OnComplete(node.OnComplete);
                }
            }
        }


        //public Vector3 CalculateRootPosition(Transform transform)
        //{
        //    Vector3 position = Vector3.zero;
        //    Transform tf = transform;
        //    while (tf != null)
        //    {
        //        position += tf.localPosition;
        //        tf = tf.parent;
        //    }
        //    return position;
        //}

        //public static void FlyTo(Graphic graphic)
        //{
        //    RectTransform rt = graphic.rectTransform;
        //    Color c = graphic.color;
        //    c.a = 0;
        //    graphic.color = c;
        //    Sequence mySequence = DOTween.Sequence();
        //    Tweener move1 = rt.DOMoveY(rt.position.y + 50, 0.5f);
        //    Tweener move2 = rt.DOMoveY(rt.position.y + 100, 0.5f);
        //    Tweener alpha1 = graphic.DOColor(new Color(c.r, c.g, c.b, 1), 0.5f);
        //    Tweener alpha2 = graphic.DOColor(new Color(c.r, c.g, c.b, 0), 0.5f);
        //    mySequence.Append(move1);
        //    mySequence.Join(alpha1);
        //    mySequence.AppendInterval(1);
        //    mySequence.Append(move2);
        //    mySequence.Join(alpha2);
        //}

        //void OnDrawGizmosSelected()
        //{
        //    if (nodes == null)
        //        return;
        //}
    }
}


