using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiPath : MonoBehaviour
{
    public List<Transform> points;
    public CarAI car;
    private Transform targetPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        targetPoint = points[0];
    }

    // Update is called once per frame
    void Update()
    {
        
        car.CustomPath(targetPoint);
    }
}
