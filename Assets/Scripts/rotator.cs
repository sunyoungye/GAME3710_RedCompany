using UnityEngine;

public class rotator : MonoBehaviour
{
    public Vector2 turn;
    public float sensivity = .5f;

    // Update is called once per frame
    void Update()
    {
        turn.y += Input.GetAxis("Mouse Y") * sensivity;
        turn.x += Input.GetAxis("Mouse X") * sensivity;
        transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }
}
