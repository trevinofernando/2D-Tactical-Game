using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWeaponMenu : MonoBehaviour
{
    public GameObject weaponCanvas;
    public GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.gameState == GameManager.GameState.TurnInProgress)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (!weaponCanvas.gameObject.activeInHierarchy)
                {
                    weaponCanvas.SetActive(true);
                }
                else
                {
                    weaponCanvas.SetActive(false);
                }
            }
        }
        else
        {
            weaponCanvas.SetActive(false);
        }
    }
}
