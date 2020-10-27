using System.Collections.Generic;
using System.Diagnostics;
using Mapbox.Unity.Map;
using UnityEngine;

public class MapBoxManager : MonoBehaviour
{
    public AbstractMap map;//地图

    public Camera mapCamera;//地图相机

    public Transform POIContainer;//装POI容器

    [SerializeField]
    private GameObject prefab;//POI预制体

    [HideInInspector]
    public List<Marker> markers;//存储所有的POI

    public float hight = 0;//POI 的海拔高度

    private float _scale = 1f;//POI缩放比例系数

    [SerializeField]
    private float initZoom = 16;//初始化比例尺

    public static MapBoxManager instance;

    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start()
    {
        markers = new List<Marker>();
        map.SetZoom(initZoom);
        //对地图做任何事件操作都会更新POI
        map.OnUpdated += OnUpdatedMarkerts;
    }
    private void OnDestroy()
    {
        map.OnUpdated -= OnUpdatedMarkerts;
    }


    /// <summary>
    /// 更新POI位置状态
    /// </summary>
    private void OnUpdatedMarkerts()
    {
        if (markers != null && markers.Count > 0)
        {
            //POI大小的比例随地图大小变动
            float curretValue = (map.Zoom - initZoom) * 0.5f;
            if (curretValue > 0.5f)
            {
                curretValue = 0.5f;
            }
            else if (curretValue < -0.5f)
            {
                curretValue = -0.5f;
            }

            //Debug.Log(curretValue);
            foreach (var m in markers)
            {
                //经纬度坐标转Unity世界坐标
                var pos = MapHelper.instance.GeoToWorldPositon(m.longitude, m.latitude);
                m.transform.localPosition = new Vector3(pos.x, hight, pos.z);
                var scale = m.transform.localScale;
                m.transform.localScale = new Vector3(1 + curretValue, 1, 1 + curretValue);//这里初始化时默认POI大小为 Vector3.one
            }
        }
    }


    /// <summary>
    /// 查找一个POI
    /// </summary>
    public Marker FindMarker(string uuid)
    {
        return markers.Find(x => x.uuid == uuid);
    }
    /// <summary>
    /// 删除POi点
    /// </summary>
    public void RemoveMarker(string uuid)
    {
        var m = markers.Find(x => x.uuid == uuid);
        RemoveMarker(m);
    }
    /// <summary>
    /// 删除POI点
    /// </summary>
    public void RemoveMarker(Marker m)
    {
        if (markers != null && markers.Count > 0)
        {
            markers.Remove(m);
        }
    }
    /// <summary>
    /// 创建POI点
    /// </summary>
    public void CreateMarker(Marker m)
    {
        CreateMarker(m.uuid, m.type, m.longitude, m.latitude, m.customData);
    }
    /// <summary>
    /// 创建POI点
    /// </summary>
    public void CreateMarker(string uuid, string type, double lon, double lat, object custom)
    {
        var current = Instantiate(prefab, POIContainer);
        Marker m = current.AddComponent<Marker>();
        m.uuid = uuid;
        m.type = type;
        m.latitude = lat;
        m.longitude = lon;
        m.customData = custom;
        m.Init += Init;
        m.OnEnter += OnEnter;
        m.OnExit += OnExit;
        m.OnLeftClick += OnLeftClick;
        m.OnRightClick += OnRightClick;
        m.OnDoubleClick += OnDoubleClick;
        m.OnDrag += OnDrag;
        m.OnEndDrag += OnEndDrag;
        m.OnDestory += OnDestory;
        markers.Add(m);
    }
    private void Init(MarkerBase obj)
    {

        UnityEngine.Debug.Log("初始化！");
        var pos = MapHelper.instance.GeoToWorldPositon(obj.longitude, obj.latitude);
        obj.transform.localPosition = new Vector3(pos.x, hight, pos.z);
    }
    private void OnEnter(MarkerBase obj)
    {
        UnityEngine.Debug.Log("移入！");
    }
    private void OnExit(MarkerBase obj)
    {
        UnityEngine.Debug.Log("移出！");
    }
    private void OnLeftClick(MarkerBase obj)
    {
        UnityEngine.Debug.Log("左击！");
    }

    private void OnRightClick(MarkerBase obj)
    {
        UnityEngine.Debug.Log("右击！");
    }

    private void OnDrag(MarkerBase obj)
    {
        UnityEngine.Debug.Log("拖拽中！");
    }
    private void OnEndDrag(MarkerBase obj)
    {
        UnityEngine.Debug.Log("拖拽结束！");
    }

    private void OnDoubleClick(MarkerBase obj)
    {
        UnityEngine.Debug.Log("双击！");
    }

    private void OnDestory(MarkerBase obj)
    {
        UnityEngine.Debug.Log("销毁！");
    }

}

