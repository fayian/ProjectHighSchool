using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    static protected class AStartAlgorithm {
        public class Node {
            public Vector2Int pos;
            public Vector2Int end;
            public Node parent;

            public float g;
            public int h() {
                return Mathf.Abs((end.x - pos.x) + (end.y - pos.y));
            }

            public Node(Vector2Int pos, Vector2Int end, float g, Node parent) {
                this.pos = pos;
                this.end = end;
                this.g = g;
                this.parent = parent;
            }
        }

        static private Stack<Vector2> GetPath(Node node) {
            Stack<Vector2> result = new Stack<Vector2>();
            while(node != null) {
                result.Push(node.pos);
                node = node.parent;
            }
            return result;
        }
        static private Node FindBest(Dictionary<Vector2Int, Node> openSet) {
            var nodes = from node in openSet.Values
                                  where (node.g + node.h()) == openSet.Values.Min(n => n.g + n.h())
                                  select node;

            foreach(Node maxNode in nodes) {
                return maxNode;
            }
            return null;
        }

       static public Stack<Vector2> AStar(Vector2Int start, Vector2Int end) {
            Dictionary<Vector2Int, Node> openSet = new Dictionary<Vector2Int, Node>();
            HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

            Node currNode = new Node(start, end, 0, null);
            Node tmp;
            openSet.Add(currNode.pos, currNode);

            while(openSet.Count > 0) {
                //if already get to the end, return
                if (currNode.pos == end) { return GetPath(currNode); }

                currNode = FindBest(openSet);   if (currNode == null) print("error");
                int x = currNode.pos.x; int y = currNode.pos.y;

                //check four directions
                //right
                if (!Global.levelMapForMob[x + 1][y] && !closedSet.Contains(new Vector2Int(x + 1, y))) { //if tile is accessable and is not in the closed set
                    if(!openSet.TryGetValue(new Vector2Int(x + 1, y), out tmp))
                        openSet.Add(new Vector2Int(x + 1, y), new Node(new Vector2Int(x + 1, y), end, currNode.g + 1, currNode) );
                    else { //if the node is already in the openList, check if g(the distance between the node and start) is smaller, which means its a better path
                        if (currNode.g + 1 < tmp.g) {
                            tmp.g = currNode.g + 1;
                            tmp.parent = currNode;
                        }                            
                    }
                }

                //left
                if (!Global.levelMapForMob[x - 1][y] && !closedSet.Contains(new Vector2Int(x - 1, y))) {
                    if (!openSet.TryGetValue(new Vector2Int(x - 1, y), out tmp))
                        openSet.Add(new Vector2Int(x - 1, y), new Node(new Vector2Int(x - 1, y), end, currNode.g + 1, currNode));
                    else { //if the node is already in the openList, check if g(the distance between the node and start) is smaller, which means its a better path
                        if (currNode.g + 1 < tmp.g) {
                            tmp.g = currNode.g + 1;
                            tmp.parent = currNode;
                        }
                    }
                }

                //up
                if (!Global.levelMapForMob[x ][y + 1] && !closedSet.Contains(new Vector2Int(x, y + 1))) {
                    if (!openSet.TryGetValue(new Vector2Int(x, y + 1), out tmp))
                        openSet.Add(new Vector2Int(x, y + 1), new Node(new Vector2Int(x, y + 1), end, currNode.g + 1, currNode));
                    else { //if the node is already in the openList, check if g(the distance between the node and start) is smaller, which means its a better path
                        if (currNode.g + 1 < tmp.g) {
                            tmp.g = currNode.g + 1;
                            tmp.parent = currNode;
                        }
                    }
                }
                
                //down
                if (!Global.levelMapForMob[x][y - 1] && !closedSet.Contains(new Vector2Int(x, y - 1))) {
                    if (!openSet.TryGetValue(new Vector2Int(x, y - 1), out tmp))
                        openSet.Add(new Vector2Int(x, y - 1), new Node(new Vector2Int(x, y - 1), end, currNode.g + 1, currNode));
                    else { //if the node is already in the openList, check if g(the distance between the node and start) is smaller, which means its a better path
                        if (currNode.g + 1 < tmp.g) {
                            tmp.g = currNode.g + 1;
                            tmp.parent = currNode;
                        }
                    }
                }

                closedSet.Add(currNode.pos);
                openSet.Remove(currNode.pos);
            }
            print("error");
            return null;
        }
    }

    protected Stack<Vector2> path;
    protected float beginTime;
    protected const float SEARCH_PATH_DELAY = 0.5f;
    protected void SearchPath() {
        Vector2Int playerPos = Vector2Int.FloorToInt(Global.player.transform.position);
        Vector2Int pos = Vector2Int.FloorToInt(transform.position);
        path = AStartAlgorithm.AStar(pos, playerPos);
        beginTime = Time.time;
    }
    protected void MoveTowardPlayer() {
        if(Time.time - beginTime >= SEARCH_PATH_DELAY) {
            SearchPath();
            path.Pop(); //remove the first waypoint, which is the starting point
        } //Search new path to player every SEARCH_PATH_DELAY second

        //move to player
        if (path.Count > 0) {
            if (Vector2.Distance(transform.position, path.Peek() + new Vector2(0.5f, 0.5f)) > Mathf.Epsilon) {
                transform.position = Vector2.MoveTowards(transform.position, path.Peek() + new Vector2(0.5f, 0.5f), gameObject.GetComponent<EnemyStats>().speed);
                //+(0.5, 0.5) to set the tile center from bottom left to center
            } else {
                path.Pop();
            }
        }
        return;
    }
}
