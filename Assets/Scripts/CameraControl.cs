using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] public Vector3 offset;
    [SerializeField] public float height;

    private Quaternion targetRotation;

    public float yRotation;
    private float xRotation;
    private float xRotationClapmed;

    [SerializeField] public float yRotationMin;
    [SerializeField] public float yRotationMax;

    [SerializeField] private float xSensivity;
    [SerializeField] public float ySensivity;

    [SerializeField] private bool invertX;
    private int xInvertedValue;

    private Vector3 desiredPos;

    private void Start()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        xInvertedValue = invertX ? 1 : 1;
    }

    private void Update()
    {
        yRotation += Input.GetAxis("Mouse X") * ySensivity;
        xRotation += Input.GetAxis("Mouse Y") * xSensivity * xInvertedValue;
    }

    private void LateUpdate()
    {
        xRotationClapmed = Mathf.Clamp(yRotation, yRotationMin, yRotationMax);
        targetRotation = Quaternion.Euler(xRotation, yRotation, 0.0f);

        desiredPos = target.position - targetRotation * offset + Vector3.up * height;
        transform.SetPositionAndRotation(desiredPos, targetRotation);
    }

    //public Quaternion YYRotation { get { return Quaternion.Euler(0.0f, yRotation, 0.0f); } }

    public Quaternion YRotation => Quaternion.Euler(0.0f, xRotation, 0.0f);
}
