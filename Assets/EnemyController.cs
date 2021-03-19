using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform target;
    [SerializeField] private PathFinding pathFinding;
    Vector3 vPosCorrection;
    public Transform beforeTarget;

    private void Awake()
    {
        pathFinding = FindObjectOfType<PathFinding>();
    }

    private void Start()
    {
        vPosCorrection = new Vector3(0.5f, 0.5f, 0f);
        pathFinding.FindPath(transform.position, target.position, FinishedProcessingPath);

    }
    private void FixedUpdate()
    {

        pathFinding.FindPath(transform.position, target.position, FinishedProcessingPath);

    }

    private void FinishedProcessingPath(Stack<Node> path, bool Successed)
    {
        if (beforeTarget != target)
        {
            StopCoroutine("FollowPath");
            
        }
        Debug.Log("´Ù½Ã");
        
        if (Successed)
        {
             StartCoroutine("FollowPath", path);
             beforeTarget = target;
        }
    }

    IEnumerator FollowPath(Stack<Node> path)
    {
        Node nextNode = path.Pop();
        Vector3 nextPosition = nextNode.Position + vPosCorrection;

        while (path.Count > 0)
        {
            if (Vector3.Distance(transform.position, nextPosition) < 0.1f)
            {
                nextNode = path.Pop();
                nextPosition = nextNode.Position + vPosCorrection;
            }

            transform.position = Vector3.MoveTowards(transform.position, nextPosition, Time.deltaTime * 10f);

            yield return new WaitForSeconds(0.02f);
        }

        yield return null;
    }

}
