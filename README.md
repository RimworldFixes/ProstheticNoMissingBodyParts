# Prosthetic No Missing Body Parts

This mod allow your pawns with bionic arms and legs continue wearing apparel for feet and hands.

Works with vanilla, EPOE, VAE, Jewelry, etc. Does not require new save game. Can be removed any time.

This mod does not change body part groups and do not try to connect fingers and hands to torso directly like as [KV] Keep Hands and Feet - 1.1

Instead of this, this mod patch original Rimworld ApparelUtility method "HasPartsToWear" on the fly. 
If your pawn has two bionic legs, it cannot wear boots by default. This happens cause game remove all feet, when you try to install bionic upgrade on your legs.
When original game code return that pawn does not have any body part for wearing boots or gloves (negative check), patch code starts and try to find any bionic body part on upper levels, like Bionic Arm or Bionic Leg. 
If any bionic part exists in right place, patch change original result value to positive. And your pawn can continue wearing any gear on it hands, feet, etc...

Parts whitelisted for arms:
- SimpleProstheticArm
- BionicArm
- AdvancedBionicArm
- ArchotechArm

For legs:
- SimpleProstheticLeg
- BionicLeg
- AdvancedBionicLeg
- ArchotechLeg
- MuscleStimulatorLegs

You can change settings of whitelisted body part any time in mod settings. Reloading does not required.

Please, locate this mod in the end part of your mod list. After all othe mods that adds bionic parts.

Compatibility
- Must be compatible with all other mod, exclude mods that do the same patch as this mod.
- Tested with 100+ mods, including EPOE, VAE, Jewelry, etc... 
- Does not test with any CE mods, cause I does not use CE in my games.
- HarmonyLib Required.
