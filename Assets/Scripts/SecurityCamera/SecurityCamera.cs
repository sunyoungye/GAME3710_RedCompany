using UnityEngine;
using UnityEngine.Events;

public class SecurityCamera : MonoBehaviour
{
    public UnityEvent OnPlayerDetected;
    public Material defaultMaterial;
    public Material detectedMaterial;
    public Transform eyePoint; 

    private Renderer coneRenderer;
    private Transform playerTransform;

    void Start()
    {
        coneRenderer = GetComponent<Renderer>();
        if (coneRenderer != null && defaultMaterial != null)
        {
            coneRenderer.material = defaultMaterial;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        // 
        if (eyePoint == null)
        {
            Debug.LogError("EyePoint Transform not assigned! Detection will not work.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (eyePoint == null || playerTransform == null) return;

        if (other.CompareTag("Player"))
        {
            if (CheckLineOfSight(playerTransform))
            {
                if (coneRenderer != null && detectedMaterial != null)
                {
                    coneRenderer.material = detectedMaterial;
                }
                OnPlayerDetected.Invoke();
            }
            else
            {
                if (coneRenderer != null && defaultMaterial != null)
                {
                    coneRenderer.material = defaultMaterial;
                }
            }
        }
    }

    private bool CheckLineOfSight(Transform target)
    {
        
        Vector3 directionToTarget = (target.position - eyePoint.position).normalized;
        float distanceToTarget = Vector3.Distance(eyePoint.position, target.position);
        RaycastHit hit;

        if (Physics.Raycast(eyePoint.position, directionToTarget, out hit, distanceToTarget))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                Debug.DrawRay(eyePoint.position, directionToTarget * distanceToTarget, Color.red);
                return false; 
            }
        }

        Debug.DrawRay(eyePoint.position, directionToTarget * distanceToTarget, Color.green);
        return true; 
    }
}
