using UnityEngine;

public class StockMilk : MonoBehaviour
{
    [SerializeField] private Quest quest;
    [SerializeField] private int stepIndexToComplete = 3;

    [Header("Milk Cartons to Activate (in order)")]
    public GameObject[] milkSlots;

    [Header("Crate Settings")]
    public string crateName = "MilkCrate";
    public string crateTag = "Crate";

    [Header("Extra Objects to Destroy After Stocking")]
    public GameObject[] extraObjects;

    [Header("Object to Show After Stocking")]
    public GameObject objectToShow;

    private bool playerInside = false;
    private GameObject crateInside = null;

    void Start()
    {
        // Hide all milk slots initially
        foreach (GameObject slot in milkSlots)
            slot.SetActive(false);

        // Hide the object to show at the start
        if (objectToShow != null)
            objectToShow.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;

        if ((other.CompareTag(crateTag) || other.name == crateName))
            crateInside = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;

        if (crateInside != null && other.gameObject == crateInside)
            crateInside = null;
    }

    void Update()
    {
        if (!playerInside || crateInside == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Stock the first empty milk slot
            foreach (GameObject slot in milkSlots)
            {
                if (!slot.activeSelf)
                {
                    slot.SetActive(true);
                    break; // only one per press
                }
            }

            // Check if all milk slots are full
            bool allFull = true;
            foreach (GameObject slot in milkSlots)
                if (!slot.activeSelf) allFull = false;

            if (allFull && crateInside != null)
            {
                if (quest != null)
                    quest.CompleteStep(stepIndexToComplete);
                else
                    Debug.LogWarning("Look at the Quest!.");

                // Destroy crate
                Destroy(crateInside);
                crateInside = null;

                // Destroy extra objects
                foreach (GameObject obj in extraObjects)
                    if (obj != null)
                        Destroy(obj);

                // Show the special object
                if (objectToShow != null)
                    objectToShow.SetActive(true);

                Debug.Log("All milk stocked!");
            }
        }
    }
}

