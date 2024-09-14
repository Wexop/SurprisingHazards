using System.Linq;
using StaticNetcodeLib;
using Unity.Netcode;
using UnityEngine;

namespace SurprisingHazards.Scripts;

[StaticNetcode]
public class NetworkSurprisingHazard
{
    [ClientRpc]
    public static void ChangeClipClientRpc(ulong networkId, int audioClip)
    {
        if(!SurprisingHazardsPlugin.instance.RegisteredHazards.ContainsKey(networkId)) return;
        
        var objectFound = SurprisingHazardsPlugin.instance.RegisteredHazards?[networkId];

        if (objectFound == null)
        {
            Debug.Log($"OBJECT NOT FOUND {networkId}");
        }
        else
        {
            objectFound.surprisingHazardBehavior.SetClip(audioClip);
        }
    }

}