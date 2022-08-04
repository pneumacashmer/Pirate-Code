using System.Collections.Generic;
using UnityEngine;

namespace Quests {
    public class QuestGiver : Interactable {
        [Tooltip("The quest that this character gives")]
        public Quest quest;
        [Tooltip("The manager for handling quests")]
        public QuestManager questManager;
        [Tooltip("The quests needed to be completed before the quest can start")]
        public List<Quest> neededQuestsBeforeStart;
        private CutsceneManager _cm;
        [Tooltip("The eventlist of starting the Quest")]
        public EventList startingCutscene;
        [Tooltip("The eventlist of the dialogue done while the Quest is active")]
        public EventList doingQuestCutscene;
        [Tooltip("The eventlist of turning in the Quest")]
        public EventList turnInQuestCutscene;
        [Tooltip("The eventlist of the dialogue said post Quest")]
        public EventList completedQuestCutscene;
        [Tooltip("The default dialogue that the NPC/Object has")]
        public EventList defaultDialogue;
        private bool neededQuestsCompleted;

        private void Start()
        {
            _cm = FindObjectOfType<CutsceneManager>();
            questManager = FindObjectOfType<QuestManager>();

        }

        /// <summary>
        /// Checks and runs which cutscene information is needed
        /// </summary>
        public override void Interact()
        {
            foreach ( Quest needed in neededQuestsBeforeStart ) {
                if ( questManager.HasCompletedQuest(needed) ) {
                    neededQuestsCompleted = true;
                    break;
                }
            }

            if ( neededQuestsCompleted && quest != null ) {
                if ( !quest.isActive && !quest.isComplete ) {
                    _cm.StartSequence(startingCutscene.information);
                    //TODO Yes No Option for Cutscenes
                    quest.isActive = true;
                    questManager.AddToActive(quest);
                }
                //This happens if the quest is active but not complete.  Quest gets called to complete from the more specific scripts.
                else if ( quest.isActive && !quest.isComplete ) {
                    _cm.StartSequence(doingQuestCutscene.information);
                }
                //This plays when the quest gets turned in
                else if ( quest.isActive && quest.isComplete ) {
                    _cm.StartSequence(turnInQuestCutscene.information);
                    quest.isActive = false;
                    questManager.AddToComplete(quest);
                    questManager.RemoveFromActive(quest);
                }
                //This plays when the quest is completed and the player talks to the NPC again
                else if ( quest.isComplete && !quest.isActive ) {
                    _cm.StartSequence(completedQuestCutscene.information);
                }
            }
            //The NPC has no quest
            else {
                _cm.StartSequence(defaultDialogue.information);
                return;
            }
        }
    }
}
