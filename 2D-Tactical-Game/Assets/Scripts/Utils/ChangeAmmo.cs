using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeAmmo : MonoBehaviour
{
    public WeaponCodes weapon;
    public TextMeshProUGUI ammo;

    private GlobalVariables GLOBALS;

    private void Start() {
        GLOBALS = GlobalVariables.Instance;
        UpdateAmmoCount();
    }

    public void ChangeAmmoBy(int increment)
    {
        GLOBALS.arsenalAmmo[(int)weapon] += increment;
        GLOBALS.arsenalAmmo[(int)weapon] = Mathf.Clamp(GLOBALS.arsenalAmmo[(int)weapon], 0, 99);
        UpdateAmmoCount();
    }

    public void UpdateAmmoCount(){
        int ammoCount = GLOBALS.arsenalAmmo[(int)weapon];
        ammo.SetText(ammoCount.ToString("00"));
    }
}
