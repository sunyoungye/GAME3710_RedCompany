using UnityEngine;

public class PlayerController : MonoBehaviour
{
LayerMask layerMask;
public static GameObject Grabbable;
public Transform Grabarea;
    private CharacterController _characterController;

    public float MovementSpeed = 10f, RotationSpeed = 5f, JumpForce = 10f, Gravity = -30f;

    private float _rotationY;
    private float _verticalVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _characterController = GetComponent<CharacterController>();   
    }
void Awake()
{
layerMask=LayerMask.GetMask("Item","Player");
}

    public void Move(Vector2 movementVector)
    {
        Vector3 move = transform.forward * movementVector.y + transform.right * movementVector.x;
        move = move * MovementSpeed * Time.deltaTime;
        _characterController.Move(move);

        _verticalVelocity = _verticalVelocity + Gravity * Time.deltaTime;
        _characterController.Move(new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);
    }

    public void Rotate(Vector2 rotationVector)
    {
        _rotationY += rotationVector.x * RotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, _rotationY, 0);
    }

    public void Jump()
    {
        if (_characterController.isGrounded)
        {
            _verticalVelocity = JumpForce;
        }
    }
    public void Grab()
    {

     RaycastHit hit;
     if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)&& Grabbable == null) 
        {
     Grabbable = hit.transform.gameObject;
     Grabbable.transform.SetParent(Grabarea);
print(Grabbable);
        }
     }
    public void Drop(){
    if(Grabbable != null){
        Grabbable.transform.parent=null;
        Grabbable = null;
        }
}
}
