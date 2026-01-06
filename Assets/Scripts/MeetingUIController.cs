using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MeetingUIController : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private GameObject buttonPrefab;  
    [SerializeField] private Transform gridParent;     
    [SerializeField] private GameManager gameManager;

    public void SetupMeetingButtons()
    {

        foreach (Transform child in gridParent) Destroy(child.gameObject);

        NPC[] allNPCs = FindObjectsOfType<NPC>();

        foreach (NPC npc in allNPCs)
        {
     
            if (npc != null && !npc.isKilled)
            {
                GameObject btnObj = Instantiate(buttonPrefab, gridParent);
                btnObj.SetActive(true);

         
                TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                {
                    btnText.text = npc.name;
 
                    Renderer rend = npc.GetComponentInChildren<Renderer>();
                    if (rend != null) btnText.color = rend.material.color;
                }

                Button btn = btnObj.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();

    
                NPC capturedNPC = npc; 
                btn.onClick.AddListener(delegate { gameManager.EjectNPC(capturedNPC); });
            }
        }
    }
}
