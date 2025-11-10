using UnityEngine;

public class Cleanupspill : MonoBehaviour
{
    private bool playerInside = false;
    private PlayerController player; // reference to the player inside


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player = other.GetComponent<PlayerController>(); // grab reference
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
        if (playerInside && Input.GetKeyDown(KeyCode.F))
        {
            if (player != null && player.HasEquipped()) // ? only if mop equipped
            {
                Debug.Log("Spill cleaned!");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("You need a mop to clean this!");
            }
        }
    }
}