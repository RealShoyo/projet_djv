using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool gameEnded = false;
    void Start()
    {
        AssignImpostors();

        StartCoroutine(CheckGameStatusRoutine());
    }

    IEnumerator CheckGameStatusRoutine()
    {
        while (!gameEnded)
        {
            VerifyWinLossConditions();
            yield return new WaitForSeconds(1f); // Vérifie toutes les secondes
        }
    }

    void VerifyWinLossConditions()
    {
        NPC[] allNPCs = FindObjectsOfType<NPC>();

        int impostorsAlive = 0;
        int crewmatesAlive = 0;

        foreach (NPC npc in allNPCs)
        {
            if (!npc.isKilled)
            {
                if (npc.isImpostor) impostorsAlive++;
                else crewmatesAlive++;
            }
        }


        crewmatesAlive += 1;


        if (impostorsAlive == 0)
        {
            EndGame(2); //WIN
        }

        else if (impostorsAlive >= crewmatesAlive)
        {
            EndGame(3); //LOOSE
        }
    }

    void EndGame(int scene)
    {
        gameEnded = true;
        SceneManager.LoadSceneAsync(scene);
    }

    void AssignImpostors()
    {
        
        NPC[] allNPCs = FindObjectsOfType<NPC>();

        
        List<NPC> npcList = new List<NPC>(allNPCs);

        
        if (npcList.Count < 2)
        {
            Debug.LogWarning("Pas assez de NPCs pour désigner 2 imposteurs !");
            return;
        }

        
        int index1 = Random.Range(0, npcList.Count);
        npcList[index1].isImpostor = true;
        Debug.Log(npcList[index1].name + " est un Imposteur !");

        
        npcList.RemoveAt(index1);

        
        int index2 = Random.Range(0, npcList.Count);
        npcList[index2].isImpostor = true;
        Debug.Log(npcList[index2].name + " est un Imposteur !");
    }



    public void EjectNPC(GameObject npcObject) 
    {
        if (npcObject == null) return;

    
        NPC npcScript = npcObject.GetComponent<NPC>();

        if (npcScript != null)
        {
            Debug.Log("<color=yellow>ÉJECTION : </color>" + npcScript.name + " a été éjecté !");


            npcScript.isKilled = true;

  
            Destroy(npcObject);

   
            MeetingManager meeting = FindObjectOfType<MeetingManager>();
            if (meeting != null)
            {
                meeting.ResumeGame();
            }
        }
    }

    public void SkipVote()
    {
        Debug.Log("Le vote a été passé (Skip). Personne n'a été éjecté.");
        MeetingManager meeting = FindObjectOfType<MeetingManager>();
        if (meeting != null)
        {
            meeting.ResumeGame();
        }
    }
}
