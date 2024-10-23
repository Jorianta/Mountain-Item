using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using System;
using RoR2.UI;
using UnityEngine.UI;
using Rewired.Utils;

namespace MountainShrineItem
{
    public class MountainShrineItem
    {
        public static ItemDef itemDef;
        // public static BuffDef chestBuff;

        public static int bonusItems = ConfigManager.bonusItems.Value;
        //NOTE: the tp director has 600 credits by default
        public static float bossCredits = ConfigManager.difficultyPercent.Value * 6;
        public static float difficultyMultiplier = ConfigManager.difficultyPercent.Value/100;
        public static bool multiplyShrines = ConfigManager.multiplyShrines.Value;

        internal static void Init()
        {
            Log.Info("Initializing Mons Crisium Item");
            //ITEM//
            itemDef = ScriptableObject.CreateInstance<ItemDef>();

            itemDef.name = "MOUNTAINSHRINEITEM";
            itemDef.nameToken = "ITEM_MOUNTAINSHRINEITEM_NAME";
            itemDef.pickupToken = "ITEM_MOUNTAINSHRINEITEM_PICKUP";
            itemDef.descriptionToken = "ITEM_MOUNTAINSHRINEITEM_DESC";
            itemDef.loreToken = "ITEM_MOUNTAINSHRINEITEM_LORE";

            itemDef.AutoPopulateTokens();

            ItemTierCatalog.availability.CallWhenAvailable(() =>
            {
                if (itemDef) itemDef.tier = ItemTier.Lunar;
            });

            itemDef.tags = new ItemTag[]
            {
                ItemTag.Utility,
                ItemTag.CannotCopy,
                ItemTag.CannotSteal,
                ItemTag.AIBlacklist,
                ItemTag.HoldoutZoneRelated,
            };

            itemDef.pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/FragileDamageBonus/texDelicateWatchIcon.png").WaitForCompletion();
            itemDef.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FragileDamageBonus/DisplayDelicateWatch.prefab").WaitForCompletion();
            
            ModelPanelParameters ModelParams = itemDef.pickupModelPrefab.AddComponent<ModelPanelParameters>();

            ModelParams.minDistance = 5;
            ModelParams.maxDistance = 10;
            // itemDef.pickupModelPrefab.GetComponent<ModelPanelParameters>().cameraPositionTransform.localPosition = new Vector3(1, 1, -0.3f); 
            // itemDef.pickupModelPrefab.GetComponent<ModelPanelParameters>().focusPointTransform.localPosition = new Vector3(0, 1, -0.3f);
            // itemDef.pickupModelPrefab.GetComponent<ModelPanelParameters>().focusPointTransform.localEulerAngles = new Vector3(0, 0, 0);
            
            
            itemDef.canRemove = true;
            itemDef.hidden = false;

            ItemDisplayRuleDict displayRules = new ItemDisplayRuleDict(null);
            ItemAPI.Add(new CustomItem(itemDef, displayRules));
            
            Hooks();

            Debug.Log("Mons Crisium Initialized");
        }

        private static void Hooks()
        {
            // On.RoR2.BossGroup.DropRewards += BossGroup_DropRewards;
            TeleporterInteraction.onTeleporterBeginChargingGlobal +=  TeleporterInteraction_onTeleporterBeginChargingGlobal;
        }

        // public static void BossGroup_DropRewards(On.RoR2.BossGroup.orig_DropRewards orig, BossGroup self)
        //     {
        //         if (NetworkServer.active)
        //         {
        //             int stackCount = Util.GetItemCountForTeam(TeamIndex.Player, itemDef.itemIndex, true);
                    
        //             //Add 1 item per stack per player
        //             int itemsToAdd = stackCount * bonusItems * Run.instance.participatingPlayerCount;
        //             if (multiplyShrines) itemsToAdd *= self.

        //             Log.Debug("Items Added: " + stackCount * bonusItems * Run.instance.participatingPlayerCount);
        //             self.bonusRewardCount += stackCount * bonusItems * Run.instance.participatingPlayerCount;
        //         }
        //         orig(self);
        //     }

        public static void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction self)
        {
            int stackCount = Util.GetItemCountForTeam(TeamIndex.Player, itemDef.itemIndex, true);
                //Add Director Credits
            if (NetworkServer.active)
            {
                if (self.bossDirector && self.bossGroup)
                {
                    if (stackCount > 0)
                    {
                      
                        var creditsToAdd = bossCredits * stackCount;
                        if (multiplyShrines) creditsToAdd *= self.shrineBonusStacks + 1;

                        Log.Debug("Added Credits: " + creditsToAdd);
                        self.bossDirector.monsterCredit += (int)(creditsToAdd * Mathf.Pow(Run.instance.compensatedDifficultyCoefficient, 0.5f));


                        //Add 1 item per stack per player
                        int itemsToAdd = stackCount * bonusItems * Run.instance.participatingPlayerCount;
                        if (multiplyShrines) itemsToAdd *= self.shrineBonusStacks + 1;

                        Log.Debug("Items Added: " + itemsToAdd);
                        self.bossGroup.bonusRewardCount += itemsToAdd;
                    }
                }
            }
        }
    }
}

