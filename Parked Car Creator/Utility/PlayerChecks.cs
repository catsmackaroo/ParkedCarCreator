using CCL.GTAIV;
using IVSDKDotNet;
using System;
using static IVSDKDotNet.Native.Natives;

namespace ParkedCarCreator.NETBasePreset
{
    public class PlayerChecks
    {
        public static bool IsPlayerInOrNearCombat()
        {
            IVPool pedPool = IVPools.GetPedPool();
            for (int i = 0; i < pedPool.Count; i++)
            {
                UIntPtr ptr = pedPool.Get(i);
                if (ptr != UIntPtr.Zero)
                {
                    if (ptr == IVPlayerInfo.FindThePlayerPed())
                        continue;

                    int pedHandle = (int)pedPool.GetIndex(ptr);

                    if (IS_PED_IN_COMBAT(pedHandle))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsPlayerSeenByPolice()
        {
            foreach (var kvp in PedHelper.PedHandles)
            {
                int pedHandle = kvp.Value;
                IVPed pedPed = NativeWorld.GetPedInstanceFromHandle(pedHandle);

                if (pedPed == null || IS_CHAR_DEAD(pedHandle))
                    continue;

                GET_CHAR_MODEL(pedHandle, out uint pedModel);
                GET_CURRENT_BASIC_COP_MODEL(out uint copModel);

                if (pedModel != copModel)
                    continue;

                if (pedPed.CanCharSeeChar(Main.PlayerPed, 50, 130))
                    return true;
            }
            return false;
        }

        private static uint currentHealth = 0;
        private static uint previousHealth = 0;
        private static bool damagedTaken = false;
        public static bool HasPlayerBeenDamagedHealth()
        {

            GET_CHAR_HEALTH(Main.PlayerPed.GetHandle(), out previousHealth);

            if (previousHealth < currentHealth && currentHealth > 0 && previousHealth > 0)
                damagedTaken = true;

            GET_CHAR_HEALTH(Main.PlayerPed.GetHandle(), out currentHealth);

            if (damagedTaken)
            {
                previousHealth = currentHealth;
                damagedTaken = false;
                return true;
            }
            else
                return false;

        }

        private static uint currentArmor = 0;
        private static uint previousArmor = 0;
        private static bool armorDamageTaken = false;
        public static bool HasPlayerBeenDamagedArmor()
        {
            GET_CHAR_ARMOUR(Main.PlayerPed.GetHandle(), out previousArmor);

            if (previousArmor < currentArmor && currentArmor > 0 && previousArmor > 0)
                armorDamageTaken = true;

            GET_CHAR_ARMOUR(Main.PlayerPed.GetHandle(), out currentArmor);

            if (armorDamageTaken)
            {
                previousArmor = currentArmor;
                armorDamageTaken = false;
                return true;
            }
            else
                return false;
        }

        private static DateTime lastShotTime = DateTime.MinValue;
        private static readonly TimeSpan timeBetweenShots = TimeSpan.FromSeconds(1);

        public static bool HasPlayerShotRecently()
        {
            if (IS_CHAR_SHOOTING(Main.PlayerPed.GetHandle()))
            {
                DateTime currentTime = DateTime.Now;

                TimeSpan elapsed = currentTime - lastShotTime;

                if (elapsed <= timeBetweenShots)
                {
                    lastShotTime = currentTime;
                    return true;
                }

                lastShotTime = currentTime;
            }
            return false;
        }

        private static uint previousVehicleHealth = 0;
        private static uint currentVehicleHealth = 0;

        public static (int damageAmount, int damageLevel, float normalizedDamage) GetVehicleDamage()
        {
            int damageAmount = 0;
            int damageLevel = 0;
            float normalizedDamage = 0f;

            GET_CAR_CHAR_IS_USING(Main.PlayerPed.GetHandle(), out int currentVehicle);
            if (currentVehicle == 0)
            {
                // Player is not in a vehicle
                previousVehicleHealth = 0;
                currentVehicleHealth = previousVehicleHealth;
                return (0, 0, 0f);
            }

            GET_CAR_HEALTH(currentVehicle, out uint currentHealth);

            currentVehicleHealth = currentHealth;

            if (previousVehicleHealth > 0 && currentVehicleHealth > 0 && currentVehicleHealth < previousVehicleHealth)
            {
                damageAmount = (int)(previousVehicleHealth - currentVehicleHealth);
                normalizedDamage = CommonHelpers.Clamp((float)damageAmount / 500f, 0f, 1f);
            }

            previousVehicleHealth = currentVehicleHealth;

            if (damageAmount > 450) damageLevel = 4;
            else if (damageAmount > 250) damageLevel = 3;
            else if (damageAmount > 100) damageLevel = 2;
            else if (damageAmount > 0) damageLevel = 1;

            return (damageAmount, damageLevel, normalizedDamage);
        }

    }
}
