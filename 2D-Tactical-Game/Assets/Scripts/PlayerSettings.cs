using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{

    public SpriteRenderer sp;

    public void SetColor(Color newColor)//RBGA
    {
        sp.color = newColor;
    }


}
