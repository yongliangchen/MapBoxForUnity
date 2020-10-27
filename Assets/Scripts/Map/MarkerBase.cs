using System;
using UnityEngine;

public class MarkerBase : MonoBehaviour
{
    /// <summary>
    /// 唯一标识符
    /// </summary>
    public string uuid { get; set; }
    /// <summary>
    /// 名字
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// 类型
    /// </summary>
    public string type { get; set; }
    /// <summary>
    /// 经度
    /// </summary>
    public double longitude { get; set; }
    /// <summary>
    /// 纬度
    /// </summary>
    public double latitude { get; set; }
    /// <summary>
    /// 对象类
    /// </summary>
    public object customData { get; set; }
    /// <summary>
    /// 初始化
    /// </summary>
    public Action<MarkerBase> Init;
    /// <summary>
    /// 移入事件
    /// </summary>
    public Action<MarkerBase> OnEnter;
    /// <summary>
    /// 移出事件
    /// </summary>
    public Action<MarkerBase> OnExit;
    /// <summary>
    /// 双击事件
    /// </summary>
    public Action<MarkerBase> OnDoubleClick;
    /// <summary>
    /// 左击击事件
    /// </summary>
    public Action<MarkerBase> OnLeftClick;
    /// <summary>
    /// 左击击事件
    /// </summary>
    public Action<MarkerBase> OnRightClick;
    /// <summary>
    /// 拖拽开始事件
    /// </summary>
    public Action<MarkerBase> OnBeginDrag;
    /// <summary>
    /// 拖拽中事件
    /// </summary>
    public Action<MarkerBase> OnDrag;
    /// <summary>
    /// 拖拽结束事件，
    /// UI:鼠标拖拽已经封装好 直接在这对拖拽结束进行复制即可
    /// </summary>
    public Action<MarkerBase> OnEndDrag;
    /// <summary>
    /// 初始化
    /// </summary>
    public Action<MarkerBase> OnDestory;
}

