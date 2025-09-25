using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Grabable : MonoBehaviour
{
    private Rigidbody rb;
    public Rigidbody Rb => rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
}
