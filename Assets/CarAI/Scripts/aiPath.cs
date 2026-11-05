using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiPath : MonoBehaviour
{
    public List<Transform> points;
    public CarAI car;
    private Transform targetPoint;
    private int pointIndex=0;

    // Start is called before the first frame update
    void Start()
    {
        targetPoint = points[pointIndex];
        car.CustomPath(targetPoint);
        
    }

    // Update is called once per frame
    void Update()
    {


        if (Vector3.Distance(targetPoint.position, car.transform.position) < 0.4f)
        {
            pointIndex++;
            targetPoint = points[pointIndex];
            car.CustomPath(targetPoint);
        }
    }
}
