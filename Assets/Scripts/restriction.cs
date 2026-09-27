using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class restriction : MonoBehaviour
{
    public BoxCollider col;
    private void OnTriggerExit(Collider other)
    {
        col.isTrigger = false;
    }
}
