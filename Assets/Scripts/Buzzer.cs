using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buzzer : MonoBehaviour
{
    [SerializeField] private MeetingManager meetingManager;
    private bool isPlayerInside = false;

    void Update()
    {

        if (isPlayerInside && Input.GetKeyDown(KeyCode.Space))
        {
            if (meetingManager != null)
            {
                meetingManager.StartEmergencyMeeting();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            Debug.Log("Appuie sur ESPACE pour lancer la réunion !");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}
