using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManger : MonoBehaviour
{
    public void ToGame()
    {
        SoundManager.Instance.PlaySound(0);
        SceneManager.LoadScene("Game");
        SoundManager.Instance.PlaySound(0);
    }
}
