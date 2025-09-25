using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contains_Item : MonoBehaviour
{
    public List<GameObject> colliderList = new List<GameObject>();
    public static int Itemsstored = 0;
    public void OnTriggerEnter(Collider collider)
    {
        if (!colliderList.Contains(collider.gameObject))
        {
            colliderList.Add(collider.gameObject);
            Itemsstored +=1;
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if(colliderList.Contains(collider.gameObject))
        {
            colliderList.Remove(collider.gameObject);
            Itemsstored -=1;
        }
    }
}

