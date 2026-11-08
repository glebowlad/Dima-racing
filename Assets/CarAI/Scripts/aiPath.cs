using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiPath : MonoBehaviour
{
    public List<Transform> points;
    public CarAI car;
    private Transform targetPoint;
    private int pointIndex=3;

    // Start is called before the first frame update
    void Start()
    {
        targetPoint = points[pointIndex];
        
    }

    // Update is called once per frame
    void Update()
    {

        car.CustomPath(targetPoint);

        if (Vector3.Distance(targetPoint.position, car.transform.position) <= 10f)
        {
            pointIndex++;
            if (pointIndex >= points.Count)
            { pointIndex = 0; }

            targetPoint = points[pointIndex];
            car.CustomPath(targetPoint);
        }
        
    }
}
