using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectMenu : MonoBehaviour
{
    public Text serverAddressText;
    public Button connectButton;

    public void Setup(Action<string> connectButtonOnClick) {
        connectButton.onClick.AddListener(() => connectButtonOnClick(serverAddressText.text));
    }
}
