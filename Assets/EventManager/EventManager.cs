using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    #region 初始化以及单例模式

    private struct InvokeData
    {
        public EventType eventType;
        public BaseEventArgs args;
    }
    
    private static EventManager _instance;

    private static EventManager Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = FindObjectOfType<EventManager>();
            if (_instance != null) return _instance;
            var _ = new GameObject("EventManager", typeof(EventManager));
            return _instance;
        }
    }

    private Dictionary<EventType, List<Action<BaseEventArgs>>> EventDict = new Dictionary<EventType, List<Action<BaseEventArgs>>>();
    private Queue<InvokeData> InvokeQueue = new Queue<InvokeData>();
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        
        if (_instance == this)
        {
            return;
        }
        Destroy(gameObject);
    }

    private void Update()
    {
        while (InvokeQueue.Count > 0)
        {
            var data = InvokeQueue.Dequeue();
            if (!EventDict.TryGetValue(data.eventType, out var actions)) continue;
            foreach (var action in actions)
            {
                action(data.args);
            }
        }
    }

    private void OnDestroy()
    {
        EventDict.Clear();
        InvokeQueue.Clear();
    }

    #endregion

    public static void AddListener(EventType eventType, Action<BaseEventArgs> action)
    {
        if (!Instance.EventDict.TryGetValue(eventType, out var actions))
        {
            actions = new List<Action<BaseEventArgs>>();
            Instance.EventDict.Add(eventType, actions);
        }
        actions.Add(action);
    }

    public static void RemoveListener(EventType eventType, Action<BaseEventArgs> action)
    {
        if (Instance.EventDict.TryGetValue(eventType, out var actions))
        {
            actions.Remove(action);
        }
    }

    public static void RemoveAllListener(EventType eventType)
    {
        if (Instance.EventDict.TryGetValue(eventType, out var actions))
        {
            actions.Clear();
        }
    }

    public static void Invoke(EventType eventType, BaseEventArgs args)
    {
        Instance.InvokeQueue.Enqueue(new InvokeData()
        {
            eventType =  eventType,
            args = args,
        });
    }
}
