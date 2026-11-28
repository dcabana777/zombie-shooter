using UnityEngine;

// allows display of weapons field in inspector
[System.Serializable]

public class WeaponData
{
    public string weaponName;   // identifies weapon

    // shooting settings
    public GameObject bulletPrefab; // allows different weapons to use different projectile prefabs
    public float fireRate = 5f;

    // spread settings
    public int pelletsPerShot = 1;
    public float spreadAngle = 0f;

    // ammo settings
    public int magSize = 30;
    public int reserveAmmo = 120;
    public float reloadTime = 1.5f;

    // runtime settings
    // prevents unity from serializing the runtime ammo count
    [System.NonSerialized] public int currentAmmo;

}
