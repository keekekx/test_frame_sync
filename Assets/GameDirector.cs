using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    private static GameDirector _instance;

    public static GameDirector Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = FindObjectOfType(typeof(GameDirector)) as GameDirector;
            if (_instance == null)
            {
                var go = new GameObject("GameDirector");
                _instance = go.AddComponent<GameDirector>();
            }
            return _instance;
        }
    }
    
    private List<SyncBehavior> _syncObjects;
    private float _frameTime;
    private string _playerId;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init(string playerId, bool localMode = true,int rate = 15)
    {
        Physics.autoSimulation = false;
        Physics.autoSyncTransforms = false;
        _playerId = playerId;
        _frameTime = 1f / 15;
        _syncObjects = new List<SyncBehavior>();
        if (localMode)
        {
            Debug.Log("本地模式");
            InvokeRepeating(nameof(LocalLoop), 0, _frameTime);
        }
        else
        {
            Debug.Log("远程模式");
        }
    }

    private void LocalLoop()
    {
        LogicUpdate(PackCmd());
    }
    
    public SyncBehavior CreateSyncObject(SyncBehavior obj, Transform parent = null)
    {
        var go = Instantiate(obj, parent, true);
        _syncObjects.Add(go);
        go.OnSyncAwake();
        go.OnSyncStart();
        return go;
    }

    public void DestroySyncObject(SyncBehavior obj)
    {
        obj.OnSyncDestroy();
        _syncObjects.Remove(obj);
    }
    
    public void LogicUpdate(int cmd)
    {
        foreach (var syncObject in _syncObjects)
        {
            syncObject.OnSyncUpdate(_frameTime, cmd);
        }
    }

    public void Clear()
    {
        _syncObjects = null;
    }

    private int PackCmd()
    {
        return (int)Input.GetAxisRaw("Vertical");
    }
    
    private void Update()
    {
        var dt = Time.deltaTime;
        foreach (var syncObject in _syncObjects)
        {
            syncObject.OnUpdate(dt);
        }
    }
}
