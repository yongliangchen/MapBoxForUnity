using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapBoxTest : MonoBehaviour
{
    public Transform button;
    public Camera UICamera;
    public RectTransform panel;
    private void Start()
    {
        GameObject objDrag = null;
        var self = button.gameObject.AddComponent<EventTrigger>();
        self.triggers = new List<EventTrigger.Entry>(3)  {
                        new EventTrigger.Entry()
                        {
                            eventID = EventTriggerType.BeginDrag,
                            callback = new EventTrigger.TriggerEvent()
                        },
                        new EventTrigger.Entry()
                        {
                            eventID = EventTriggerType.Drag,
                            callback = new EventTrigger.TriggerEvent()
                         },
                        new EventTrigger.Entry()
                        {
                            eventID = EventTriggerType.EndDrag,
                            callback = new EventTrigger.TriggerEvent()
                         }
                };


        //开始拖拽
        self.triggers[0].callback.AddListener((d) =>
        {
            objDrag = Instantiate(self.gameObject, self.transform.parent);
            objDrag.GetComponent<Image>().raycastTarget = false;
        });
        //拖拽中
        self.triggers[1].callback.AddListener((d) =>
        {
            if (objDrag != null)
            {
                Vector3 pos;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(panel, Input.mousePosition, UICamera, out pos))
                {
                    objDrag.transform.position = pos;
                }
            }

        });
        //拖拽结束
        self.triggers[2].callback.AddListener((d) =>
        {
            double lng, lat;

            Vector2 pos = Input.mousePosition;
            MapHelper.instance.GetLatlng(pos, out lng, out lat);
            Marker m = new Marker()
            {
                uuid = Guid.NewGuid().ToString(),
                name = "测试",
                latitude = lat,
                longitude = lng,
            };
            Destroy(objDrag, 0.1f);
            Debug.Log(lng + ":" + lat);
            MapBoxManager.instance.CreateMarker(m);
        });
    }
}

