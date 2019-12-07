using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawnerManager : MonoBehaviour
{
    public GameManager GM;
    public GameObject tile;

    public LayerMask layerMask;
    public GameObject[] tilePrefabs;

    private Transform TilePrefab;
    private SpriteRenderer TileSprRen;
    private Collider2D TileCollider;
    private Vector3 mousePos;
    private Color red = new Color(1f, 0f, 0f, 1f);
    private Color white = new Color(1f, 1f, 1f, 1f);

    private void Start() {
        TilePrefab = tile.GetComponent<Transform>();
        TileSprRen = tile.GetComponent<SpriteRenderer>();
        TileCollider = tile.GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        TilePrefab.position = mousePos;
    }

    private void Update() {
        
        if(TileCollider.IsTouchingLayers(layerMask)){
            TileSprRen.color = red;
            return; //can't place tile here
        }else{
            TileSprRen.color = white;
        }

        if(Input.GetButtonDown("Fire1")){
            //Get necessary scripts for Arsenal Check
            PlayerSettings ps = GM.teams[GM.currTeamTurn, GM.currSoldierTurn[GM.currTeamTurn]].GetComponent<PlayerSettings>();
            WeaponController wc = GM.teams[GM.currTeamTurn, GM.currSoldierTurn[GM.currTeamTurn]].GetComponent<WeaponController>();
            
            if(wc != null && ps != null){
                //If no ammo, leave
                if(!ps.HaveAmmo(wc.currWeapon)){
                    return;
                }
                //Else spawn the prefab
                AudioManager.instance.Play("Building_Blocks");
                Instantiate(tilePrefabs[wc.currWeapon - (int)WeaponCodes.Weak_Stone], TilePrefab.position, Quaternion.identity);
                ps.UpdateAmmo(wc.currWeapon, -1); //decrement the ammo on this weapon
            }
        }
    }

}
