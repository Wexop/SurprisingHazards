using System;
using Unity.Netcode;

namespace SurprisingHazards.Scripts;

public class SurprisingHazardSetUp : NetworkBehaviour
{
    public void Start()
    {
        SurprisingHazardsPlugin.RegisterModdedHazard(this);
    }
}