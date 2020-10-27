using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mx.UI;
using Mx.Msg;
using Mx.Util;
using UnityEngine.UI;

/// <summary> 没有相册权限提醒UI窗口 </summary>
public class PhotoAlbumPermissionUIForm : BaseUIForm
{
    private void Awake()
    {
        RigisterAllButtonObjectEvent(OnClickButton);
        MessageMgr.AddMsgListener("PhotoAlbumPermissionUIFormMsg",OnUIFormMessagesEvent);
    }

    private void OnDestroy()
    {
        MessageMgr.RemoveMsgListener("PhotoAlbumPermissionUIFormMsg", OnUIFormMessagesEvent);
    }

    private void OnClickButton(GameObject click)
    {
        switch(click.name)
        {
            case "BtnClose": CloseUIForm(); break;
        }
    }

    private void OnUIFormMessagesEvent(string key, object values)
    {
        
    }
}

