using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using RimWorld;
using HarmonyLib;

namespace ProstheticNoMissingBodyParts
{
    
    [StaticConstructorOnStartup]
    partial class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("com.prostheticnomissingbodyparts.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log.Message(
                "ProstheticNoMissingBodyParts Harmony Patches:" + Environment.NewLine +
                "  Prefix:" + Environment.NewLine +
                "    ApparelUtility.HasPartsToWear");
        }
    }
    
    [Harmony]
    
    [HarmonyPatch(typeof(ApparelUtility), "HasPartsToWear")]
    static class Patch_ApparelUtility_HasPartsToWear
    {
        static bool Prefix(ref bool __result, Pawn p, ThingDef apparel)
        {
            // *****
            // Original Rimworld code start
            // ****
            
            List<Hediff> hediffs = p.health.hediffSet.hediffs;
            bool flag = false;
            for (int index = 0; index < hediffs.Count; ++index)
            {
                if (hediffs[index] is Hediff_MissingPart)
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                __result = true;
                return false;
            }
            IEnumerable<BodyPartRecord> notMissingParts = p.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, (BodyPartTagDef) null, (BodyPartRecord) null);
            List<BodyPartGroupDef> groups = apparel.apparel.bodyPartGroups;
            for (int i = 0; i < groups.Count; i++)
            {
                if (notMissingParts.Any<BodyPartRecord>((Func<BodyPartRecord, bool>) (x => x.IsInGroup(groups[i]))))
                {
                    __result = true;
                    return false;
                }
            }
            
            // *****
            // Original Rimworld code end
            // ****

            
            // *****
            // ProstheticNoMissingBodyParts patch code start
            // *****
            
            // this code run only if original Rimworld check return false (body part is missed)
            // and we try to fix this, if pawn has a bionic part in right place (Bionic Arm, Bionic Leg, etc...)
            
            // load mod settings with whitelisted bionic parts for arms and legs
            var settings = LoadedModManager
                .GetMod<ProstheticNoMissingBodyPartsMod>()
                .GetSettings<ProstheticNoMissingBodyPartsSettings>();

            // prepare hash sets for quicker checks
            var armsWhitelist = new HashSet<string>();
            var legsWhitelist = new HashSet<string>();

            // for arms
            if (settings.ArmsWhitelist != null)
            {
                armsWhitelist.AddRange(settings.ArmsWhitelist);
            }
            
            // for legs
            if (settings.LegsWhitelist != null)
            {
                legsWhitelist.AddRange(settings.LegsWhitelist);
            }
            
            // prepare bool set for apparel body part groups
            var isLeftHand = false;
            var isRightHand = false;
            var isHands = false;
            var isFeet = false;

            foreach (var g in groups)
            {
                if (g.defName.Equals("LeftHand"))
                {
                    isLeftHand = true;
                    break;
                }

                if (g.defName.Equals("RightHand"))
                {
                    isRightHand = true;
                    break;
                }
                
                if (g.defName.Equals("Hands"))
                {
                    isHands = true;
                    break;
                }
                
                if (g.defName.Equals("Feet"))
                {
                    isFeet = true;
                    break;
                }

                var body = p.def.race.body.AllParts;
                
                if (BodyPartUtils.ExistsByGroupAndParent(body, "Shoulder", g.defName))
                {
                    isHands = true;
                    break;
                }
                
                if (BodyPartUtils.ExistsByGroupAndParent(body, "Leg", g.defName))
                {
                    isFeet = true;
                    break;
                }
            }
            
            // check if apparel needed left hand, especially for jewelry mod, pawns try to wear bracer... on fingers... omg...
            if (isLeftHand)
            {
                __result = hediffs.Exists((h) =>
                {
                    if (h.Part?.customLabel != null && h.def?.defName != null)
                    {
                        // true if left arm replaced with whitelisted bionic part
                        return h.Part.def.defName.Equals("Shoulder") &&
                               armsWhitelist.Contains(h.def.defName) &&
                               BodyPartUtils.ExistsDeep(h.Part, "LeftHand");
                    }

                    return false;
                });
                return false;
            }
            
            // check if apparel needed right hand, especially for jewelry, wearing rings?
            if (isRightHand)
            {
                __result = hediffs.Exists((h) =>
                {
                    if (h.Part?.customLabel != null && h.def?.defName != null)
                    {
                        // true if any right arm replaced with whitelisted bionic part
                        return h.Part.def.defName.Equals("Shoulder") &&
                               armsWhitelist.Contains(h.def.defName) &&
                               BodyPartUtils.ExistsDeep(h.Part, "RightHand");
                    }

                    return false;
                });
                return false;
            }
            
            // check if apparel needed hands, useful for gloves
            if (isHands)
            {
                __result = hediffs.Exists((h) =>
                {
                    if (h.Part?.def?.defName != null && h.def?.defName != null)
                    {
                        // true if any arm replaced with whitelisted bionic part
                        return h.Part.def.defName.Equals("Shoulder") &&
                               armsWhitelist.Contains(h.def.defName);
                    }

                    return false;
                });
                return false;
            }
            
            // check if apparel needed feet, useful for boots
            if (isFeet)
            {
                __result = hediffs.Exists((h) =>
                {
                    if (h.Part?.def?.defName != null && h.def?.defName != null)
                    {
                        // true if any leg replaced with whitelisted bionic part
                        return h.Part.def.defName.Equals("Leg") &&
                               legsWhitelist.Contains(h.def.defName);
                    }

                    return false;
                });
                return false;
            }            
            
            // *****
            // ProstheticNoMissingBodyParts patch code end
            // *****
            
            // body part still missed
            
            __result = false;
            return false;
        }
    }
}