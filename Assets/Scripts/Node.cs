using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public struct Direction
{
    public Vector2 direction { get; }

    public Node node { get; }

    public int distance { get; }

    public Direction(Node node, Vector2 direction,int distance): this()
    {
        this.node = node;
        this.direction = direction;
        this.distance = distance;
    }
}

public class Node : MonoBehaviour
{
    static public int idCounter {get; private set;} = 0;

    static public List<Node> nodeList {get; private set;} = new List<Node>();
    public int id {get; private set;}
    public LayerMask obstacleLayer;
    public LayerMask nodeLayer;

    public List<Direction> availableDirections { get; private set; }

    private Tilemap tilemap;

    private void Start()
    {
        id = idCounter++;
        availableDirections = new List<Direction>();
        nodeList.Add(this);
        tilemap = GameObject.Find("Nodes").GetComponent<Tilemap>();
        CheckAvailableDirection(Vector2.up);
        CheckAvailableDirection(Vector2.down);
        CheckAvailableDirection(Vector2.left);
        CheckAvailableDirection(Vector2.right);
    }

    private void CheckAvailableDirection(Vector2 direction)
    {
        RaycastHit2D hit =
            Physics2D
                .BoxCast(transform.position,
                Vector2.one * 0.5f,
                0f,
                direction,
                1f,
                obstacleLayer);

        if (hit.collider == null)
        {
            Vector3Int centerCell = tilemap.WorldToCell(transform.position);
            Vector3Int targetCell = new Vector3Int(centerCell.x+(int)direction.x, centerCell.y+(int)direction.y, centerCell.z);
            int i = 1;
            while(true){
                //taille maximal de la grille
                //pour detecter les 2 teleporteurs
                if(targetCell.x < -13){
                    targetCell.x = 14;
                }
                else if(targetCell.x > 13){
                    targetCell.x = -14;
                }
                TileBase tile = tilemap.GetTile(targetCell);
                if(tile!=null){
                    Node prefab = tilemap.GetInstantiatedObject(targetCell).GetComponent<Node>();
                    availableDirections.Add (new(prefab,direction,i));
                    break;
                }
                targetCell.x+=(int)direction.x;
                targetCell.y+=(int)direction.y;
                i++;
            }
        }
    }


}

