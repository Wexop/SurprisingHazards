using UnityEngine;

namespace SurprisingHazards.Scripts;

public class RegisteredHazard
{
    public GameObject gameObject;
    public ulong networkId;
    public SurprisingHazardBehavior surprisingHazardBehavior;
}