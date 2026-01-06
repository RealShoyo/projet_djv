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
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

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
                    if (npc.transform.childCount > 0)
                    {
                        Renderer npcRenderer = npc.transform.GetChild(0).GetComponent<Renderer>();
                        if (npcRenderer != null) btnText.color = npcRenderer.material.color;
                    }
                }

                Button btn = btnObj.GetComponent<Button>();


                GameObject npcToEject = npc.gameObject;

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => gameManager.EjectNPC(npcToEject));

            }
        }
    }
}
