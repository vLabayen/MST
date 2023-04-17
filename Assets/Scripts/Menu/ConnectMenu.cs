using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectMenu : MonoBehaviour
{
    public Text serverAddressText;
    public Button connectButton;

    public delegate void TryConnectToServerDelegate(string serverAddress);
    public event TryConnectToServerDelegate tryConnectToServer;

    public void Setup() {
        connectButton.onClick.AddListener(() => tryConnectToServer?.Invoke(serverAddressText.text));
    }
}
