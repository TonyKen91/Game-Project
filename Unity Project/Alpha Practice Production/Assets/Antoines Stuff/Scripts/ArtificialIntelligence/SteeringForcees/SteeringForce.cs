using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringForce
{
    public abstract Vector3 GetForce(AgentActor agent);
}
