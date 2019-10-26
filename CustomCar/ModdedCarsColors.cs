using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Spectrum.API.Configuration;
using UnityEngine;

namespace CustomCar
{
    public static class ModdedCarsColors
    {
        public static void LoadAll()
        {
            var profileManager = G.Sys.ProfileManager_;

            if (profileManager == null)
                return;

            var settings = new Settings("CustomCars");

            var profiles = profileManager.GetType().GetField("profiles_", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(profileManager) as List<Profile>;

            foreach(var profile in profiles)
            {
                var colors = profile.GetType().GetField("carColorsList_", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(profile) as CarColors[];

                int carInfosLength = G.Sys.ProfileManager_.carInfos_.Length;
                if (carInfosLength > colors.Length)
                {
                    var colors2 = new CarColors[carInfosLength];
                    for (int i = 0; i < colors.Length; i++)
                        colors2[i] = colors[i];
                    colors = colors2;
                }
                
                for(int i = CustomCarsPatchInfos.baseCarCount; i < carInfosLength; i++)
                    LoadCarColors(settings, profile.Name_, i, ref colors[i]);

                profile.GetType().GetField("carColorsList_", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(profile, colors);
            }
        }

        static void LoadCarColors(Settings settings, string profileName, int index, ref CarColors colors)
        {
            string baseKey = profileName + ".." + index;

            LoadColor(settings, ref colors.primary_, baseKey + "..primary_");
            LoadColor(settings, ref colors.secondary_, baseKey + "..secondary_");
            LoadColor(settings, ref colors.glow_, baseKey + "..glow_");
            LoadColor(settings, ref colors.sparkle_, baseKey + "..sparkle_");

        }

        static void LoadColor(Settings settings, ref Color color, string baseKey)
        {
            color.r = settings.GetOrCreate<float>(baseKey + "..r", color.r);
            color.g = settings.GetOrCreate<float>(baseKey + "..g", color.g);
            color.b = settings.GetOrCreate<float>(baseKey + "..b", color.b);
            color.a = settings.GetOrCreate<float>(baseKey + "..a", color.a);
        }

        public static void SaveAll()
        {
            var profileManager = G.Sys.ProfileManager_;

            if (profileManager == null)
                return;

            var settings = new Settings("CustomCars");
            settings.Clear();

            var profiles = profileManager.GetType().GetField("profiles_", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(profileManager) as List<Profile>;

            foreach (var profile in profiles)
            {
                var colors = profile.GetType().GetField("carColorsList_", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(profile) as CarColors[];

                for (int i = CustomCarsPatchInfos.baseCarCount; i < colors.Length; i++)
                    SaveCarsColors(settings, profile.Name_, i, colors[i]);
            }

            settings.Save();
        }

        static void SaveCarsColors(Settings settings, string profileName, int index, CarColors colors)
        {
            string baseKey = profileName + ".." + index;

            SaveColor(settings, colors.primary_, baseKey + "..primary_");
            SaveColor(settings, colors.secondary_, baseKey + "..secondary_");
            SaveColor(settings, colors.glow_, baseKey + "..glow_");
            SaveColor(settings, colors.sparkle_, baseKey + "..sparkle_");
        }

        static void SaveColor(Settings settings, Color color, string baseKey)
        {
            settings.Add(baseKey + "..r", color.r);
            settings.Add(baseKey + "..g", color.g);
            settings.Add(baseKey + "..b", color.b);
            settings.Add(baseKey + "..a", color.a);
        }
    }
}
