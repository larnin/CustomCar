using Harmony;
using System;
using System.Reflection;

namespace CustomCar
{
    //[HarmonyPatch(typeof(Profile), "GetColorsForIndex")]
    //internal class ProfileGetColorsForIndex
    //{
    //    static bool Prefix(Profile __instance, ref int index)
    //    {
    //        try
    //        {
    //            var carColorsList = (CarColors[])__instance.GetType().GetField("carColorsList_", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);

    //            if (index >= carColorsList.Length)
    //                index = 0;
    //        }
    //        catch (Exception e)
    //        {
    //            Console.Out.WriteLine(e.ToString());
    //        }

    //        return true;
    //    }
    //}

    [HarmonyPatch(typeof(Profile), "Awake")]
    internal class ProfileAwake
    {
        static void Postfix(Profile __instance)
        {
            var carColors = new CarColors[Constants.carNb];
            for (int i = 0; i < carColors.Length; i++)
                carColors[i] = G.Sys.ProfileManager_.carInfos_[i].colors_;

            var field = __instance.GetType().GetField("carColorsList_", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(__instance, carColors);
        }
    }
}