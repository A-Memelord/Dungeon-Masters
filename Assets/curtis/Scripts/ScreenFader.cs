using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image image;

    private MenuManager M;

    void Start()
    {
       // StartCoroutine(FadeOut())       
    }

    public void FadeAndLoad(string sceneName, float duration)
    {
      //  StartCoroutine(Fader(sceneName, float duration));
    }

    IEnumerator Fader(string sceneName, float duration)
    {
        float t = 0;
        Color c = image.color;
        while(t< duration)
        {
            //t == Time.deltaTime;
            c.a = t / duration;
            image.color = c;
            yield return null;
        }
    }


    void Update()
    {
   //     if (M.)       
    }
}