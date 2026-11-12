using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiPath : MonoBehaviour
{
    public List<Transform> points;
    public List<CarAI> cars;
    private Transform targetPoint;
    private int pointIndex=0;

    // Start is called before the first frame update
    void Start()
    {
        targetPoint = points[pointIndex];
        foreach (CarAI car in cars)
        {
            car.CustomDestination = targetPoint;
            car.Invoke(nameof(CarAI.CustomPath),0.2f);

        }

    }

    // Update is called once per frame
    void Update()
    {
        foreach (CarAI car in cars)
        {
           // car.CustomPath(targetPoint);

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
}
