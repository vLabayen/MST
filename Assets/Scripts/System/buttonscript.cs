using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonscript : MonoBehaviour
{
    public void Menu(string level)
    {
        SceneManager.LoadScene(level);
    }
}
