using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class TurnRound : MonoBehaviour
{
    public RectTransform canvasRectTrans;

    private RectTransform transTurnTable;
    private Transform transLocation;
    private Transform transLocationItem;
    private Transform transAvatarManager;
    private Transform transAvatarItem;
    
    private Dictionary<GameObject, GameObject> dictAvater = new Dictionary<GameObject, GameObject>();

    public float minScale = 0.5f;       //最小缩放系数
    public int avaterCount = 2;         //个数

    private Vector2 originPos;          //圆心
    private Vector2 refDir;             //初始参考方向
    private float radius;               //圆半径
    private float beginPointerX;
    private float beginEulerZ = 0;

    private Vector3 m_vTuntableOriginAngel = Vector3.zero;
	void Awake()
    {
        InitUI();             
    }

    private void InitUI()
    {
        transTurnTable = transform.Find("Turntable").GetComponent<RectTransform>();
        transLocation = transform.Find("Turntable/Location");
        transLocationItem = transform.Find("Turntable/Location/LocationItem");
        transAvatarManager = transform.Find("AvatarManager");
        transAvatarItem = transform.Find("AvatarManager/AvatarItem");
    }

    private void InitParam()
    {
        m_vTuntableOriginAngel = transTurnTable.localEulerAngles;
        originPos = GetPositionInCanvas(transTurnTable.gameObject);
        Vector2 refPoint = GetPositionInCanvas(transLocationItem.gameObject);
        refDir = (refPoint - originPos).normalized;
        radius = transLocationItem.localPosition.y;
    }

    public void InitEvent()
    {
        UGUIDragHandler.Get(transTurnTable.gameObject).onDrag += OnTurntableDrag;
        UGUIDragHandler.Get(transTurnTable.gameObject).onBeginDrag += OnTurntableBeginDrag;
        UGUIDragHandler.Get(transTurnTable.gameObject).onEndDrag += OnTurntableEndDrag;
    }

	void Start () {
        InitParam();
        InitEvent();

        CreateAvater();
	}

    public void CreateAvater()
    {
        for(int i = 1;i <= avaterCount;i++)
        {
            //创建location
            var locationItem = GameObject.Instantiate(transLocationItem.gameObject) as GameObject;
            locationItem.SetActive(true);
            locationItem.transform.SetParent(transLocation);
            locationItem.transform.localScale = Vector3.one;
            locationItem.transform.rotation = Quaternion.identity;

            //从（0，r）开始旋转，结合旋转矩阵得到坐标
            float rotateRadian = (2 * Mathf.PI / avaterCount) * (i - 1);
            float x = -radius * Mathf.Sin(rotateRadian);
            float y = radius * Mathf.Cos(rotateRadian);
            locationItem.transform.localPosition = new Vector3(x, y, -10);

            //创建avater
            var avatarItem = GameObject.Instantiate(transAvatarItem.gameObject) as GameObject;
            avatarItem.SetActive(true);
            avatarItem.transform.SetParent(transAvatarManager);
            avatarItem.transform.localPosition = Vector3.one;
            avatarItem.transform.localScale = Vector3.one;
            //设置属性
            var tex = avatarItem.transform.Find("Text").GetComponent<Text>();
            tex.text = i.ToString();

            dictAvater.Add(locationItem, avatarItem);
        }

        UpdateAvaterTransform();
        AdjustShowOrder();
    }

 
    //更新卡牌位置
    public void UpdateAvaterTransform()
    {
        foreach (var item  in dictAvater)
        {
            var pos = GetPositionInCanvas(item.Key);
            var avaterRT = item.Value.GetComponent<RectTransform>();
            avaterRT.anchoredPosition = pos;

            //计算向量点乘
            Vector2 dir = (pos - originPos).normalized;
            float cos = dir.x * refDir.x + dir.y * refDir.y;

            //缩放
            float scale = (cos + 1 + minScale) / 2;
            scale = Mathf.Clamp(scale, minScale, 1);
            avaterRT.localScale = new Vector3(scale, scale, scale);
        }
    }

    //获得canvas下坐标
    public Vector2 GetPositionInCanvas(GameObject obj)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTrans, obj.transform.position, null, out localPoint))
        {
            return localPoint;
        }

        return Vector2.zero;
    }

    //调整视图上的显示顺序，保证近处遮挡远处
    public void AdjustShowOrder()
    {
        List<GameObject> avaters = new List<GameObject>(dictAvater.Values);
        avaters.Sort(delegate(GameObject a, GameObject b)
        {
            if (a.transform.localPosition.y > b.transform.localPosition.y)
                return -1;

            return 0;
        });

        avaters.ForEach(p =>
        {
            p.transform.SetAsLastSibling();
        });
    }

    //旋转结束限制卡牌到每个分割点位置
    private void OnTurntableEndDrag(GameObject target, PointerEventData eventData)
    {
        float endPointerX = eventData.position.x;
        Vector3 eulerAng = Vector3.forward * (endPointerX - beginPointerX) / 3.1415f;

        float n = eulerAng.z / (360 / avaterCount);
        n = Mathf.RoundToInt(n);
        //print("eulerAng : " + eulerAng + " n = " + n);
        
        if (Mathf.Abs(eulerAng.z) > 180 / avaterCount)
        {
            float z = 0;
            z = beginEulerZ + (360 / avaterCount) * n;
            if (Mathf.Abs(z) >=  360)
            {
                if (z > 0)
                    z = z - 360;
                else
                    z = z + 360;
            }
            beginEulerZ = z;        //更新z
            transTurnTable.DORotate(new Vector3(m_vTuntableOriginAngel.x, m_vTuntableOriginAngel.y, z), 0.2f)
                     .OnUpdate(() => { UpdateAvaterTransform(); })
                     .SetId("OnTurntableEndDrag DORotate");        
 
        }
        else
        {
            transTurnTable.DORotate(new Vector3(m_vTuntableOriginAngel.x, m_vTuntableOriginAngel.y, beginEulerZ), 0.2f)
                     .OnUpdate(() => { UpdateAvaterTransform(); })
                     .SetId("OnTurntableEndDrag DORotate");   
        }
    }

    
    private void OnTurntableBeginDrag(GameObject target, PointerEventData eventData)
    {
        beginPointerX = eventData.position.x;
    }
 
    private void OnTurntableDrag(GameObject target, PointerEventData eventData)
    {
        Vector3 eulerAng = Vector3.forward * eventData.delta.x / 3.1415f;
        transTurnTable.Rotate(eulerAng);

        UpdateAvaterTransform();
        AdjustShowOrder();
    }

    //void OnGUI()
    //{
    //   if(GUILayout.Button("位置调整2->1"))
    //   {
    //       //把2号avater排到第一位置


    //   }
    //}

    ////循环左移
    //public void LeftShift(GameObject[] objs,int shiftCount,int len)
    //{
    //    Reverse(objs, 0, shiftCount - 1);
    //    Reverse(objs, shiftCount, len - 1);
    //    Reverse(objs, 0, len - 1);
    //}

    //// 逆序
    //public GameObject[] Reverse(GameObject[] objs, int i, int j)
    //{
    //    for (int begin = i, end = j; begin < end; begin++, end--)
    //    {
    //        GameObject temp = objs[begin];
    //        objs[begin] = objs[end];
    //        objs[end] = temp;
    //    }
    //    return objs;
    //}
}
