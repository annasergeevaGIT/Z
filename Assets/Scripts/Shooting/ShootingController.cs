using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingController : MonoBehaviour
{
    public Animator animator;
    public Transform firePoint;
    public float fireRate = 0.1f;
    public float fireRange = 10f;
    private float nextFireTime = 0f;
    public bool isAuto = false;
    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 1.5f;
    private bool isReloading = false;
    public ParticleSystem muzzleFlash;
    public ParticleSystem bloodEffect;
    public int damagePerShot = 10;

    [Header("Sound Effects")]
    public AudioSource soundAudioSource;
    public AudioClip shootingSoundClip;
    public AudioClip reloadSoundClip;

    //public Text currentAmmoText;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {

        //currentAmmoText.text = currentAmmo.ToString();

        if(isReloading)
            return;

        if(isAuto == true)
        {
            if(Input.GetButton("Fire1") && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Shoot();
            }
            else
            {
                animator.SetBool("Shoot", false);
            }
        }
        else
        {
            if(Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Shoot();
            }
            else
            {
                animator.SetBool("Shoot", false);
            }
        }

        if(Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            Reload();
        }
    }
    private void Shoot()
    {
        if(currentAmmo > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, fireRange))
            {
                Debug.Log(hit.transform.name);

                //apply damage to zombie
                ZombieAI zombieAI = hit.collider.GetComponent<ZombieAI>();
                if (zombieAI != null)
                {
                    zombieAI.TakeDamage(damagePerShot);

                    // Play blood effect particle system at the hit point.
                    ParticleSystem blood = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(blood.gameObject, blood.main.duration);
                }
                
                /*
            WaypointZombieAI waypointzombieAI = hit.collider.GetComponent<WaypointZombieAI>();
                if(waypointzombieAI != null)
                {
                    waypointzombieAI.TakeDamage(damagePerShot);

                    // Play blood effect particle system at the hit point.
                    ParticleSystem blood = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(blood.gameObject, blood.main.duration);
                }*/
            }

            muzzleFlash.Play();
            animator.SetBool("Shoot", true);
            currentAmmo--;

            soundAudioSource.PlayOneShot(shootingSoundClip);
        }
        else
        {
            //Reload
            Reload();
        }
    }

    private void Reload()
    {
        if(!isReloading && currentAmmo < maxAmmo)
        {
            animator.SetTrigger("Reload");
            isReloading = true;
            //play reload sound
            soundAudioSource.PlayOneShot(reloadSoundClip);
            Invoke("FinishReloading", reloadTime);
        }
    }

    private void FinishReloading()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        animator.ResetTrigger("Reload");
    }
}
