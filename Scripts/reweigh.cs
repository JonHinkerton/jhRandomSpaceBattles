using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Globalmap.SectorMap;
using Kingmaker.Globalmap;
using Kingmaker.Utility;
using System.Collections;
using System.Collections.Generic;
using Kingmaker.Blueprints.Root;
using Kingmaker.Globalmap.Blueprints.SectorMap;
using System;
using System.Reflection;
using System.Linq;
using Kingmaker.Networking;
using Kingmaker;
using Kingmaker.Modding;
using Owlcat.Runtime.Core.Logging;

namespace jhRandomSpaceBattles
{
    [HarmonyPatch]
    public static class Main
    {
        internal static LogChannel log;
        internal static OwlcatModification jhrsbmod;

        [OwlcatModificationEnterPoint]
        public static void Load(OwlcatModification mod)
        {
            jhrsbmod = mod;
            log = mod.Logger;
            Harmony harmony = new(jhrsbmod.Manifest.UniqueName);
            harmony.PatchAll();
            log.Log("jhrsb: Load");
        }

        [HarmonyPatch(typeof(SaveNetManager), nameof(SaveNetManager.PostLoad))]
        [HarmonyPostfix]
        static void MyMethod()
        {

            log.Log("jhrsb: PostLoad");

            FieldInfo m_InitialWeightsInfo = AccessTools.Field(typeof(RandomWeightsForSave<BlueprintDialogReference>), "m_InitialWeights");
            FieldInfo m_CurrentWeightsInfo = AccessTools.Field(typeof(RandomWeightsForSave<BlueprintDialogReference>), "m_CurrentWeights");

            var state = Game.Instance.Player.GlobalMapRandomGenerationState;
            var customDialogs = ResourcesLibrary.TryGetBlueprint<BlueprintWarpRoutesSettings>("7d1f397ee0624f7da48e87aa813e42f7");
            log.Log("jhrsb: " + customDialogs);
            foreach (var diff in Enum.GetValues(typeof(SectorMapPassageEntity.PassageDifficulty)))
            {
                log.Log("jhrsb: " + diff);
                var dialogs = state.GetRandomEncountersDialogs((SectorMapPassageEntity.PassageDifficulty)diff);
                log.Log("jhrsb: " + dialogs);
                var newdialogs = customDialogs.DifficultySettingsList.FirstOrDefault(ds => ds.Difficulty == (SectorMapPassageEntity.PassageDifficulty)diff).RandomEncounters;
                log.Log("jhrsb: " + newdialogs);

                var initialWeights = (List<WeightPair<BlueprintDialogReference>>)m_InitialWeightsInfo.GetValue(dialogs);
                log.Log("jhrsb: " + initialWeights);
                var currentWeights = (List<WeightPair<BlueprintDialogReference>>)m_CurrentWeightsInfo.GetValue(dialogs);
                log.Log("jhrsb: " + currentWeights);

                foreach (var dia in newdialogs.Weights)
                {
                    log.Log("jhrsb: " + dia);
                    if (!initialWeights.Any(w => w.Object.Guid == dia.Object.Guid))
                    {
                        WeightPair<BlueprintDialogReference> wgt = new WeightPair<BlueprintDialogReference>(dia.Object, dia.Weight);
                        initialWeights.Add(wgt);
                        currentWeights.Add(wgt);
                        log.Log("jhrsb: " + wgt);
                    }
                }
            }
        }
    }
}

//var encounters = dialogs.;

//foreach (var enc in dialogs.Weights)
//{
//    if (!initialWeights.Any(w => w.Object.Guid == enc.Object.Guid))
//    {
//            initialWeights.Add(enc);
//            currentWeights.Add(enc);
//    }
//}

//if (!initialWeights.Any(w => w.Object == enc.Object))
//{
//    WeightPair<BlueprintDialogReference> weights = enc;
//    foreach (var weightPair in weights)
//    {
//        initialWeights.Add(weightPair);
//        currentWeights.Add(weightPair);
//    }


//}

//.DifficultySettingsList
//.Where(ds => ds.Difficulty == difficulty)
//.SelectMany(ds => ds.RandomEncounters);

//[HarmonyPatch(typeof(GlobalMapRandomGenerationState), nameof(GlobalMapRandomGenerationState.GetRandomEncountersDialogs)), HarmonyPostfix]
//public static RandomWeightsForSave<BlueprintDialogReference> GetRandomEncountersDialogs(ref RandomWeightsForSave<BlueprintDialogReference> __result, SectorMapPassageEntity.PassageDifficulty difficulty)
//{
//    var warproutesettings = ResourcesLibrary.TryGetBlueprint<BlueprintWarpRoutesSettings>("c9c5501906994aad9d210206ab943264");
//    RandomWeightsForSave<BlueprintDialogReference> newList = new RandomWeightsForSave<BlueprintDialogReference>();
//    foreach (var diff in warproutesettings.DifficultySettingsList)
//    {
//        if(diff.ToString() == difficulty.ToString())
//        {

//            foreach (var dialog in diff)
//            {

//                newList.Add(dialog);
//            }
//            return newList;
//        }


//        return RandomEncounterDialogWeights.FirstOrDefault((PassageDifficultyDialogsData re) => re.Difficulty == difficulty).Dialogs;

//        return newList;
//}

//System.Random random = new System.Random();
//double chance = 0.1; 
//if (random.NextDouble() < chance)
//{
//    RandomWeightsForSave<BlueprintDialogReference> spaceBattle = new RandomWeightsForSave<BlueprintDialogReference>(); 
//    return spaceBattle;
//}
//else
//{
//    return __result;
//}


//RandomEncounterDialogWeights.Add(new PassageDifficultyDialogsData
//{
//    Difficulty = difficultySettings.Difficulty,
//    Dialogs = new RandomWeightsForSave<BlueprintDialogReference>(difficultySettings.RandomEncounters)
//});


//}
