using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    public TMP_InputField Input;
    public Button Confirm;

    public GameObject Lobby;

    private void Start()
    {
        EventManager.AddListener(EventType.OnLogin, OnLogin);
        Confirm.onClick.AddListener(() =>
        {
            Main.Login(Input.text);
        });
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(EventType.OnLogin, OnLogin);
    }

    private void OnLogin(BaseEventArgs args)
    {
        if (args.Code == 0)
        {
            Instantiate(Lobby, transform.parent);
            Destroy(gameObject);
        }
        Debug.Log(args.Message);
    }
}
