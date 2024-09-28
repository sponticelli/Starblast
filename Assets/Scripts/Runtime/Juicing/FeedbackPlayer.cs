using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Starblast.Juicing
{
    public class FeedbackPlayer : MonoBehaviour
    {
        [SerializeField] private List<Feedback> feedbacks = null;
        
        
        public void PlayFeedbacks()
        {
            FinishFeedbacks();
            foreach (var feedback in feedbacks)
            {
                feedback.CreateFeedback();
            }
        }

        private void FinishFeedbacks()
        {
            foreach (var feedback in feedbacks)
            {
                feedback.CompletePreviousFeedback();
            }
        }
    }
}