using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lagame;
using Packages.com.unity.mgobe.Runtime.src;
using Packages.com.unity.mgobe.Runtime.src.SDK;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static string openId;
    // Start is called before the first frame update
    public static void Login(string name)
    {
        var gameInfo = new GameInfoPara {
            // 替换 为控制台上的“游戏ID”
            GameId = "obg-gb9uj4hq",
            // 玩家 openId
            OpenId = name,
            //替换 为控制台上的“游戏Key”
            SecretKey = "50635494ab9c1a40074f1ccf66c260f5d072b831"
        };
        var config = new ConfigPara {
            // 替换 为控制台上的“域名”
            Url = "gb9uj4hq.wxlagame.com",
            ReconnectMaxTimes = 5,
            ReconnectInterval = 1000,
            ResendInterval = 1000,
            ResendTimeout = 10000,
            IsAutoRequestFrame = true,
        };
        
// 初始化监听器 Listener
        Listener.Init (gameInfo, config, (ResponseEvent eve) => {
            if (eve.Code == 0) {
                EventManager.Invoke(EventType.OnLogin, new BaseEventArgs()
                {
                    Code = 0,
                    Message = "初始化成功"
                });
                openId = name;
            } else {
                Debug.LogFormat ("初始化失败: {0}", eve.Code);
                EventManager.Invoke(EventType.OnLogin, new BaseEventArgs()
                {
                    Code = -1,
                    Message = "初始化失败"
                });
            }
// 初始化广播回调事件
        });
    }

    private void OnDestroy()
    {
        if (Listener.IsInited())
        {
            Listener.Clear();
        }
    }
}
