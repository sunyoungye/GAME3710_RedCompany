using UnityEngine;

public class Cleanupspill : MonoBehaviour
{
    // for the UI
    [SerializeField] private Quest quest;
    [SerializeField] private int stepIndexToComplete = 0;

    private bool playerInside = false;
    private PlayerMovem player; // reference to the player inside


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player = other.GetComponent<PlayerMovem>(); // grab reference
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            player = null;
        }
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"E pressed, player = {player}");

            if (player != null)
            {
                bool has = player.HasEquipped();
                Debug.Log("HasEquipped() = " + has);

                if (has)
                {
                    Debug.Log("Spill cleaned!");
                    quest.CompleteStep(stepIndexToComplete);
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("You need a mop to clean this!");
                }
            }
            else
            {
                Debug.Log("player is null");
            }
        }
    }

}