using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject Car;

    // Координаты из скриншота (Position)
    private Vector3 positionOffset = new Vector3(-0.051f, 3.308f, -5.921f);

    // Координаты из скриншота (Rotation)
    private Vector3 rotationOffset = new Vector3(15.677f, 0.687f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        // Устанавливаем начальную позицию камеры относительно машины
        transform.position = Car.transform.position + positionOffset;
        transform.eulerAngles = rotationOffset;
    }

    // LateUpdate лучше подходит для камер, следящих за объектами (убирает дергания)
    void LateUpdate()
    {
        if (Car == null) return;

        // 1. Обновляем ПОЗИЦИЮ
        // Чтобы камера вращалась вокруг машины вместе с ней, умножаем смещение на поворот машины.
        // Если вы хотите, чтобы камера двигалась строго по мировым осям (не поворачивалась вокруг машины),
        // замените следующую строку на: transform.position = Car.transform.position + positionOffset;
        Vector3 rotatedOffset = Car.transform.rotation * positionOffset;
        transform.position = Car.transform.position + rotatedOffset;

        // 2. Обновляем ПОВОРОТ
        // Берем поворот машики по оси Y и прибавляем к нему наши смещения из инспектора
        float carY = Car.transform.eulerAngles.y;
        transform.eulerAngles = new Vector3(rotationOffset.x, carY + rotationOffset.y, rotationOffset.z);
    }
}