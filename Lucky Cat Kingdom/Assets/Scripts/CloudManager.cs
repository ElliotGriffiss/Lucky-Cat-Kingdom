using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int NumberOfClouds;
    [SerializeField] private Transform StartPos;
    [SerializeField] private Transform EndPos;
    [Space]
    [SerializeField] private float CloudSpeed;


    [Header("Prefabs")]
    [SerializeField] private List<GameObject> InstantiatedGameObjects;

    private void Update()
    {
        foreach(GameObject obj in InstantiatedGameObjects)
        {
            if (obj.transform.position.x > EndPos.position.x)
            {
                obj.transform.position = new Vector2(StartPos.position.x, obj.transform.position.y);
            }
            else
            {
                obj.transform.position = new Vector2(obj.transform.position.x + (CloudSpeed * Time.deltaTime), obj.transform.position.y);
            }
        }
    }
}
