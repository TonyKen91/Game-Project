using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISteeringForce
{
    Vector3 GetForce(AgentActor agent);
}
