using System;
using System.Collections;
using System.Collections.Generic;
using Lagame;
using Packages.com.unity.mgobe.Runtime.src;
using Packages.com.unity.mgobe.Runtime.src.SDK;
using UnityEngine;

public class UILobby : MonoBehaviour
{
    public GameObject Room;

    private Room room;
    
    public void Start()
    {
        EventManager.AddListener(EventType.OnMatch, OnMatch);
        //查询玩家自己的房间
        room = new Room (null);
        Listener.Add(room);
        room.GetRoomDetail ((ResponseEvent evt) => {
            if (evt.Code != 0 && evt.Code != 20011) {
                Debug.Log ("初始化失败");
            }
            // Type type = e.data.GetType();
            // Debug.LogFormat ("查询成功: {0}", type);
            Debug.Log ("查询成功");
            if (evt.Code == 20011) {
                Debug.Log ("玩家不在房间内");
            } else {
                // 玩家已在房间内
                var res = (GetRoomByRoomIdRsp) evt.Data;
                Debug.LogFormat ("房间名 {0}", res.RoomInfo.Name);

                EventManager.Invoke(EventType.OnMatch, new BaseEventArgs(){Code = evt.Code});
            }
        });
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(EventType.OnMatch, OnMatch);
    }

    public void Match()
    {
        var matchRoomPara = new MatchRoomPara(){
            PlayerInfo = new PlayerInfoPara()
            {
                Name = Main.openId,
                CustomProfile = "nil.png",
                CustomPlayerStatus = 1,
            },
            MaxPlayers = 2,
            RoomType = "1",
        };
        
        room.MatchRoom(matchRoomPara, evt => {
            EventManager.Invoke(EventType.OnMatch, new BaseEventArgs(){Code = evt.Code});
        });
    }

    private void OnMatch(BaseEventArgs args)
    {
        if (args.Code != 0) {
            Debug.LogFormat("匹配失败, {0}", args.Code);
        }
        else
        {
            var go = Instantiate(Room, transform.parent);
            var r = go.GetComponent<UIRoom>();
            r.room = room;
            Destroy(gameObject);
        }
    }
}
