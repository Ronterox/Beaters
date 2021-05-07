using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{
    private static PlayableDirector director;
    private static GameObject controlPanel;

    void Awake(){
        director = GetComponent<PlayableDirector>();
        director.played +=Director_Played;
        director.stopped += Director_Stoped;
    }

    private void Director_Stoped(PlayableDirector obj){
         controlPanel.SetActive(true);
     }

     private void Director_Played(PlayableDirector obj){
         controlPanel.SetActive(false);
     }

     public static void StartTimeline(){
         director.Play();
     }
}
