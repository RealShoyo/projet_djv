using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeetingManager : MonoBehaviour
{
    [SerializeField] private GameObject meetingCanvas;
    [SerializeField] private GameObject player;
    [SerializeField] private MonoBehaviour playerMovementScript;
    [SerializeField] private MeetingUIController uiController;

    private Vector3 playerStartPos;
    private Dictionary<NPC, Vector3> npcStartPositions = new Dictionary<NPC, Vector3>();
    private bool isMeetingActive = false;

    void Start()
    {

        if (player != null) playerStartPos = player.transform.position;


        NPC[] allNPCs = FindObjectsOfType<NPC>();
        foreach (NPC npc in allNPCs)
        {
            npcStartPositions.Add(npc, npc.transform.position);
        }
    }

    public void StartEmergencyMeeting()
    {
        if (isMeetingActive) return;

        NPC[] allNPCsInScene = FindObjectsOfType<NPC>();
        foreach (NPC npc in allNPCsInScene)
        {
            if (npc != null && npc.isKilled)
            {
                Destroy(npc.gameObject);
            }
        }

        List<NPC> toRemove = new List<NPC>();
        foreach (var npc in npcStartPositions.Keys)
        {
            if (npc == null) toRemove.Add(npc); // Si l'objet a été Destroy()
        }
        foreach (var npc in toRemove) npcStartPositions.Remove(npc);

        if (uiController != null) uiController.SetupMeetingButtons();

        isMeetingActive = true;


        if (playerMovementScript != null) playerMovementScript.enabled = false;
        player.transform.position = playerStartPos;


        foreach (KeyValuePair<NPC, Vector3> entry in npcStartPositions)
        {
            NPC npc = entry.Key;
            Vector3 startPos = entry.Value;

            if (npc != null && !npc.isKilled)
            {

                NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.isStopped = true;
                    agent.enabled = false;
                }


                npc.transform.position = startPos;
                npc.enabled = false;
            }
        }


        if (meetingCanvas != null)
        {
            meetingCanvas.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        isMeetingActive = false;


        if (playerMovementScript != null) playerMovementScript.enabled = true;


        StartCoroutine(RestartNPCsRoutine());

        if (meetingCanvas != null)
        {
            meetingCanvas.SetActive(false);

        }
    }

    private IEnumerator RestartNPCsRoutine()
    {

        yield return new WaitForEndOfFrame();

        foreach (NPC npc in npcStartPositions.Keys)
        {

            if (npc != null && !npc.isKilled)
            {
                npc.enabled = true;
                NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();

                if (agent != null)
                {
                    agent.enabled = true;


                    yield return null;

                    if (agent.isOnNavMesh)
                    {
                        agent.isStopped = false;
                        npc.CancelInvoke("GoToRandomTask");
                        npc.GoToRandomTask();
                    }
                }
            }
        }
    }
}
