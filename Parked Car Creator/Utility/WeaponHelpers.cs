using IVSDKDotNet;
using static IVSDKDotNet.Native.Natives;
using System;
using CCL.GTAIV;
using System.Numerics;
using System.Collections.Generic;
using IVSDKDotNet.Enums;

namespace ParkedCarCreator.NETBasePreset
{
    public class WeaponHelpers
    {
        public enum WeaponGroup
        {
            SmallPistol = 5,
            HeavyPistol = 6,
            SMG = 8,
            Shotgun = 9,
            AssaultRifle = 10,
            Sniper = 11
        }

        public static int GetWeaponType()
        {
            GET_CURRENT_CHAR_WEAPON(Main.PlayerPed.GetHandle(), out int pWeapon);
            return pWeapon;
        }
        public static IVWeaponInfo GetWeaponInfo()
        {
            return IVWeaponInfo.GetWeaponInfo((uint)GetWeaponType());
        }

        public static WeaponGroup GetWeaponGroup() 
        {
            return (WeaponGroup)GetWeaponInfo().Group;
        }

        public static List<eWeaponType> GetWeaponInventory(bool IncludeMelee)
        {
            List<eWeaponType> inventory = new List<eWeaponType>();

            HashSet<eWeaponType> melee = new HashSet<eWeaponType>
            {
                eWeaponType.WEAPON_UNARMED,
                eWeaponType.WEAPON_BASEBALLBAT,
                eWeaponType.WEAPON_KNIFE,
                eWeaponType.WEAPON_ANYMELEE
            };


            foreach (eWeaponType weapon in Enum.GetValues(typeof(eWeaponType)))
            {
                if (melee.Contains(weapon) && !IncludeMelee)
                    melee.Add(weapon);

                if (HAS_CHAR_GOT_WEAPON(Main.PlayerPed.GetHandle(), (int)weapon) && !melee.Contains(weapon))
                {
                    inventory.Add(weapon);
                }
            }

            return inventory;
        }

        public static void PrintWeaponInventory()
        {
            // Get the player's weapon inventory
            List<eWeaponType> inventory = GetWeaponInventory(true);

            // Print each weapon in the inventory
            Main.Log("Player's Weapon Inventory:");
            foreach (eWeaponType weapon in inventory)
                Main.Log($"- {weapon}");
        }
        public static Dictionary<eWeaponType, int> GetWeaponAmmoCounts()
        {
            Dictionary<eWeaponType, int> ammoCounts = new Dictionary<eWeaponType, int>();

            foreach (eWeaponType weapon in GetWeaponInventory(false))
            {
                GET_AMMO_IN_CHAR_WEAPON(Main.PlayerPed.GetHandle(), (int)weapon, out int ammo);
                ammoCounts[weapon] = ammo;
            }

            return ammoCounts;
        }
        public static int GetSpecificWeaponAmmo(eWeaponType eWeaponType)
        {
            Dictionary<eWeaponType, int> ammoCounts = GetWeaponAmmoCounts();
            if (ammoCounts.ContainsKey(eWeaponType))
                return ammoCounts[eWeaponType];
            else
                return 0;
        }
        public static bool IsReloading()
        {
            GET_CURRENT_CHAR_WEAPON(Main.PlayerPed.GetHandle(), out int currentWeapon);
            GET_AMMO_IN_CHAR_WEAPON(Main.PlayerPed.GetHandle(), currentWeapon, out int weaponAmmo);
            GET_AMMO_IN_CLIP(Main.PlayerPed.GetHandle(), currentWeapon, out int clipAmmo);
            GET_MAX_AMMO_IN_CLIP(Main.PlayerPed.GetHandle(), currentWeapon, out int clipAmmoMax);

            if (clipAmmo < clipAmmoMax && weaponAmmo - clipAmmo > 0)
                return true;
            else
                return false;
        }
    }
}
