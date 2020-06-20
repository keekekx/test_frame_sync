using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SyncBehavior
{
    public Rigidbody rigidbody;
    private Vector3 _logicPosition;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private Vector3 CurrentPosition
    {
        set => transform.position = value;
        get => transform.position;
    }
    public override void OnSyncAwake()
    {
        _logicPosition = CurrentPosition;
    }

    public override void OnSyncStart()
    {
        
    }

    public override void OnSyncUpdate(float dt, int cmd)
    {
        if (cmd != 0)
        {
            _logicPosition += (new Vector3(0, 0, cmd * dt));
            rigidbody.position = _logicPosition;
            Physics.Simulate(dt);
            _logicPosition = rigidbody.position;
        }
    }

    public override void OnSyncDestroy()
    {
        
    }

    public override void OnUpdate(float dt)
    {
        var followSpeed = 1f * dt;
        var dis = Vector3.Distance(CurrentPosition, _logicPosition);
        CurrentPosition = Vector3.Lerp(CurrentPosition, _logicPosition, Math.Min(followSpeed/dis, 1));
    }
}
