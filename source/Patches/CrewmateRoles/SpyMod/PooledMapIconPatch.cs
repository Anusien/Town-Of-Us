using HarmonyLib;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SpyMod {
	[HarmonyPatch(typeof(PooledMapIcon), nameof(PooledMapIcon.Reset))]
	public static class PooledMapIconPatch {
		public static void Postfix(PooledMapIcon __instance) {
			var sprite = __instance.GetComponent<SpriteRenderer>();
			if (sprite != null) {
				PlayerControl.SetPlayerMaterialColors(new Color(0.8793f, 1, 0, 1), sprite); // Reset Color to default. Fixes Shifted Spy issue
			}
		}
	}
}