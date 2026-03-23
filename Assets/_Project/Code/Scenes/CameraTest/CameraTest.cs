using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTest : MonoBehaviour
{
    public Transform target;
    public float distance = 10f;
    public float minDistance = 5f;
    public float maxDistance = 20f;
    public float rotationSpeed = 10f;
    public float zoomSpeed = 1.2f;

    private float currentAngle = 0f;  
    private float targetAngle = 0f;   
    private bool isRotating = false;  

    private Keyboard keyboard;
    private Mouse mouse;

    void Awake()
    {
        keyboard = Keyboard.current;
        mouse = Mouse.current;
    }

    void Update()
    {
        // Поворот камеры
        if (keyboard != null)
        {
            if (keyboard.qKey.wasPressedThisFrame)
            {
                targetAngle -= 90f;
                isRotating = true;
            }
            else if (keyboard.eKey.wasPressedThisFrame)
            {
                targetAngle += 90f;
                isRotating = true;
            }

            targetAngle = Mathf.Repeat(targetAngle, 360f);
        }

        if (isRotating)
        {
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);
            if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) < 0.1f)
            {
                currentAngle = targetAngle;
                isRotating = false;
            }
        }

        // Масштабирование колесиком мыши (нелинейно)
        if (mouse != null)
        {
            float scroll = mouse.scroll.y.ReadValue();
            if (Mathf.Abs(scroll) > 0.01f)
            {
                distance *= Mathf.Pow(zoomSpeed, -scroll); // экспоненциальное масштабирование
                distance = Mathf.Clamp(distance, minDistance, maxDistance);
            }
        }

        // Вычисляем позицию камеры
        Vector3 offset = new Vector3(Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0.5f, Mathf.Cos(currentAngle * Mathf.Deg2Rad)) * distance;
        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * 0.5f);
    }
}