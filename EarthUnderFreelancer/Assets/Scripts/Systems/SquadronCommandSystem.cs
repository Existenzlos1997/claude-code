using UnityEngine;

namespace EarthUnderFreelancer.Systems
{
    /// <summary>
    /// Squadron command system for leading AI wingmen
    /// Issue orders to squadron members
    /// </summary>
    public class SquadronCommandSystem : MonoBehaviour
    {
        public enum CommandType
        {
            FormUp,
            Attack,
            Defend,
            Retreat,
            CoverMe,
            BreakAndAttack,
            HoldPosition
        }
        
        [Header("Squadron Settings")]
        [SerializeField] private int maxSquadronSize = 4;
        [SerializeField] private float commandCooldown = 2f;
        
        private Transform[] squadronMembers;
        private int squadronSize = 0;
        private float lastCommandTime = 0f;
        private CommandType lastCommand = CommandType.FormUp;
        
        private void Start()
        {
            squadronMembers = new Transform[maxSquadronSize];
        }
        
        private void Update()
        {
            HandleCommandInput();
        }
        
        private void HandleCommandInput()
        {
            if (Time.time < lastCommandTime + commandCooldown) return;
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                IssueCommand(CommandType.FormUp);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                IssueCommand(CommandType.Attack);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                IssueCommand(CommandType.Defend);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                IssueCommand(CommandType.Retreat);
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                IssueCommand(CommandType.CoverMe);
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                IssueCommand(CommandType.BreakAndAttack);
            }
        }
        
        public void IssueCommand(CommandType command)
        {
            lastCommand = command;
            lastCommandTime = Time.time;
            
            Debug.Log($"[Squadron] Command issued: {command}");
            
            // Apply command to all squadron members
            for (int i = 0; i < squadronSize; i++)
            {
                if (squadronMembers[i] != null)
                {
                    ApplyCommandToMember(squadronMembers[i], command);
                }
            }
        }
        
        private void ApplyCommandToMember(Transform member, CommandType command)
        {
            // Get AI behavior component
            var ai = member.GetComponent<AI.EnhancedAIBehavior>();
            if (ai == null) return;
            
            switch (command)
            {
                case CommandType.FormUp:
                    ai.JoinFormation(transform, GetMemberIndex(member), 
                        AI.EnhancedAIBehavior.FormationType.VFormation);
                    break;
                    
                case CommandType.Attack:
                    ai.BreakFormation();
                    // Would trigger attack behavior
                    break;
                    
                case CommandType.Defend:
                    ai.JoinFormation(transform, GetMemberIndex(member), 
                        AI.EnhancedAIBehavior.FormationType.LineAbreast);
                    break;
                    
                case CommandType.Retreat:
                    ai.BreakFormation();
                    // Would trigger retreat behavior
                    break;
                    
                case CommandType.CoverMe:
                    ai.JoinFormation(transform, GetMemberIndex(member), 
                        AI.EnhancedAIBehavior.FormationType.Trail);
                    break;
                    
                case CommandType.BreakAndAttack:
                    ai.BreakFormation();
                    ai.PerformEvasiveManeuver();
                    break;
            }
        }
        
        public bool AddSquadronMember(Transform member)
        {
            if (squadronSize >= maxSquadronSize) return false;
            
            squadronMembers[squadronSize] = member;
            squadronSize++;
            
            Debug.Log($"[Squadron] Added member: {member.name} ({squadronSize}/{maxSquadronSize})");
            return true;
        }
        
        public void RemoveSquadronMember(Transform member)
        {
            for (int i = 0; i < squadronSize; i++)
            {
                if (squadronMembers[i] == member)
                {
                    // Shift array
                    for (int j = i; j < squadronSize - 1; j++)
                    {
                        squadronMembers[j] = squadronMembers[j + 1];
                    }
                    squadronMembers[squadronSize - 1] = null;
                    squadronSize--;
                    
                    Debug.Log($"[Squadron] Removed member: {member.name}");
                    break;
                }
            }
        }
        
        private int GetMemberIndex(Transform member)
        {
            for (int i = 0; i < squadronSize; i++)
            {
                if (squadronMembers[i] == member) return i;
            }
            return 0;
        }
        
        public int GetSquadronSize() => squadronSize;
        public CommandType GetLastCommand() => lastCommand;
        
        private void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 14;
            
            float y = 300f;
            GUI.Label(new Rect(20, y, 300, 20), $"Squadron: {squadronSize}/{maxSquadronSize}", style);
            y += 25f;
            GUI.Label(new Rect(20, y, 300, 20), $"Last Order: {lastCommand}", style);
            y += 25f;
            GUI.Label(new Rect(20, y, 300, 20), "F=Form, A=Attack, D=Defend", style);
            y += 20f;
            GUI.Label(new Rect(20, y, 300, 20), "R=Retreat, C=Cover, B=Break", style);
        }
    }
}
