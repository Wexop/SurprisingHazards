using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurprisingHazards.Scripts;

public class SurprisingHazardBehavior: NetworkBehaviour
{
    public List<AudioClip> audioClips;
    public AudioSource audioSource;

    public GameObject parent;
    private Vector3 baseScale;
    private float animationTimer;
    private float animationDuration = 0.5f;
    public ulong networkId;
    public bool isServer;
    
    private bool animationGrow;

    private int state;
    private int lastState;

    private float visibleRange;

    public void Start()
    {
        visibleRange = SurprisingHazardsPlugin.instance.GetCustomDistance(parent.name);

        baseScale = parent.transform.localScale;
        audioSource.maxDistance = visibleRange + 5;

        transform.parent = null;
        state = 0;

    }
    

    private void Update()
    {
        state = 0;
        StartOfRound.Instance?.allPlayerScripts?.ToList().ForEach(player =>
        {
            if (Vector3.Distance(player.transform.position, transform.position) <=
                visibleRange)
            {
                state = 1;
            }
        });

        if (state != lastState)
        {
            if (state == 1)
            {
                GetRandomClip();
                animationTimer = 0;
                animationGrow = true;
            }
            else
            {
                animationTimer = 0;
                animationGrow = false;
            }
        }
        
        if (!transform.position.Equals(parent.transform.position)) transform.position = parent.transform.position;
        if (animationTimer < animationDuration)
        {
            animationTimer += Time.deltaTime;
            parent.transform.localScale = animationGrow ? baseScale * (Mathf.Clamp(animationTimer / animationDuration, 0, 1)) : baseScale * Mathf.Clamp(animationDuration - animationTimer, 0, 1);
        }

        lastState = state;

    }

    public void SetClip(int audioClip)
    {
        audioSource.clip = audioClips[audioClip];
        audioSource.volume = SurprisingHazardsPlugin.instance.audioVolume.Value;
        audioSource.Play();
    }
    
    public void GetRandomClip()
    {
        if (isServer)
        {
            NetworkSurprisingHazard.ChangeClipClientRpc(networkId, Random.Range(0, audioClips.Count));
        }
    }
}