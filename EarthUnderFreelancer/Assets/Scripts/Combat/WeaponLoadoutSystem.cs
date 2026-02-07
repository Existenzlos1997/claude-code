using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Combat
{
    /// <summary>
    /// Weapon loadout customization system
    /// Allows players to configure weapon setups
    /// </summary>
    public class WeaponLoadoutSystem : MonoBehaviour
    {
        [System.Serializable]
        public class WeaponSlot
        {
            public string slotName;
            public WeaponType allowedType;
            public Weapon currentWeapon;
            public int maxAmmo;
            public int currentAmmo;
        }
        
        public enum WeaponType
        {
            MachineGun,
            Cannon,
            Rocket,
            Missile,
            Bomb
        }
        
        [System.Serializable]
        public class Weapon
        {
            public string name;
            public WeaponType type;
            public float damage;
            public float fireRate;
            public float range;
            public int ammoPerShot;
        }
        
        [Header("Weapon Slots")]
        [SerializeField] private List<WeaponSlot> weaponSlots = new List<WeaponSlot>();
        
        [Header("Available Weapons")]
        [SerializeField] private List<Weapon> availableWeapons = new List<Weapon>();
        
        private int currentSlotIndex = 0;
        
        private void Start()
        {
            InitializeDefaultLoadout();
        }
        
        private void Update()
        {
            HandleWeaponSwitching();
        }
        
        private void InitializeDefaultLoadout()
        {
            // Create default weapon slots
            weaponSlots.Add(new WeaponSlot
            {
                slotName = "Primary",
                allowedType = WeaponType.MachineGun,
                maxAmmo = 1000
            });
            
            weaponSlots.Add(new WeaponSlot
            {
                slotName = "Secondary",
                allowedType = WeaponType.Missile,
                maxAmmo = 8
            });
            
            weaponSlots.Add(new WeaponSlot
            {
                slotName = "Payload",
                allowedType = WeaponType.Bomb,
                maxAmmo = 4
            });
            
            // Create default weapons
            availableWeapons.Add(new Weapon
            {
                name = ".50 Cal MG",
                type = WeaponType.MachineGun,
                damage = 10f,
                fireRate = 10f,
                range = 800f,
                ammoPerShot = 1
            });
            
            availableWeapons.Add(new Weapon
            {
                name = "AIM-9 Sidewinder",
                type = WeaponType.Missile,
                damage = 100f,
                fireRate = 1f,
                range = 5000f,
                ammoPerShot = 1
            });
            
            // Equip default weapons
            EquipWeapon(0, availableWeapons[0]);
            EquipWeapon(1, availableWeapons[1]);
        }
        
        private void HandleWeaponSwitching()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && weaponSlots.Count > 0)
            {
                currentSlotIndex = 0;
                Debug.Log($"Selected: {weaponSlots[0].slotName}");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && weaponSlots.Count > 1)
            {
                currentSlotIndex = 1;
                Debug.Log($"Selected: {weaponSlots[1].slotName}");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && weaponSlots.Count > 2)
            {
                currentSlotIndex = 2;
                Debug.Log($"Selected: {weaponSlots[2].slotName}");
            }
        }
        
        public void EquipWeapon(int slotIndex, Weapon weapon)
        {
            if (slotIndex < 0 || slotIndex >= weaponSlots.Count) return;
            
            WeaponSlot slot = weaponSlots[slotIndex];
            
            if (weapon.type == slot.allowedType)
            {
                slot.currentWeapon = weapon;
                slot.currentAmmo = slot.maxAmmo;
                Debug.Log($"Equipped {weapon.name} to {slot.slotName}");
            }
            else
            {
                Debug.LogWarning($"Cannot equip {weapon.type} to {slot.slotName} (requires {slot.allowedType})");
            }
        }
        
        public bool Fire()
        {
            if (currentSlotIndex >= weaponSlots.Count) return false;
            
            WeaponSlot slot = weaponSlots[currentSlotIndex];
            if (slot.currentWeapon == null) return false;
            
            if (slot.currentAmmo >= slot.currentWeapon.ammoPerShot)
            {
                slot.currentAmmo -= slot.currentWeapon.ammoPerShot;
                Debug.Log($"Fired {slot.currentWeapon.name} ({slot.currentAmmo} ammo remaining)");
                return true;
            }
            
            Debug.Log("Out of ammo!");
            return false;
        }
        
        public void Reload(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= weaponSlots.Count) return;
            
            WeaponSlot slot = weaponSlots[slotIndex];
            slot.currentAmmo = slot.maxAmmo;
            Debug.Log($"Reloaded {slot.slotName}");
        }
        
        public WeaponSlot GetCurrentWeaponSlot()
        {
            if (currentSlotIndex >= weaponSlots.Count) return null;
            return weaponSlots[currentSlotIndex];
        }
        
        public List<WeaponSlot> GetAllSlots() => weaponSlots;
    }
}
