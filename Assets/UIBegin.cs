using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class UIBegin : MonoBehaviour
{
    public UILogin Login;
    public Player Player;

    public void Local()
    {
        Destroy(gameObject);
        GameDirector.Instance.Init("test");
        GameDirector.Instance.CreateSyncObject(Player);
    }

    public void Remote()
    {
        Instantiate(Login, transform.parent, false);
        Destroy(gameObject);
    }
}
