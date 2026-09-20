using UnityEngine;

public class HoldCameraSwitch : MonoBehaviour
{
    public Camera mainCamera;      // Основная камера (первая)
    public Camera secondaryCamera; // Вторая камера, на которую переключаемся

    private bool isOnSecondary = false;

    void Start()
    {
        // При старте убедимся, что активна основная камера, вторая выключена
        mainCamera.enabled = true;
        secondaryCamera.enabled = false;
    }

    void Update()
    {
        // Если зажата клавиша B
        if (Input.GetKey(KeyCode.B))
        {
            if (!isOnSecondary)
            {
                SwitchToSecondary();
            }
        }
        // Если клавиша B отпущена
        else if (Input.GetKeyUp(KeyCode.B))
        {
            if (isOnSecondary)
            {
                SwitchToMain();
            }
        }
    }

    void SwitchToSecondary()
    {
        mainCamera.enabled = false;
        secondaryCamera.enabled = true;
        isOnSecondary = true;
    }

    void SwitchToMain()
    {
        mainCamera.enabled = true;
        secondaryCamera.enabled = false;
        isOnSecondary = false;
    }
}