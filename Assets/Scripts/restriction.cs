using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class restriction : MonoBehaviour
{
    public BoxCollider col;
    public MeshRenderer meshRend;
    private void Start()
    {
        col = GetComponent<BoxCollider>();
        meshRend = GetComponent<MeshRenderer>();

    }
    private void OnTriggerExit(Collider other)
    {
        col.isTrigger = false;
        meshRend.enabled = false;
    }
}
