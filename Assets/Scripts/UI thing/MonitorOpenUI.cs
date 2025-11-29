using UnityEngine;

public class MonitorOpenUI : MonoBehaviour
{
    [SerializeField] private Canvas monitorCanvas;

    private void Start()
    {
        if (monitorCanvas != null)
            monitorCanvas.enabled = false;   // 처음엔 꺼두기
    }

    // 이 오브젝트를 클릭했을 때 자동으로 호출됨 (Collider 필수)
    private void OnMouseDown()
    {
        if (monitorCanvas != null)
            monitorCanvas.enabled = true;    // UI 켜기

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Monitor clicked → UI ON");
    }
}
