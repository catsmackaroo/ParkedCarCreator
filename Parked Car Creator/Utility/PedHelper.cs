using IVSDKDotNet;
using System;
using System.Collections.Generic;

namespace ParkedCarCreator.NETBasePreset
{
    internal class PedHelper
    {
        public static Dictionary<UIntPtr, int> PedHandles { get; private set; } = new Dictionary<UIntPtr, int>();
        public static Dictionary<UIntPtr, int> VehHandles { get; private set; } = new Dictionary<UIntPtr, int>();

        public static void GrabAllPeds()
        {
            PedHandles.Clear();

            IVPool pedPool = IVPools.GetPedPool();
            for (int i = 0; i < pedPool.Count; i++)
            {
                UIntPtr ptr = pedPool.Get(i);

                if (ptr != UIntPtr.Zero && ptr != Main.PlayerPed.GetUIntPtr())
                {
                    int pedHandle = (int)pedPool.GetIndex(ptr);
                    PedHandles[ptr] = pedHandle;
                }
            }
        }
        public static void GrabAllVehicles()
        {
            VehHandles.Clear();

            IVPool vehPool = IVPools.GetVehiclePool();
            for (int i = 0; i < vehPool.Count; i++)
            {
                UIntPtr ptr = vehPool.Get(i);

                if (ptr != UIntPtr.Zero)
                {
                    int vehHandle = (int)vehPool.GetIndex(ptr);
                    VehHandles[ptr] = vehHandle;
                }
            }
        }
    }
}
