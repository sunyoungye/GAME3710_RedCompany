using UnityEngine;

public class Cleanupspill : MonoBehaviour
{
    [Header("Quest Settings")]
    [SerializeField] private Quest quest;
    [SerializeField] private int stepIndexToComplete = 0;

    private bool playerInside = false;
    private PlayerMovem player; // reference to the player

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger ENTER by: " + other.name);

        // Check if this object OR parent has PlayerMovem component
        player = other.GetComponent<PlayerMovem>();
        if (player == null)
            player = other.GetComponentInParent<PlayerMovem>();

        if (player != null)
        {
            playerInside = true;
            Debug.Log("Player detected: " + player.name);
        }
        else
        {
            Debug.Log("Non-player entered trigger.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Reset player reference if leaving
        PlayerMovem exitingPlayer = other.GetComponent<PlayerMovem>();
        if (exitingPlayer == null)
            exitingPlayer = other.GetComponentInParent<PlayerMovem>();

        if (exitingPlayer != null && exitingPlayer == player)
        {
            playerInside = false;
            player = null;
            Debug.Log("Player exited trigger.");
        }
    }

    void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (player == null)
            {
                Debug.LogError("Player is null even though inside trigger!");
                return;
            }

            if (player.HasEquipped())
            {
                Debug.Log("Spill cleaned!");
                if (quest != null)
                    quest.CompleteStep(stepIndexToComplete);
                else
                    Debug.LogWarning("Quest reference not set on spill.");

                Destroy(gameObject);
            }
            else
            {
                Debug.Log("You need a mop to clean this!");
            }
        }
    }
}