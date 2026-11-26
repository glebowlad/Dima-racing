using UnityEngine;

public class CarRespawn : MonoBehaviour
{
    [Header("Настройки")]
    public Transform[] waypoints; // Сюда перетащите все путевые точки по порядку
    public float stuckTimeLimit = 4f; 
    public float minSpeedThreshold = 0.5f; // Порог скорости вместо строгого нуля

    private Rigidbody rb;
    private float stuckTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Проверяем, пытается ли игрок ехать (жмет газ или задний ход)
        bool isTryingToMove = Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;

        // Проверяем, стоит ли машина на месте (с учетом микровибраций физики)
        bool isNotMoving = rb.velocity.magnitude < minSpeedThreshold;

        // Также проверяем, не перевернулась ли машина на крышу/бок
        bool isFlipped = Vector3.Dot(transform.up, Vector3.up) < 0.3f;

        if (( isNotMoving) || isFlipped)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= stuckTimeLimit)
            {
                RespawnToNearestWaypoint();
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }

    public void RespawnToNearestWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // 1. Ищем ближайшую путевую точку
        int nearestIndex = 0;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, waypoints[i].position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearestIndex = i;
            }
        }

        // 2. Определяем следующую точку, чтобы знать, куда повернуть нос машины
        int nextIndex = (nearestIndex + 1) % waypoints.Length;
        Vector3 targetPos = waypoints[nearestIndex].position;
        Vector3 lookTarget = waypoints[nextIndex].position;

        // 3. Сбрасываем физику
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 4. Перемещаем машину чуть выше дороги и разворачиваем по трассе
        transform.position = targetPos + Vector3.up * 0.8f;

        Vector3 direction = (lookTarget - targetPos).normalized;
        direction.y = 0; // Выравниваем по горизонту, чтобы машина не клевала носом
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        stuckTimer = 0f;
    }
}
