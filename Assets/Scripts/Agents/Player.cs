using System;
using System.Collections;
using System.Collections.Generic;
using Starblast.Inputs;
using UnityEngine;

namespace Starblast.Agents
{
    public class Player : MonoBehaviour
    {
       [SerializeField]
       private AgentMover _agentMover;
       
       [SerializeField]
       private PlayerInputSpace _agentInput;
       
       
       private void OnEnable()
       {
           _agentInput.OnRotate.AddListener(_agentMover.SetRotationInput);
           _agentInput.OnThrust.AddListener(_agentMover.SetThrustInput);
       }

       private void OnDisable()
       {
           _agentInput.OnRotate.RemoveListener(_agentMover.SetRotationInput); 
           _agentInput.OnThrust.RemoveListener(_agentMover.SetThrustInput);
       }
    }
}