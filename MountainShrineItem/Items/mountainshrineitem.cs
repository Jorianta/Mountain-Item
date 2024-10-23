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

        public static bool isEnabled = true;
        public static int bonusItems = 1;
        //NOTE: the tp director has 600 credits by default
        public static float bossCredits = 600f;

        internal static void Init()
        {
            Debug.Log("Initializing Mons Crisium Item");
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
                ItemTag.InteractableRelated,
                ItemTag.AIBlacklist,
                ItemTag.OnStageBeginEffect
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
            On.RoR2.BossGroup.DropRewards += BossGroup_DropRewards;
            TeleporterInteraction.onTeleporterBeginChargingGlobal +=  TeleporterInteraction_onTeleporterBeginChargingGlobal;
        }

        public static void BossGroup_DropRewards(On.RoR2.BossGroup.orig_DropRewards orig, BossGroup self)
            {
                if (NetworkServer.active)
                {
                    int itemCount = Util.GetItemCountForTeam(TeamIndex.Player, itemDef.itemIndex, true);
                    Log.Debug(itemCount * bonusItems * Run.instance.participatingPlayerCount + " Bonus Items");
                    
                    //Add 1 item per stack per player
                    self.bonusRewardCount += itemCount * bonusItems * Run.instance.participatingPlayerCount;
                }
                orig(self);
            }

        public static void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction self)
        {
            int itemCount = Util.GetItemCountForTeam(TeamIndex.Player, itemDef.itemIndex, true);
                //Add Director Credits
            if (NetworkServer.active)
            {
                if (self.bossDirector)
                {
                    if (itemCount > 0)
                    {
                        var creditsToAdd = bossCredits * itemCount;
                        Log.Debug("Credits: " +creditsToAdd);
                        self.bossDirector.monsterCredit += (int)(creditsToAdd * Mathf.Pow(Run.instance.compensatedDifficultyCoefficient, 0.5f));
                    }
                }
            }
        }

        public class MountainItemBehaviour : MonoBehaviour
        {

            public void Start()
            {
                Log.Debug("MountainItemBehavior:Start()");


                
            }

                    //Add extra drops
            

            
        }
    }
}
