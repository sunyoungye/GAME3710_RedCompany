using UnityEngine;
using System.Collections;
public class PA_Script : MonoBehaviour
{
AudioSource Muzak;
AudioSource Announcements;
public float AudioTime = 0.00f;
public float MinAudioTime  = 60.00f;
public float MaxAudioTime  = 180.00f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
void SetTimer(){
AudioTime=Random.Range(MinAudioTime,MaxAudioTime);
}
    void Start()
    {
          AudioSource[] audios = GetComponents<AudioSource>();
    Muzak = audios[0];
    Announcements = audios[1];
    Muzak.Play();
    SetTimer();
    }
    // Update is called once per frame
    void Update()
    {
    AudioTime -= Time.deltaTime;
    if(AudioTime <= 0){
        Announcements.Play();
        SetTimer();
    }
    }
}
