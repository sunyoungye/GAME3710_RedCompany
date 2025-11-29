using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    LayerMask layerMask;
    public Transform Grabarea;

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Player", "Item");
    }

    private void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && PlayerMovem.Grabbable == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position,
                                transform.forward,
                                out hit,
                                5f,
                                layerMask))
            {
                PlayerMovem.Grabbable = hit.transform.gameObject;
                PlayerMovem.Grabbable.transform.SetParent(Grabarea, true);
                PlayerMovem.Grabbable.transform.localPosition = Vector3.zero;
                PlayerMovem.Grabbable.transform.localRotation = Quaternion.identity;
            }
        }

        if (Input.GetMouseButtonDown(1) && PlayerMovem.Grabbable != null)
        {
            PlayerMovem.Grabbable.transform.SetParent(null, true);
            PlayerMovem.Grabbable = null;
        }
    }
}
