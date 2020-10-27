using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class MapHelper : MonoBehaviour
{
    public static MapHelper instance;
    private AbstractMap _map;
    private Camera mapCamera;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        _map = MapBoxManager.instance.map;
        mapCamera = MapBoxManager.instance.mapCamera;
    }

    /// <summary>
    /// 设置2D地图的中心点
    /// </summary>
    /// <param name="lon"></param>
    /// <param name="lat"></param>
    private void SetMapCenter(double lon, double lat)
    {
        Vector2d latlng = new Vector2d
        {
            x = lat,
            y = lon,
        };
        _map.UpdateMap(latlng);
    }

    /// <summary>
    /// 经纬度坐标转Unity世界坐标
    /// </summary>
    public Vector3 GeoToWorldPositon(double lon, double lat)
    {
        Vector2d location = new Vector2d()
        {
            x = lat,
            y = lon
        };
        //地理坐标转世界坐标
        return _map.GeoToWorldPosition(location, true);
    }
    /// <summary>
    /// Unity世界坐标转世界坐标
    /// </summary>
    public void WorldToGeoPositon(Vector3 position, out double lon, out double lat)
    {
        Vector2d location = _map.WorldToGeoPosition(position);
        lon = location.y;
        lat = location.x;
    }
    /// <summary>
    /// 经纬度转换屏幕坐标
    /// </summary>
    public Vector3 GetScreenPoint(double lng, double lat)
    {
        Vector2d latlng = new Vector2d
        {
            x = lat,
            y = lng,
        };

        var pos = _map.GeoToWorldPosition(latlng);
        pos.y = 0;
        Vector3 screenPos = mapCamera.WorldToScreenPoint(pos);
        return new Vector2(screenPos.x, screenPos.y);
    }
    /// <summary>
    /// 世界坐标转屏幕坐标
    /// </summary>
    public Vector2 GetScreenPoint(Vector3 pos)
    {
        Vector3 screenPos = mapCamera.WorldToScreenPoint(pos);
        return new Vector2(screenPos.x, screenPos.y);
    }
    /// <summary>
    /// 世界坐标转经纬度
    /// </summary>
    public void GetLatlng(Vector3 position, out double lng, out double lat)
    {
        Vector3 mousePosScreen = GetScreenPoint(position);
        mousePosScreen.z = mapCamera.transform.localPosition.y;
        var pos = mapCamera.ScreenToWorldPoint(mousePosScreen);
        Vector2d location = _map.WorldToGeoPosition(pos);
        lng = location.y;
        lat = location.x;
    }
    /// <summary>
    /// 屏幕坐标转获取经纬度
    /// </summary>
    public void GetLatlng(Vector2 mousePositiong, out double lng, out double lat)
    {
        Vector3 mousePosScreen = mousePositiong;
        mousePosScreen.z = mapCamera.transform.localPosition.y;
        var pos = mapCamera.ScreenToWorldPoint(mousePosScreen);
        Vector2d location = _map.WorldToGeoPosition(pos);
        lng = location.y;
        lat = location.x;
    }

}

