using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

public class PathRequestManager : MonoBehaviour
{
    [HideInInspector]
    public Grid grid;
    public static PathRequestManager instance;
    Pathfinding pathFinding;
    Thread pathFinderThread;
    bool isProcessingPath;
    List<PathRequest> pathRequests;

    void Awake()
    {
        instance = this;
        grid = GetComponent<Grid>();
        pathFinding = new Pathfinding();
        pathRequests = new List<PathRequest>();
        pathFinderThread = new Thread(PathFinding);
        pathFinderThread.Start();
        StartCoroutine(PathFoundManager());
    }

    public static void RequestPath(PathRequest _pathRequest)
    {
        instance.pathRequests.Add(_pathRequest);
    }

    private void PathFinding()
    {
        while (true)
        {
            if (pathRequests.Count > 0)
            {
                for (int i = 0; i < pathRequests.Count; i++)
                {
                    if (pathRequests[i] != null)
                    {
                        if (pathRequests[i].done && pathRequests[i].callbackDone)
                        {
                            pathRequests.Remove(pathRequests[i]);
                        }
                        else if (!pathRequests[i].done)
                        {
                            PathRequest currentPath = pathRequests[i];
                            currentPath.path = pathFinding.StartFindPath(instance.grid, currentPath.pathStart, currentPath.pathEnd);
                            currentPath.done = true;
                        }
                    }
                }
            }
        }
    }
    private IEnumerator PathFoundManager()
    {
        while (true)
        {
            for (int i = 0; i < pathRequests.Count; i++)
            {
                if (pathRequests[i].done)
                {
                    PathRequest currentPath = pathRequests[i];
                    currentPath.callback(currentPath.path, true);
                    currentPath.callbackDone = true;
                }
            }
            yield return null;
        }
    }

}

public class PathRequest
{
    public Vector2 pathStart;
    public Vector2 pathEnd;
    public Vector2[] path;
    public Action<Vector2[], bool> callback;
    public bool done, callbackDone;
    public PathRequest(bool done, Vector2 _start, Vector2 _end, Action<Vector2[], bool> _callback)
    {
        this.done = done;
        callbackDone = done;
        path = null;
        pathStart = _start;
        pathEnd = _end;
        callback = _callback;
    }

}
