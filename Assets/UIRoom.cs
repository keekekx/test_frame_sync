using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lagame;
using Packages.com.unity.mgobe.Runtime.src;
using Packages.com.unity.mgobe.Runtime.src.SDK;
using TMPro;
using UnityEngine;

public class UIRoom : MonoBehaviour
{
    public TextMeshProUGUI Info;
    public UILobby Lobby;
    public Room room;
    
    [Serializable]
    internal class Frame {
        [SerializeField]
        public int x;
        [SerializeField]
        public int y;
        [SerializeField]
        public string id;

        public override string ToString()
        {
            return $"{id}@{x}:{y}";
        }
    }

    private ulong current_frame_id;
    private ulong start_frame_id;
    private ulong end_frame_id;

    private void Start()
    {
        room.OnJoinRoom = evt =>
        {
            Debug.Log(evt.Data);
        };

        room.OnLeaveRoom = evt =>
        {
            Debug.Log(evt.Data);
        };

        room.OnRecvFromClient = evt =>
        {
            Debug.Log(evt.Data);
        };

        room.OnChangeCustomPlayerStatus = evt =>
        {
            Debug.Log(evt.Data);
        };

        room.onUpdate = r =>
        {
            Debug.Log("房间信息更新");
        };

        room.OnStartFrameSync = evt =>
        {
            Debug.Log("OnStartFrameSync");
        };

        room.OnStopFrameSync = evt =>
        {
            Debug.Log("OnStopFrameSync");
        };

        room.OnRecvFrame = evt =>
        {
            var bst = (RecvFrameBst) evt.Data;
            foreach (var item in bst.Frame.Items)
            {
                var frame = JsonUtility.FromJson<Frame>(item.Data);
                Debug.Log(frame);
            }
            current_frame_id = bst.Frame.Id;
            Debug.Log($"recv :{current_frame_id}");
            if (current_frame_id - start_frame_id > 1)
            {
                end_frame_id = current_frame_id;
                Debug.Log("需要补帧");
                var send = new RequestFramePara()
                {
                    BeginFrameId = (long)start_frame_id,
                    EndFrameId = (long)end_frame_id,
                };
                room.RequestFrame(send, ee =>
                {
                    Debug.Log(ee.Data);
                });
            }
            else
            {
                start_frame_id = current_frame_id;
            }
            
            //Debug.Log($"房间已开始同步，需要补帧>>{room.RoomBroadcast.FrameBroadcastFrameId}");
        };
        room.OnAutoRequestFrameError = evt =>
        {
            room.RetryAutoRequestFrame();
        };

        if (room.RoomInfo.FrameSyncState != FrameSyncState.Start) return;
        Request();
    }

    public void OnInfo()
    {
        var info = room.RoomInfo;
        var full = (int)info.MaxPlayers == info.PlayerList.Count;
        var names = info.PlayerList.Aggregate("", (current, playerInfo) => current + $"name:{playerInfo.Name},id:{playerInfo.Id},state:{playerInfo.CustomPlayerStatus}");
        var frameState = info.FrameSyncState;
        var frameId = room.RoomBroadcast.FrameBroadcastFrameId;
        Info.text = $"Room full:{full}\n {names}\n{frameState}\n{frameId}";
    }

    public void OnReady()
    {
        var recvList = new List<string>();
        foreach (var playerInfo in room.RoomInfo.PlayerList)
        {
            recvList.Add(playerInfo.Id);
        }
        var send = new SendToClientPara()
        {
            Msg = "Hello World",
            RecvType = RecvType.RoomAll,
            RecvPlayerList = recvList,
        };
        room.SendToClient(send, evt =>
        {
            Debug.Log(evt.Code);
        });
    }

    public void OnChangeStatus()
    {
        var send = new ChangeCustomPlayerStatusPara()
        {
            CustomPlayerStatus = 100,
        };   
        room.ChangeCustomPlayerStatus(send, evt =>
        {
            Debug.Log(evt.Code);
        });
    }

    public void OnQuit()
    {
        room.LeaveRoom(evt =>
        {
            Debug.Log(evt.Code);
        });
    }

    public void Begin()
    {
        room.StartFrameSync(evt =>
        {
            Debug.Log(evt.Code);
        });
    }

    public void Stop()
    {
        room.StopFrameSync(evt =>
        {
            Debug.Log(evt.Code);
        });
    }

    public void Send()
    {
        var send = new SendFramePara()
        {
            Data = JsonUtility.ToJson(new Frame()
            {
                id  = Main.openId,
                x   = 1,
                y = 2,
            }) ,
        };
        room.SendFrame(send, evt =>
        {
            Debug.Log(evt.Code);
        });
    }

    public void Rename()
    {
        room.RoomInfo.Name = "换个名字";
    }

    public void Request()
    {
        room.RequestFrame(new RequestFramePara()
        {
            BeginFrameId = 1,
            EndFrameId = 10,
        }, ee =>
        {
            Debug.Log(ee.Data);
        });
    }

    public void AutoRequest()
    {
        room.RetryAutoRequestFrame();
    }
}
