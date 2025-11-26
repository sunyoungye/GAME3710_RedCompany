using UnityEngine;

public class PA_Script : MonoBehaviour
{
AudioSource Muzak;
AudioSource Announcements;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
          AudioSource[] audios = GetComponents<AudioSource>();
    Muzak = audios[0];
    Announcements = audios[1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
