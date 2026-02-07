using UnityEngine;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    {
        public string speakerName;
        public string text;
        public List<DialogueChoice> choices = new List<DialogueChoice>();
    }
    
    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public string nextNodeId;
        public System.Action onSelected;
    }
    
    public class DialogueSystem : MonoBehaviour
    {
        private static DialogueSystem instance;
        public static DialogueSystem Instance => instance;
        
        public Dictionary<string, DialogueNode> dialogueDatabase = new Dictionary<string, DialogueNode>();
        private DialogueNode currentNode;
        public System.Action<DialogueNode> onDialogueUpdate;
        
        private void Awake() { if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); } }
        
        public void StartDialogue(string nodeId)
        {
            if (dialogueDatabase.TryGetValue(nodeId, out DialogueNode node))
            {
                currentNode = node;
                onDialogueUpdate?.Invoke(currentNode);
            }
        }
        
        public void SelectChoice(int choiceIndex)
        {
            if (currentNode != null && choiceIndex < currentNode.choices.Count)
            {
                DialogueChoice choice = currentNode.choices[choiceIndex];
                choice.onSelected?.Invoke();
                if (!string.IsNullOrEmpty(choice.nextNodeId))
                    StartDialogue(choice.nextNodeId);
                else
                    EndDialogue();
            }
        }
        
        public void EndDialogue() { currentNode = null; onDialogueUpdate?.Invoke(null); }
        
        public void RegisterDialogue(string id, DialogueNode node) { dialogueDatabase[id] = node; }
    }
}
