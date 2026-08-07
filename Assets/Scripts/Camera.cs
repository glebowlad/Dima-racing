using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject Car;
    private float CarY;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        CarY = Car.transform.eulerAngles.y;
        transform.eulerAngles = new Vector3(0, CarY, 0);
    }
}
