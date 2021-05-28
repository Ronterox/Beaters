using System.Collections.Generic;
using Plugins.Tools;
using TMPro;
using UnityEngine;

namespace Utilities
{
    public class DialoguesMessage : MonoBehaviour
    {
        public TMP_Text dialogueText;
        [TextArea]
        public string[] dialogues;
        private readonly Queue<string> dialoguesQueue = new Queue<string>();

        private void Start()
        {
            dialogues.ForEach(dialoguesQueue.Enqueue);
            SeeNextDialogue();
        }

        public void AddDialogues(params string[] texts) => texts.ForEach(dialoguesQueue.Enqueue);

        public void SeeNextDialogue()
        {
            if (dialoguesQueue.Count < 1) gameObject.SetActive(false);
            else
            {
                string nextDialogue = dialoguesQueue.Dequeue();
                dialogueText.text = nextDialogue;
            }
        }
    }
}
