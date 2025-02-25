using CCL.GTAIV;
using IVSDKDotNet;
using System;
using System.Collections.Generic;
using System.Numerics;
using static IVSDKDotNet.Native.Natives;

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
            if (carData == null)
            {
                Main.Log("Car data is null. Cannot spawn car.");
                return;
            }

            if (carData.Episode != 0 && carData.Episode != episode)
            {
                Main.Log($"Car {carData.Slot} is for episode {carData.Episode}, current episode is {episode}. Skipping spawn.");
                return;
            }

            var modelHash = GetModelHash(carData);
            var name = GET_DISPLAY_NAME_FROM_VEHICLE_MODEL(modelHash);

            if (IsCarSpotTaken(carData.Coords))
            {
                Main.Log($"Car spot is taken. Cannot spawn {name} in slot {carData.Slot}.");
                return;
            }

            var vehicle = SpawnVehicle(modelHash, carData.Coords, carData.Heading);
            ConfigureVehicle(vehicle, carData);

            Main.Log($"Spawned car {name} : {carData}");
        }

        private static uint GetModelHash(Manager.CarData carData)
        {
            var random = new Random();
            if (carData.ModelHashes.Count > 0 && (carData.ModelNames.Count == 0 || random.Next(2) == 0))
            {
                return carData.ModelHashes[random.Next(carData.ModelHashes.Count)];
            }
            else
            {
                var selectedModel = carData.ModelNames[random.Next(carData.ModelNames.Count)];

                if (selectedModel.Equals("random", StringComparison.OrdinalIgnoreCase))
                {
                    GET_RANDOM_CAR_MODEL_IN_MEMORY(true, out uint hash, out int errorId);
                    return hash;
                }
                return (uint)GET_HASH_KEY(selectedModel);
            }
        }

        private static bool IsCarSpotTaken(Vector3 coords)
        {
            var closestCar = GET_CLOSEST_CAR(coords, 5f, 0, 70);
            return closestCar != 0;
        }

        private static IVVehicle SpawnVehicle(uint modelHash, Vector3 coords, int heading)
        {
            var vehicle = NativeWorld.SpawnVehicle(modelHash, coords, out int carHandle, false);
            SET_CAR_ON_GROUND_PROPERLY(carHandle);
            SET_CAR_HEADING(carHandle, heading);
            SET_VEHICLE_DIRT_LEVEL(carHandle, Main.GenerateRandomNumber(0, 15));
            vehicle.VehicleFlags.NeedsToBeHotWired = true;
            return vehicle;
        }

        private static void ConfigureVehicle(IVVehicle vehicle, Manager.CarData carData)
        {
            var carHandle = vehicle.GetHandle();

            if (carHandle == 0)
                return;

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
                Main.Log($"Car {carData.Slot} spawned with natural colors.");
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
                foreach (var extra in carData.Extras)
                {
                    TURN_OFF_VEHICLE_EXTRA(carHandle, extra, false);
                }
            }

            if (carData.Sirens)
            {
                SWITCH_CAR_SIREN(carHandle, true);
            }
        }
    }
}
