using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScene : MonoBehaviour
{

    public GameObject[] tuts;

    public void nextTut(int tut)
    {
        tuts[tut].SetActive(false);
        tuts[tut+1].SetActive(true);
    }

    public static void LoadGameScene()
    {
        SceneManager.LoadScene(2);
    }
}
