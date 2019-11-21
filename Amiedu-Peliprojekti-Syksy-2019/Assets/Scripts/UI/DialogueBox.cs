using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueBox : MonoBehaviour, IUIHandler
{
    List<UIItem> items = new List<UIItem>();
    TextMeshProUGUI text;
    TextMeshProUGUI talkerName;
    public GameObject head;
    string currentText, textToShow;
    public Dialogue[] currentDialogue;
    Animator anim;
    int dialogueIndex;
    private float speed = 0.02f;
    bool textActivated;

    public void EntryClick(int index, PointerEventData.InputButton button)
    {
        if (button == PointerEventData.InputButton.Left) SkipDialogue();
    }

    public void EntryEnter(int index)
    {
        items[0].anim.SetBool("Hover", true);
    }

    public void EntryLeave(int index)
    {
        items[0].anim.SetBool("Hover", false);
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        text = transform.Find("Dialogue").GetComponent<TextMeshProUGUI>();
        talkerName = transform.Find("TalkerName").GetComponent<TextMeshProUGUI>();
        items.UItemInitialize(transform);
        talkerName.text = "";
        text.text = "";
        head = transform.GetFromAllChildren("Head").gameObject;
    }

    public void StartDialogue()
    {
        textActivated = true;
        dialogueIndex = 0;
        items[0].text.text = "Skip";
        text.text = "";

        talkerName.text = currentDialogue[0].talker;
        currentText = "";
        textToShow = currentDialogue[0].text;
        StartCoroutine("ShowText");
    }

    IEnumerator ShowText()
    {
        for (int i = 0; i < textToShow.Length; i++)
        {
            currentText += textToShow[i];
            text.text = currentText;
            yield return new WaitForSeconds(speed);
        }
        items[0].text.text = "Ok";
    }

    public void ProceedDialogue()
    {
        dialogueIndex++;
        if (dialogueIndex >= currentDialogue.Length)
        {
            Events.onDialogueBox = false;
            anim.SetTrigger("Disable");
            textActivated = false;
            return;
        }
        items[0].text.text = "Skip";
        currentText = "";
        talkerName.text = currentDialogue[dialogueIndex].talker;
        textToShow = currentDialogue[dialogueIndex].text;
        StartCoroutine("ShowText");
    }
    public void DisableDialogue()
    {
        talkerName.text = "";
        text.text = "";
        ObjectPooler.op.DeSpawn(gameObject);
    }
    public void SkipDialogue()
    {
        if (!textActivated) return;
        if (items[0].text.text == "Skip")
        {
            StopCoroutine("ShowText");
            text.text = textToShow;
            items[0].text.text = "Ok";
        }
        else
        {
            ProceedDialogue();
        }
    }
}
public struct Dialogue
{
    public GameObject head;
    public string talker;
    public string text;
}