using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class WeaponSystem : MonoBehaviour
{
    //private PlayerManagerSystem managerSystem;
    private PhotonView view;

    private PlayerInputSystem controls;
    [SerializeField] SpriteRenderer weaponRender;
    [SerializeField] SpriteRenderer handRender;
    public TeamIdentity identity;
    public WeaponBlueprint blueprint;
    playerGravityControl gravity;

    public List<GameObject> weaponLibary = new List<GameObject>();

    [SerializeField] private GameObject cursor;
    [SerializeField] private Text currentMagText;
    [SerializeField] private Text backMagText;
    [SerializeField] private Slider ammoSlider;


    private void Start()
    {
        view = GetComponentInParent<PhotonView>();
        controls = GetComponentInParent<PlayerInputSystem>();
        gravity = GetComponentInParent<playerGravityControl>();

        //managerSystem = gravity.gameObject.GetComponentInParent<PlayerManagerSystem>();
        disableWeapons(0);
    }

    public void disableWeapons(int setWeaponIndex)
    {
        for (int i = 0; i < weaponLibary.Count; i++)
        {
            if(i != setWeaponIndex)
                weaponLibary[i].SetActive(false);
            else
            {
                weaponLibary[i].SetActive(true);
                blueprint = weaponLibary[i].GetComponent<WeaponBlueprint>();

                blueprint.currentMagazine = blueprint.magazine;
                blueprint.currentReserves = blueprint.reserves;
            }
        }
    }

    private void Update()
    {
        if (!view.IsMine || blueprint == null)
            return;

        cursor = GetComponent<CursorSystem>().cursorOBJ;

        if (cursor == null)
            return;

        Vector3 mousePos = cursor.transform.position;
        weaponRender = blueprint.weaponRender;
        handRender = blueprint.handRender;
        //render.sprite = blueprint.WeaponVariants[managerSystem.PlayerTeam];
        //handRender.color = managerSystem.manager.teamColorArray[managerSystem.PlayerTeam];

        Vector3 aimDirection = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);
        FlipGun(angle);

        currentMagText.text = blueprint.currentMagazine.ToString("F0");
        backMagText.text = blueprint.currentReserves.ToString("F0");

        ammoSlider.maxValue = blueprint.magazine;
        //if (blueprint.currentMagazine <= 0)
            ammoSlider.value = blueprint.currentMagazine;
        //else
            //ammoSlider.value += blueprint.reloadTime * Time.deltaTime;
    }

    private void FlipGun(float angle)
    {
        //if (angle > 90 || angle < -90)
            //weaponRender.flipY = true;
        //else
            //weaponRender.flipY = false;

        switch (gravity.chosenDirection)
        {
            case 0:
                if (angle > 90 || angle < -90)
                {
                    //weaponRender.flipY = true;
                    //handRender.flipY = true;

                    Quaternion rotation = Quaternion.Euler(180, 0, 0);
                    blueprint.gameObject.transform.localRotation = rotation;
                }
                else
                {
                    //weaponRender.flipY = false;
                    //handRender.flipY = false;

                    Quaternion rotation = Quaternion.Euler(0, 0, 0);
                    blueprint.gameObject.transform.localRotation = rotation;
                }

                break;

            case 1:
                if (angle > 0 || angle < -180)
                {
                    //weaponRender.flipY = true;
                    //handRender.flipY = true;

                    Quaternion rotation = Quaternion.Euler(180, 0, 0);
                    blueprint.gameObject.transform.localRotation = rotation;
                }
                else
                {
                    //weaponRender.flipY = false;
                    //handRender.flipY = false;

                    Quaternion rotation = Quaternion.Euler(0, 0, 0);
                    blueprint.gameObject.transform.localRotation = rotation;
                }

                break;

            case 2:
                if (angle > 90 || angle < -90)
                {
                    // weaponRender.flipY = false;
                    //handRender.flipY = false;

                    Quaternion rotation = Quaternion.Euler(0, 0, 0);
                    blueprint.gameObject.transform.localRotation = rotation;
                }
                else
                {
                    //weaponRender.flipY = true;
                    //handRender.flipY = true;

                    Quaternion rotation = Quaternion.Euler(180, 0, 0);
                    blueprint.gameObject.transform.localRotation = rotation;
                }

                break;

            case 3:
                if (angle > 0 || angle < -180)
                {
                    //weaponRender.flipY = false;
                    //handRender.flipY = false;

                    Quaternion rotation = Quaternion.Euler(0, 0, 0);
                    blueprint.gameObject.transform.localRotation = rotation;
                }
                else
                {
                    //weaponRender.flipY = true;
                    //handRender.flipY = true;

                    Quaternion rotation = Quaternion.Euler(180, 0, 0);
                    blueprint.gameObject.transform.localRotation = rotation;
                }

                break;
        }
    }
}
