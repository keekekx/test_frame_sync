using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SyncBehavior : MonoBehaviour
{
    /// <summary>
    /// 玩家ID，用于获取CMD
    /// </summary>
    public string PlayerId;
    public abstract void OnSyncAwake();

    public abstract void OnSyncStart();

    public abstract void OnSyncUpdate(float dt, int cmd);
    
    public abstract void OnUpdate(float dt);

    public abstract void OnSyncDestroy();
}
