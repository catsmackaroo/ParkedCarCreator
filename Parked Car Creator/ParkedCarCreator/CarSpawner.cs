using IVSDKDotNet;
using static IVSDKDotNet.Native.Natives;
using CCL.GTAIV;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ParkedCarCreator
{
    internal class CarSpawner
    {
        private static uint episode;

        public static void IngameStartup()
        {
            episode = GET_CURRENT_EPISODE();
        }

        public static void SpawnCarAtLocation(Manager.CarData carData)
        {
            if (carData.Episode != 0 && carData.Episode != episode)
            {
                Main.Log($"Car {carData.Slot} is for episode {carData.Episode}, current episode is {episode}. Skipping spawn.");
                return;
            }

            Random random = new Random();
            uint modelHash;


            if (carData.ModelHashes.Count > 0 && (carData.ModelNames.Count == 0 || random.Next(2) == 0))
            {
                modelHash = carData.ModelHashes[random.Next(carData.ModelHashes.Count)];
            }
            else
            {
                string selectedModel = carData.ModelNames[random.Next(carData.ModelNames.Count)];
                modelHash = (uint)GET_HASH_KEY(selectedModel);
            }

            var name = GET_DISPLAY_NAME_FROM_VEHICLE_MODEL(modelHash);
            int closestCar = GET_CLOSEST_CAR(carData.Coords, 5f, 0, 70);
            if (closestCar != 0)
            {
                Main.Log($"Car spot is taken. Cannot spawn {name} in slot {carData.Slot}.");
                return;
            }

            IVVehicle vehicle = NativeWorld.SpawnVehicle(modelHash, carData.Coords, out int carHandle, false);
            SET_CAR_ON_GROUND_PROPERLY(carHandle);
            SET_CAR_HEADING(carHandle, carData.Heading);
            SET_VEHICLE_DIRT_LEVEL(carHandle, Main.GenerateRandomNumber(0, 15));
            vehicle.VehicleFlags.NeedsToBeHotWired = true;

            if (carData.Colors != null && carData.Colors.Count >= 2)
            {
                CHANGE_CAR_COLOUR(carHandle, carData.Colors[0], carData.Colors[1]);
                if (carData.Colors.Count >= 4)
                {
                    SET_EXTRA_CAR_COLOURS(carHandle, carData.Colors[2], carData.Colors[3]);
                }
            }
            else
            {
                Main.Log("Car spawned with natural colors.");
                GET_CAR_COLOURS(carHandle, out int primaryColor, out int secondaryColor);
                GET_EXTRA_CAR_COLOURS(carHandle, out int extraColor1, out int extraColor2);
                carData.Colors = new List<int> { primaryColor, secondaryColor, extraColor1, extraColor2 };
            }

            if (carData.Locked)
            {
                LOCK_CAR_DOORS(carHandle, 7);
            }

            if (carData.Extras != null)
            {
                foreach (int extra in carData.Extras)
                {
                    TURN_OFF_VEHICLE_EXTRA(carHandle, extra, false);
                }
            }

            Main.Log($"Spawned car {name} : {carData}");
        }
    }
}
