using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mx.UI;
using Mx.Msg;
using Mx.Util;
using UnityEngine.UI;

/// <summary> 没有网络提醒UI窗口 </summary>
public class NoNetworkUIForm : BaseUIForm
{
    private void Awake()
    {
        RigisterAllButtonObjectEvent(OnClickButton);
        MessageMgr.AddMsgListener("NoNetworkUIFormMsg",OnUIFormMessagesEvent);
    }

    private void OnDestroy()
    {
        MessageMgr.RemoveMsgListener("NoNetworkUIFormMsg", OnUIFormMessagesEvent);
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

