using Harmony;
using System;
using System.Reflection;
using UnityEngine;

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
            var carColors = new CarColors[G.Sys.ProfileManager_.carInfos_.Length];
            for (int i = 0; i < carColors.Length; i++)
                carColors[i] = G.Sys.ProfileManager_.carInfos_[i].colors_;

            var field = __instance.GetType().GetField("carColorsList_", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(__instance, carColors);
        }
    }

    //change additive to blend animation blendMode
    [HarmonyPatch(typeof(GadgetWithAnimation), "SetAnimationStateValues")]
    internal class GadgetWithAnimationSetAnimationStateValues
    {
        static bool Prefix(GadgetWithAnimation __instance)
        {
            var comp = __instance.GetComponentInChildren<Animation>();
            if(comp)
            {
                if (!ChangeBlendModeToBlend(comp.transform, __instance.animationName_))
                    return true;

                var state = comp[__instance.animationName_];
                if(state)
                {
                    state.layer = 3;
                    state.blendMode = AnimationBlendMode.Blend;
                    state.wrapMode = WrapMode.ClampForever;
                    state.enabled = true;
                    state.weight = 1f;
                    state.speed = 0f;
                }
            }

            return false;
        }

        static bool ChangeBlendModeToBlend(Transform obj, string animationName)
        {
            for(int i = 0; i < obj.childCount; i++)
            {
                var n = obj.GetChild(i).gameObject.name.ToLower();
                if (!n.StartsWith("#"))
                    continue;

                n = n.Remove(0, 1);
                var parts = n.Split(';');

                if(parts.Length == 1)
                {
                    if (parts[0] == "additive")
                        return false;
                    if (parts[0] == "blend")
                        return true;
                }
                if(parts[1] == animationName.ToLower())
                {
                    if (parts[0] == "additive")
                        return false;
                    if (parts[0] == "blend")
                        return true;
                }
            }
            return false;
        }
    }
}