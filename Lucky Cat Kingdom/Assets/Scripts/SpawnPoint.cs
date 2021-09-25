using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private Animator Anim;
    [SerializeField] private ParticleSystem RespawnParticles;
    public Transform SpawnPosition;

    public void SetSpawnPoint()
    {
        Anim.SetBool("IsActive", true);
    }

    public void PlayerRespawned()
    {
        RespawnParticles.Play();
    }
}
