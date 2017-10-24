using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BehaviourResult
{
    SUCCESS,
    FAILURE
}


public interface IBehaviour {

    BehaviourResult UpdateBehaviour(AgentActor agent);

}
