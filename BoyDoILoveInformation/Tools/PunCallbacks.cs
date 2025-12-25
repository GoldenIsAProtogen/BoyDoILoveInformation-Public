using System.Collections.Generic;
using BoyDoILoveInformation.Core;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace BoyDoILoveInformation.Tools;

public class PunCallbacks : MonoBehaviourPunCallbacks
{
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        VRRig rig = GorillaParent.instance.vrrigs.Find(rig => rig.OwningNetPlayer.GetPlayerRef().Equals(targetPlayer));

        if (rig == null || !rig.HasCosmetics())
            return;

        if (!Extensions.PlayerMods.ContainsKey(rig))
            Extensions.PlayerMods[rig] = [];
        
        Extensions.PlayerMods[rig].Clear();
        
        Hashtable    properties = targetPlayer.CustomProperties;
        List<string> mods       = [];
        List<string> cheats     = [];

        foreach (string key in properties.Keys)
        {
            if (Plugin.KnownCheats.TryGetValue(key, out string cheat))
                mods.Add($"[<color=red>{cheat}</color>]");

            if (Plugin.KnownMods.TryGetValue(key, out string mod))
                mods.Add($"[<color=green>{mod}</color>]");
        }
        
        foreach (string key in changedProps.Keys)
            if (Plugin.KnownCheats.TryGetValue(key, out string cheat))
                cheats.Add($"[<color=red>{cheat}</color>]");
        
        if (cheats.Count > 0)
            Notifications.SendNotification(
                    $"[<color=red>Cheater</color>] Player {rig.OwningNetPlayer.SanitizedNickName} has the following cheats: {string.Join(", ", cheats)}.");

        Extensions.PlayerMods[rig] = mods;
    }
}