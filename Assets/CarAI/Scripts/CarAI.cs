using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    [Header("Car Wheels (Wheel Collider)")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider backLeft;
    public WheelCollider backRight;

    [Header("Car Wheels (Transform)")]
    public Transform wheelFL;
    public Transform wheelFR;
    public Transform wheelBL;
    public Transform wheelBR;

    [Header("Car Front (Transform)")]
    public Transform carFront;

    [Header("General Parameters")]
    public List<string> NavMeshLayers = new List<string> { "Walkable" }; // По умолчанию стандартный слой
    public int MaxSteeringAngle = 45;
    public int MaxRPM = 150;

    [Header("Debug")]
    public bool ShowGizmos = true;
    public bool Debugger = true;

    [Header("Destination Parameters")]
    public bool Patrol = true;
    public Transform CustomDestination;

    [HideInInspector] public bool move = true;

    private Vector3 PositionToFollow = Vector3.zero;
    private int currentWayPoint;
    private float AIFOV = 60;
    private bool allowMovement;
    private int NavMeshLayerMask; // Исправлено название
    private List<Vector3> waypoints = new List<Vector3>();
    private float LocalMaxSpeed;
    private int Fails;
    private float MovementTorque = 1;
    private bool isCalculatingPath = false; // Защита от бесконечного спавна путей

    void Awake()
    {
        currentWayPoint = 0;
        allowMovement = true;
        move = true;
    }

    void Start()
    {
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
        }

        CalculateNavMeshLayerMask();

        // Генерируем первый путь при старте
        CreatePath();
    }

    void FixedUpdate()
    {
        UpdateWheels();
        ApplySteering();
        PathProgress();
    }

    private void CalculateNavMeshLayerMask()
    {
        if (NavMeshLayers == null || NavMeshLayers.Count == 0 || NavMeshLayers[0] == "AllAreas")
        {
            NavMeshLayerMask = NavMesh.AllAreas;
            return;
        }

        NavMeshLayerMask = 0;
        foreach (string layer in NavMeshLayers)
        {
            int areaIndex = NavMesh.GetAreaFromName(layer);
            if (areaIndex != -1)
            {
                NavMeshLayerMask |= (1 << areaIndex);
            }
        }
    }

    private void PathProgress()
    {
        wayPointManager();
        Movement();
        ListOptimizer();

        void wayPointManager()
        {
            if (waypoints.Count == 0 || currentWayPoint >= waypoints.Count)
            {
                allowMovement = false;
                // Если точки кончились, запрашиваем новый путь (если еще не считаем его)
                if (!isCalculatingPath) CreatePath();
                return;
            }

            PositionToFollow = waypoints[currentWayPoint];
            allowMovement = true;

            // Если подъехали к точке ближе чем на 3 метра — переключаемся на следующую
            if (Vector3.Distance(carFront.position, PositionToFollow) < 3f)
            {
                currentWayPoint++;
            }

            // Запрашиваем новый путь ТОЛЬКО когда осталось меньше 2 точек И мы еще не в процессе расчета
            if (currentWayPoint >= waypoints.Count - 2 && !isCalculatingPath)
            {
                CreatePath();
            }
        }

        void ListOptimizer()
        {
            // Очищаем старые пройденные точки, чтобы не забивать память
            if (currentWayPoint > 5)
            {
                waypoints.RemoveRange(0, currentWayPoint - 2);
                currentWayPoint = 2;
            }
        }
    }

    public void CreatePath()
    {
        if (CustomDestination == null)
        {
            if (Patrol)
            {
                RandomPath();
            }
            else
            {
                debug("No custom destination assigned and Patrol is set to false", false);
                allowMovement = false;
            }
        }
        else
        {
            CustomPath(CustomDestination);
        }
    }

    public void RandomPath()
    {
        isCalculatingPath = true;
        NavMeshPath path = new NavMeshPath();
        Vector3 sourcePosition = (waypoints.Count == 0) ? carFront.position : waypoints[waypoints.Count - 1];

        // Ищем случайную точку на NavMesh в радиусе 80 метров
        Vector3 randomDirection = Random.insideUnitSphere * 80f;
        randomDirection += sourcePosition;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 80f, NavMeshLayerMask))
        {
            if (NavMesh.CalculatePath(sourcePosition, hit.position, NavMeshLayerMask, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                // Добавляем точки (пропуская самую первую, так как это стартовая позиция машины)
                if (path.corners.Length > 1)
                {
                    waypoints.AddRange(path.corners.Skip(1).ToList());
                    debug("Random Path generated successfully. Total waypoints: " + waypoints.Count, false);
                }
            }
        }
        isCalculatingPath = false;
    }

    public void CustomPath(Transform destination)
    {
        isCalculatingPath = true;
        NavMeshPath path = new NavMeshPath();
        Vector3 sourcePosition = (waypoints.Count == 0) ? carFront.position : waypoints[waypoints.Count - 1];

        if (NavMesh.SamplePosition(destination.position, out NavMeshHit hit, 50f, NavMeshLayerMask))
        {
            if (NavMesh.CalculatePath(sourcePosition, hit.position, NavMeshLayerMask, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                // Очищаем старый путь, если едем к конкретной кастомной точке, чтобы не наслаивать маршруты
                if (waypoints.Count == 0 || Vector3.Distance(waypoints.Last(), hit.position) > 5f)
                {
                    waypoints.Clear();
                    currentWayPoint = 0;
                }

                waypoints.AddRange(path.corners.ToList());
                debug("Custom Path generated successfully", false);
            }
        }
        isCalculatingPath = false;
    }

    private void ApplyBrakes()
    {
        frontLeft.brakeTorque = 5000;
        frontRight.brakeTorque = 5000;
        backLeft.brakeTorque = 5000;
        backRight.brakeTorque = 5000;

        frontLeft.motorTorque = 0;
        frontRight.motorTorque = 0;
        backLeft.motorTorque = 0;
        backRight.motorTorque = 0;
    }

    private void UpdateWheels()
    {
        ApplyRotationAndPosition(frontLeft, wheelFL);
        ApplyRotationAndPosition(frontRight, wheelFR);
        ApplyRotationAndPosition(backLeft, wheelBL);
        ApplyRotationAndPosition(backRight, wheelBR);
    }

    private void ApplyRotationAndPosition(WheelCollider targetWheel, Transform wheel)
    {
        if (targetWheel == null || wheel == null) return;

        targetWheel.ConfigureVehicleSubsteps(5, 12, 15);
        Vector3 pos;
        Quaternion rot;
        targetWheel.GetWorldPose(out pos, out rot);
        wheel.position = pos;
        wheel.rotation = rot;
    }

    void ApplySteering()
    {
        if (waypoints.Count == 0 || currentWayPoint >= waypoints.Count) return;

        Vector3 relativeVector = transform.InverseTransformPoint(PositionToFollow);

        // Плавный расчет угла поворота колес
        float SteeringAngle = (relativeVector.x / relativeVector.magnitude) * MaxSteeringAngle;
        SteeringAngle = Mathf.Clamp(SteeringAngle, -MaxSteeringAngle, MaxSteeringAngle);

        if (Mathf.Abs(SteeringAngle) > 15)
            LocalMaxSpeed = MaxRPM * 0.5f; // Снижаем скорость на крутых поворотах
        else
            LocalMaxSpeed = MaxRPM;

        frontLeft.steerAngle = SteeringAngle;
        frontRight.steerAngle = SteeringAngle;
    }

    void Movement()
    {
        bool finalMoveState = move && allowMovement;

        if (finalMoveState && waypoints.Count > 0)
        {
            frontLeft.brakeTorque = 0;
            frontRight.brakeTorque = 0;
            backLeft.brakeTorque = 0;
            backRight.brakeTorque = 0;

            float SpeedOfWheels = (frontLeft.rpm + frontRight.rpm + backLeft.rpm + backRight.rpm) / 4f;

            if (SpeedOfWheels < LocalMaxSpeed)
            {
                float torque = 400 * MovementTorque;
                backRight.motorTorque = torque;
                backLeft.motorTorque = torque;
                frontRight.motorTorque = torque;
                frontLeft.motorTorque = torque;
            }
            else if (SpeedOfWheels < LocalMaxSpeed * 1.25f)
            {
                backRight.motorTorque = 0;
                backLeft.motorTorque = 0;
                frontRight.motorTorque = 0;
                frontLeft.motorTorque = 0;
            }
            else
            {
                ApplyBrakes();
            }
        }
        else
        {
            ApplyBrakes();
        }
    }

    void debug(string text, bool IsCritical)
    {
        if (Debugger)
        {
            if (IsCritical) Debug.LogError(text, this);
            else Debug.Log(text, this);
        }
    }

    private void OnDrawGizmos()
    {
        if (!ShowGizmos || waypoints == null) return;

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (i == currentWayPoint) Gizmos.color = Color.blue;
            else if (i > currentWayPoint) Gizmos.color = Color.red;
            else Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(waypoints[i], 1.5f);

            if (i > 0)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(waypoints[i - 1], waypoints[i]);
            }
        }
    }
}