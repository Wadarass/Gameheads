using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GH;

public class DialogueUIOption : GH.Event
{
    public DialogueOption line;
    public UnityAction[] onclickEvent;

    int[] numbers;
}

public class ShowSpokenLine : GH.Event
{
    public string line;
}

public class HideUI : GH.Event
{

}

public class DialogueSystem : MonoBehaviour
{
    public DialogueDatabase database;
    public float m_dialogueWaitTime = 0.5f;

    private float m_currentTime = 0.0f;
    private bool m_timerStarted = false;

    AudioSource source;
    Dictionary<string, DialogueOption> dialogueTable = new Dictionary<string, DialogueOption>();

    DialogueOption m_currentLine = null;

    void Awake()
    {
        LoadDatabase(database);
        source = GetComponent<AudioSource>();
    }

    public void LoadDatabase(DialogueDatabase newDatabase)
    {
        database = newDatabase;

        //dialogueTable.Clear();

        foreach (DialogueOption option in database.dialogueLines)
        {
            dialogueTable.Add(option.name, option);
        }
    }

    void Update()
    {
        if (m_timerStarted)
        {
            m_currentTime += Time.deltaTime;

            if (m_currentTime >= m_dialogueWaitTime)
            {
                m_currentTime = 0.0f;
                m_timerStarted = false;

                if (m_currentLine.responses.Length > 0)
                {
                    DialogueUIOption data = new DialogueUIOption();
                    data.line = option;
                    data.onclickEvent = new UnityAction[m_currentLine.responses.Length];

                    int index = 0;
                    foreach (DialogueOption response in option.responses)
                    {
                        int currentIndex = index;
                        data.onclickEvent[index] += () => { this.NextLine(currentIndex); };
                        index++;
                    }

                    GH.EventSystem.instance.RaiseEvent(data);
                }
                else
                {
                    GH.EventSystem.RaiseEvent(new HideUI() );
                }
            }
        }
    }

    void StartLine(DialogueOption option)
    {
        if (option != null)
        {
            m_currentLine = option;

            if (m_currentLine.lineAudio != null)
            {
                source.PlayOneShot(m_currentLine.lineAudio);
                m_dialogueWaitTime = m_currentLine.lineAudio.length;
            }

            GH.EventSystem.RaiseEvent(new ShowSpokenLine { line = m_currentLine.line } );
            
            m_timerStarted = true;
            m_currentTime = 0.0f;
                        
        }
    }

    public void StartLine(string dialogueLineName)
    {
        DialogueOption option = null;
        dialogueTable.TryGetValue(dialogueLineName, out option);

        StartLine(option);
    }

    public void NextLine(int lineIndex)
    {
        GH.EventSystem.RaiseEvent(new HideUI() );
        Debug.Log(string.Format("Next Line called: {0}", lineIndex));
        if (lineIndex < m_currentLine.responses.Length)
        {
            StartLine(m_currentLine.responses[lineIndex]);
        }
    }
}
