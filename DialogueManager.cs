using System.Collections; 
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[System.Serializable]
public class EntrySplit
{
    //this two must match - meaning 2 subtitles needs 2 durations

    [Header("duration index must be equal to subtitle index")]
    public string[] subtitles;
    public float[] durations;
}

[System.Serializable]
public class EntryDialogue
{
    public EntrySplit entrySplit;
    public AudioClip audioClip;
}

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI subtitleText;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private EntryDialogue[] dialogues;
    private int currentDialogueIndex = 0;

    [SerializeField]
    private int durationBetweenDialogue = 1;
    private bool isDialogueActive = false;

    private void Start()
    {
        if (!isDialogueActive)
        {
            StartCoroutine(DialogueSequence());
        }
    }

    private IEnumerator DialogueSequence()
    {
        isDialogueActive = true;
        subtitleText.gameObject.SetActive(true);

        foreach (var dialogue in dialogues)
        {
            if (dialogue.entrySplit.subtitles.Length != dialogue.entrySplit.durations.Length)
            {
                Debug.LogError(
                    $"Subtitle index and dialogue index are not equal {currentDialogueIndex}"
                );
                continue;
            }

            for (int i = 0; i < dialogue.entrySplit.subtitles.Length; i++)
            {
                subtitleText.text = dialogue.entrySplit.subtitles[i];
                audioSource.clip = dialogue.audioClip;
                audioSource.Play();
                yield return new WaitForSeconds(dialogue.entrySplit.durations[i]);
            }

            yield return new WaitForSeconds(durationBetweenDialogue);
        }

        subtitleText.gameObject.SetActive(false);
        isDialogueActive = false;
        currentDialogueIndex++;
    }

    public void SkipDialogue()
    {
        if (isDialogueActive)
        {
            StopCoroutine(DialogueSequence());
            FinishDialogue();
        }
    }

    private void FinishDialogue()
    {
        subtitleText.gameObject.SetActive(false);
        isDialogueActive = false;
        audioSource.Stop();
    }

    private void OnDestroy()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
