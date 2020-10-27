using UnityEngine;
using UnityEngine.EventSystems;

public class Marker : MarkerBase, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    //XXX：拖拽物体需要检测
    /// <summary>
    /// 目标对象的屏幕坐标
    /// </summary>
    private Vector3 targetScreenPoint;
    /// <summary>
    /// 获得鼠标的位置和cube位置差
    /// </summary>
    private Vector3 offset;

    private bool isDrag = false;

    private void Start()
    {
        //初始化
        if (base.Init != null) base.Init(this);
    }
    /// <summary>
    /// 移入
    /// </summary>
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (base.OnEnter != null) base.OnEnter(this);
    }
    /// <summary>
    /// 移出
    /// </summary>
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (base.OnExit != null) base.OnExit(this);
    }
    /// <summary>
    /// 开始拖拽
    /// </summary>
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (base.OnBeginDrag != null) base.OnBeginDrag(this);
    }
    /// <summary>
    /// 拖拽中
    /// </summary>
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (CheckGameObject())
        {
            var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, targetScreenPoint.z));
            offset = this.transform.position - pos;
        }
        if (isDrag)
        {
            //当前鼠标所在的屏幕坐标
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, targetScreenPoint.z);
            //把当前鼠标的屏幕坐标转换成世界坐标
            Vector3 curWorldPoint = Camera.main.ScreenToWorldPoint(curScreenPoint);
            this.transform.position = curWorldPoint + offset;
            if (base.OnDrag != null) base.OnDrag(this);
        }

    }

    /// <summary>
    /// 拖拽结束
    /// </summary>
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (base.OnEndDrag != null) base.OnEndDrag(this);
    }
    /// <summary>
    /// 移入
    /// </summary>
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (base.OnRightClick != null) base.OnRightClick(this);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            //XXX:双击一定会触发单击
            if (base.OnLeftClick != null) base.OnLeftClick(this);
            if (eventData.clickCount == 2)
            {
                if (base.OnDoubleClick != null) base.OnDoubleClick(this);
            }
        }
    }

    private void OnDestroy()
    {
        //被销毁时
        if (base.OnDestory != null) base.OnDestory(this);
    }
    /// <summary>
    /// 检查是否点击到cbue
    /// </summary>
    /// <returns></returns>
    bool CheckGameObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        //XXX：POI物体的layer层设置为 POI
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("POI")))
        {
            isDrag = true;
            //得到射线碰撞到的物体
            targetScreenPoint = Camera.main.WorldToScreenPoint(this.transform.position);
            return true;
        }
        return false;
    }
}

