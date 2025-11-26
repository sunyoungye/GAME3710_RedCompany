using UnityEngine;

public class PlayerMovem : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    public static GameObject Grabbable;
    public Transform Grabarea;
    public Transform Item_find;
    [SerializeField] private LayerMask interactMask;
    [SerializeField] private Transform camTransform;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (camTransform == null && Camera.main != null)
            camTransform = Camera.main.transform;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
        {
            if (Grabbable == null) Grab();
            else Drop();
        }
    }

    public void Grab()
    {
        if (Grabbable != null) return;
        if (camTransform == null) return;

        Vector3 origin = Item_find != null ? Item_find.position : camTransform.position;
        Vector3 dir = camTransform.forward;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, 5.0f, interactMask))
        {
            Grabbable = hit.transform.gameObject;
            Grabbable.transform.SetParent(Grabarea, true);
            Grabbable.transform.localPosition = Vector3.zero;
            Grabbable.transform.localRotation = Quaternion.identity;
        }
    }

    public void Drop()
    {
        if (Grabbable == null) return;

        Grabbable.transform.SetParent(null, true);
        Grabbable = null;
    }

    public bool HasEquipped()
    {
        return Grabbable != null &&
               Grabbable.layer == LayerMask.NameToLayer("Mop");
    }

}
