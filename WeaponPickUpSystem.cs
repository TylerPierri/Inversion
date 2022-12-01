using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class WeaponPickUpSystem : MonoBehaviourPunCallbacks
{
    private PhotonView view;

    [SerializeField] private int weaponIndex;
    private bool usable = false;
    private SpawnWeaponManager manager;
    private int weaponSpawnIndex;

    private float respawnTime;
    private float timer;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        manager = GameObject.Find("Game Manager").GetComponent<SpawnWeaponManager>();

        for (int i = 0; i < manager.weaponSpawns.Count; i++)
        {
            if (manager.weaponSpawns[i].weaponLocation == gameObject.transform)
            {
                weaponSpawnIndex = i;
                break;
            }
        }

        if (manager.weaponSpawns[weaponSpawnIndex].startWeapon)
        {
            usable = true;
            GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            usable = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }

        respawnTime = manager.weaponSpawns[weaponSpawnIndex].respawnTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && usable)
        {
            collision.GetComponentInChildren<WeaponSystem>().disableWeapons(weaponIndex);

            usable = false;
            timer = respawnTime;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (usable)
            return;

        if (timer >= 0)
            timer -= Time.deltaTime;
        else
        {
            usable = true;
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    
    //old Code
    //string previousWeapon = string.Empty;

    //if (collision.transform.GetChild(0).childCount != 0)
    //previousWeapon = collision.GetComponentInChildren<WeaponSystem>().blueprint.gameObject.name;

    //GameObject prefab = PhotonNetwork.Instantiate(weaponPrefab.name, collision.transform.GetChild(0).position, Quaternion.identity);
    /*
    Debug.Log(collision);
    collision.GetComponentInChildren<WeaponSystem>().blueprint = prefab.GetComponent<WeaponBlueprint>();


    if (collision.transform.GetChild(0).childCount != 0)
    {
        for (int i = 0; i < collision.transform.GetChild(0).childCount; i++)
        {
            Debug.Log(collision.transform.GetChild(0).GetChild(i).gameObject);
            PhotonNetwork.Destroy(collision.transform.GetChild(0).GetChild(i).gameObject);
        }
    }

    prefab.transform.SetParent(collision.transform.GetChild(0));
    prefab.transform.localPosition = weaponPrefab.transform.localPosition;
    prefab.transform.localRotation = weaponPrefab.transform.localRotation;
    prefab.GetComponent<WeaponBlueprint>().setWeapon();
    */
    //gameObject.SetActive(false);

    //PhotonView playerView = collision.GetComponent<PhotonView>();


    //view.TransferOwnership(playerView.OwnerActorNr);

    //Debug.Log(previousWeapon);
    /*
    if(previousWeapon != string.Empty)
    {
        string weaponName = previousWeapon;
        weaponName = weaponName.Substring(0, weaponName.Length - 7);

        PhotonNetwork.Instantiate(weaponName + "_Pickup", transform.position + new Vector3(0,3,0), Quaternion.identity);
    }*/

    //PhotonNetwork.Destroy(GetComponent<PhotonView>());
    //view.RPC("RPC_DestroyObject", RpcTarget.MasterClient);

    //transform.position = new Vector3(0, 0, 10);



}
