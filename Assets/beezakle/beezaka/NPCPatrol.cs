using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class NPCPatrol: MonoBehaviour
{
    public List<Transform> pathNodes = new List<Transform>();
    public float pathReachingRadius = 2f;

    private bool IsPathValid()
    {
        return this && pathNodes.Count > 0;
    }

    public Vector3 GetPositionOfPathNode(int NodeIndex)
    {
        if (NodeIndex < 0 || NodeIndex >= pathNodes.Count || pathNodes[NodeIndex] == null)
        {
            return Vector3.zero;
        }

        return pathNodes[NodeIndex].position;
    }

    public Vector3 GetDestinationOnPath(Transform agent, int pathDestinationNodeIndex)
    {
        if (IsPathValid())
        {
            return GetPositionOfPathNode(pathDestinationNodeIndex);
        }
        else
        {
            return agent.position;
        }
    }

    public int UpdatePathDestination(Transform agent, int pathDestinationNodeIndex, bool inverseOrder = false)
    {
        if (IsPathValid())
        {
            // Check if reached the path destination  
            if ((agent.position - GetDestinationOnPath(agent, pathDestinationNodeIndex)).magnitude <= pathReachingRadius)
            {   
                pathDestinationNodeIndex ++;
                if (pathDestinationNodeIndex >= pathNodes.Count)
                {
                    pathDestinationNodeIndex = 0;
                }
                /*
                // increment path destination index
                //pathDestinationNodeIndex = inverseOrder ? (pathDestinationNodeIndex - 1) : (pathDestinationNodeIndex + 1);
                if (pathDestinationNodeIndex < 0)
                {
                    ///pathDestinationNodeIndex += pathNodes.Count;
                    pathDestinationNodeIndex ++;
                }
                if (pathDestinationNodeIndex >= pathNodes.Count)
                {
                    //pathDestinationNodeIndex -= pathNodes.Count;
                    pathDestinationNodeIndex --;
                }
                //Thread.Sleep(3000);
                */


            }
        }
        return pathDestinationNodeIndex;
    }
}
