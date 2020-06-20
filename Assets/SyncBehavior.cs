using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SyncBehavior : MonoBehaviour
{
    public abstract void OnSyncAwake();

    public abstract void OnSyncStart();

    public abstract void OnSyncUpdate(float dt);

    public abstract void OnSyncDestroy();
}
