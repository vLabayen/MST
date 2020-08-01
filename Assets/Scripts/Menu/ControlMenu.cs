using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ControlMenu : MonoBehaviour
{
    public static ControlMenu singleton { get; private set; }

    public void Awake() {
        if (ControlMenu.singleton == null) {
            ControlMenu.singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene("Menu");
    }
}
