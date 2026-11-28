using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerShoot : MonoBehaviour
{
    // [SerializeField] private GameObject bulletPrefab;
    // [SerializeField] private Transform firePoint;
    // [SerializeField] private float fireRate = 5f;

    // // spread settings
    // [SerializeField] private int pelletsPerShot = 1;    // controls amount of projectiles per shot
    // [SerializeField] private float spreadAngle = 0f;    // controls total angle across spread of projectiles in degrees

    // // ammo settings
    // [SerializeField] private int magSize = 30;
    // [SerializeField] private int reserveAmmo = 120;
    // [SerializeField] private float reloadTime = 1.5f;

    // private int currentAmmo;
    // private bool isReloading = false;
    // private float nextFireTime = 0f;

    [SerializeField] private Transform firePoint;
    [SerializeField] private WeaponData[] weapons;

    private int currentWeaponIndex = 0;
    private bool isReloading = false;
    private float nextFireTime = 0f;

    private WeaponData currentWeapon => weapons[currentWeaponIndex];

    private void Start()
    {
        if (weapons == null || weapons.Length == 0)
            return;

        foreach (WeaponData weapon in weapons)
        {
            weapon.currentAmmo = weapon.magSize;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Mouse.current == null || Keyboard.current == null)
            return;

        if (weapons == null || weapons.Length == 0)
            return;

        if (isReloading)
            return;

        // weapon switching
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            SwitchWeapon(0);

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            SwitchWeapon(1);

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            SwitchWeapon(2);

        // manual reload
        if (Keyboard.current.rKey.wasPressedThisFrame && currentWeapon.currentAmmo < currentWeapon.magSize && currentWeapon.reserveAmmo > 0)
        {
            StartCoroutine(Reload());
            return;
        }

        // shooting
        if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime && currentWeapon.currentAmmo > 0)
        {
            Shoot();
            nextFireTime = Time.time + 1f / currentWeapon.fireRate;
        }
    }

    private void SwitchWeapon(int weaponIndex)
    {
        // makes sure weapons actually exist
        if (weaponIndex < 0 || weaponIndex >= weapons.Length)
            return;

        // if already equipped, no switching
        if (weaponIndex == currentWeaponIndex)
            return;

        currentWeaponIndex = weaponIndex;

        Debug.Log("Equipped: " + currentWeapon.weaponName);
        Debug.Log("Ammo: " + currentWeapon.currentAmmo + " / " + currentWeapon.reserveAmmo);
    }


    // if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime)
    // {
    //     Shoot();
    //     nextFireTime = Time.time + 1f / fireRate;
    // }


    private void Shoot()
    {

        for (int i = 0; i < currentWeapon.pelletsPerShot; i++)
        {
            float angle = 0f;

            if (currentWeapon.pelletsPerShot > 1)
            {
                angle = -currentWeapon.spreadAngle / 2f + (currentWeapon.spreadAngle / (currentWeapon.pelletsPerShot - 1)) * i;
                // divides spread equally
            }

            Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(0f, angle, 0f);   // rotates each bullet horizontally along y-axis

            Instantiate(currentWeapon.bulletPrefab, firePoint.position, bulletRotation);
        }

        currentWeapon.currentAmmo--;

        Debug.Log(currentWeapon.weaponName + " Ammo: " + currentWeapon.currentAmmo + " / " + currentWeapon.reserveAmmo);
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        Debug.Log("Reloading " + currentWeapon.weaponName + "...");

        yield return new WaitForSeconds(currentWeapon.reloadTime);

        int ammoNeeded = currentWeapon.magSize - currentWeapon.currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, currentWeapon.reserveAmmo);


        currentWeapon.currentAmmo += ammoToReload;
        currentWeapon.reserveAmmo -= ammoToReload;

        isReloading = false;

        Debug.Log("Reload complete! " + currentWeapon.weaponName + " Ammo: " + currentWeapon.currentAmmo + " / " + currentWeapon.reserveAmmo);
    }


}
