using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class WeaponBlueprint : MonoBehaviour
{
    private PlayerHealthSystem healthSystem;
    //public Sprite[] WeaponVariants = new Sprite[8];
    private TeamIdentity identity;
    public int TeamIndex;

    public SpriteRenderer weaponRender;
    public SpriteRenderer handRender;

    [SerializeField] private UnityEvent fireEventPhase1;
    [SerializeField] private UnityEvent fireEventPhase2;
    [SerializeField] private UnityEvent fireEventPhase3;

    [Header("General Weapon Systems")]
    public GameObject projectile;
    public Transform firePoint;
    public float fireForce = 20;

    [SerializeField] private bool InfiniteAmmo = false;
    public int magazine;
    public int reserves;
    public float reloadTime;

    public float fireMean;
    public float range;

    [HideInInspector] public int currentMagazine;
    [HideInInspector] public int currentReserves;
    [HideInInspector] public float currentReloadTime;

    [HideInInspector] protected bool reloadingAmmo = false;

    public int weaponALT;

    [Header("Weapon SFX")]
    public AudioSource source;
    public AudioClip weaponFireSFX;
    public AudioClip weaponReloadSFX;

    [Header("Weapon Anim")]
    public Animator weaponAnim;
    public int weaponFeedIcon;

    private void Awake()
    {
        Invoke("setWeapon", 0.1f);
    }

    public void setWeapon()
    {
        healthSystem = GetComponentInParent<PlayerHealthSystem>();
        identity = GetComponentInParent<WeaponSystem>().identity;

        currentMagazine = magazine;
        currentReserves = reserves;
    }

    private void Update()
    {
        if (identity == null)
            return;

        handRender.color = identity.playerTeamColor;
        TeamIndex = identity.playerTeamIndex;
    }

    public void fireWeapon(int index)
    {
        if (healthSystem.dead)
            return;

        switch(index)
        {
            case 1:
                fireEventPhase1.Invoke();
                break;
            case 2:
                fireEventPhase2.Invoke();
                break;
            case 3:
                fireEventPhase3.Invoke();
                break;
        }

    }
    public void reloadWeapon()
    {
        if (reloadingAmmo || healthSystem.dead)
            return;

        StartCoroutine(reloading());
    }
    private IEnumerator reloading()
    {
        if(InfiniteAmmo)
        {
            currentMagazine = magazine;
            yield return 0;
        }

        source.clip = weaponReloadSFX;
        source.Play();

        reloadingAmmo = true;
        yield return new WaitForSeconds(reloadTime);
        int newAmount = magazine - currentMagazine;

        if (currentReserves >= newAmount)
        {
            currentMagazine += newAmount;
            currentReserves -= newAmount;
        }
        else
        {
            currentMagazine += currentReserves;
            currentReserves -= 0;
        }

        reloadingAmmo = false;
    }

    public void altWeaponMode()
    {
        if (weaponALT <= 0)
            weaponALT++;
        else
            weaponALT = 0;

        Debug.Log(weaponALT);
    }
}
