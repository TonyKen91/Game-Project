using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeBehaviour : IBehaviour {
    protected LinkedList<IBehaviour> m_childBehaviours = new LinkedList<IBehaviour>();

    // Update is called once per frame
    public abstract BehaviourResult UpdateBehaviour(AgentActor agent);
    public void addBehaviour(IBehaviour child)
    {
        m_childBehaviours.AddLast(child);
    }
}

public class Selector : CompositeBehaviour
{
    public override BehaviourResult UpdateBehaviour(AgentActor agent)
    {
        foreach (IBehaviour child in m_childBehaviours)
        {
            if (child.UpdateBehaviour(agent) == BehaviourResult.SUCCESS)
                return BehaviourResult.SUCCESS;
        }
        return BehaviourResult.FAILURE;
        //throw new NotImplementedException();
    }

}

public class Sequence : CompositeBehaviour
{
    public override BehaviourResult UpdateBehaviour(AgentActor agent)
    {
        foreach (IBehaviour child in m_childBehaviours)
        {
            if (child.UpdateBehaviour(agent) == BehaviourResult.FAILURE)
                return BehaviourResult.FAILURE;
        }
        return BehaviourResult.SUCCESS;
        //foreach (IBehaviour child in m_child)
        //throw new NotImplementedException();
    }
}
