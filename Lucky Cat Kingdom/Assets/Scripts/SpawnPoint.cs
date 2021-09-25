using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private Animator Anim;
    [SerializeField] private Transform SpawnPosition;

    public void SetSpawnPoint()
    {
        Anim.SetBool("IsActive", true);
    }
}
