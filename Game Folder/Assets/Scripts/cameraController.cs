using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;

    float camRotX;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!gamemanager.instance.isPaused)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * sens;
            float mouseY = Input.GetAxisRaw("Mouse Y") * sens;

            camRotX -= mouseY;

            camRotX = Mathf.Clamp(camRotX, lockVertMin, lockVertMax);
            transform.localRotation = Quaternion.Euler(camRotX, 0, 0);

            transform.parent.Rotate(Vector3.up * mouseX);
        }
    }
}