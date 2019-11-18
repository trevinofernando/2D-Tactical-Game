using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public string soundName = "";
    public float delay = 0f;

    void Start()
    {
        StartCoroutine(Timer(delay));
    }

    private IEnumerator Timer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        AudioManager.instance.Play(soundName);
    }

}
