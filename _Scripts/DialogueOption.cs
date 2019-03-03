using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Karma
{
    GOOD,
    NEUTRAL,
    BAD
};

[CreateAssetMenu(menuName = "DialogueOption")]
public class DialogueOption : ScriptableObject
{
    public string line;
    public AudioClip lineAudio;
    public Karma karma;

    public DialogueOption[] responses;
};
