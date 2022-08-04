using System.Collections.Generic;
using UnityEngine;

namespace Quests {
    public class QuestManager : MonoBehaviour {

        public static List<Quest> activeQuests;
        public static List<Quest> completedQuests;

        /// <summary>
        /// Adds the quest to ActiveQuests
        /// </summary>
        /// <param name="quest">The Quest</param>
        public void AddToActive(Quest quest)
        {
            activeQuests.Add(quest);
        }

        /// <summary>
        /// Remove the quest to ActiveQuests
        /// </summary>
        /// <param name="quest">The Quest</param>
        public void RemoveFromActive(Quest quest)
        {
            activeQuests.Remove(quest);
        }

        /// <summary>
        /// Adds the quest to CompletedQuests
        /// </summary>
        /// <param name="quest">The Quest</param>
        public void AddToComplete(Quest quest)
        {
            completedQuests.Add(quest);
        }

        /// <summary>
        /// Removes the quest to CompletedQuests
        /// </summary>
        /// <param name="quest">The Quest</param>
        public void RemoveFromComplete(Quest quest)
        {
            completedQuests.Remove(quest);
        }

        /// <summary>
        /// Returns whether or not the quest has been completed
        /// </summary>
        /// <param name="quest">The Quest</param>
        public bool HasCompletedQuest(Quest quest)
        {
            foreach ( Quest complete in completedQuests ) {
                if ( complete.questName == quest.questName ) {
                    return true;
                }
            }
            return false;

        }
    }
}
