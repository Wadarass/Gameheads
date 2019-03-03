using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct DialogueButtonData
{
    GameObject button;
    Button unityButton;
    TMPro.TextMeshProUGUI buttonText;
};

public class DialogueUI : MonoBehaviour
{
    public GameObject buttonPrefab;

    private Queue<DialogueButtonData> createdButtons = new Queue<DialogueButtonData>();
    private List<DialogueButtonData> activeButtons = new List<DialogueButtonData>(4);

    public TMPro.TextMeshProUGUI lineUI;

    private const int numButtons = 4;

    void Awake()
    {
        if (buttonPrefab)
        {
            for (i = 0; i < numButtons; i++)
            {
                GameObject button = GameObject.Instantiate(buttonPrefab, gameObject.transform);
                button.SetActive(false);

                DialogueButtonData data = new DialogueButtonData();
                data.button = button;
                data.unityButton = button.GetComponent<Button>();
                data.buttonText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();

                createdButtons.Enqueue(data);
            }
        }

        if (lineUI != null)
        {
            lineUI.SetActive(false);
        }
    }

    private void OnEnable()
    {
        GH.EventSystem.instance.AddListener<DialogueUIOption>(ShowUI);
        GH.EventSystem.instance.AddListener<HideUI>(HideDialogueUI);
        GH.EventSystem.instance.AddListener<ShowSpokenLine>(ShowSpeakerLine);
    }

    private void OnDisable()
    {
        GH.EventSystem.instance.RemoveListener<DialogueUIOption>(ShowUI);
        GH.EventSystem.instance.RemoveListener<HideUI>(HideDialogueUI);
        GH.EventSystem.instance.RemoveListener<ShowSpokenLine>(ShowSpeakerLine);
    }

    private void ShowSpeakerLine(ShowSpokenLine data)
    {
        if (lineUI)
        {
            lineUI.SetText(data.line);
            lineUI.SetActive(true)
        }
    }

    private void HideDialogueUI(HideUI data)
    {
        if (lineUI)
        {
            lineUI.SetActive(false);
        }

        foreach(DialogueButtonData button in activeButtons)
        {
            button.button.SetActive(false);
            createdButtons.Enqueue(button);
        }

        activeButtons.Clear();
    }

    //SHOW DIALOGUE
    void ShowUI(DialogueUIOption option)
    {
        int index = 0;
        foreach (DialogueOption choice in option.line.responses)
        {
            DialogueButtonData button = createdButtons.Dequeue();
            button.buttonText.SetText(choice.line);

            button.unityButton.onClick.AddListener(option.onclickEvent[index]);
            button.button.SetActive(true);

            activeButtons.Add(button);
            index++;
        }
    }
}
