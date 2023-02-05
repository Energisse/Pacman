using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostChase : GhostBehavior
{
    public LayerMask obstacleLayer;

    public LayerMask nodeLayer;

    private Tilemap tilemap;

    private void Start()
    {
        tilemap = GameObject.Find("Nodes").GetComponent<Tilemap>();
    }

    private void OnDisable()
    {
        ghost.scatter.Enable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (node != null && enabled && !ghost.frightened.enabled)
        {
            //initialisation de dijkstra
            Stack<int[]> pile = new Stack<int[]>();
            int[] distances = new int[Node.idCounter];
            int[] previous = new int[Node.idCounter];
            for (int i = 0; i < Node.idCounter; i++)
            {
                distances[i] = int.MaxValue;
                previous[i] = -1;
            }
            distances[node.id] = 0;
            pile.Push(new int[2] { 0, node.id });
            while (pile.Count != 0)
            {
                int[] pop = pile.Pop();
                int distance = pop[0];
                int current = pop[1];
                if (distance > distances[current])
                {
                    continue;
                }
                for (
                    int i = 0;
                    i < Node.nodeList[current].availableDirections.Count;
                    i++
                )
                {
                    int neighbor =
                        Node.nodeList[current].availableDirections[i].node.id;
                    int weight =
                        Node.nodeList[current].availableDirections[i].distance;
                    distance = distances[current] + weight;
                    if (distance < distances[neighbor])
                    {
                        distances[neighbor] = distance;
                        previous[neighbor] = current;
                        pile.Push(new int[2] { distance, neighbor });
                    }
                }
            }

            //On regarde tous les points les plus proche du pacman
            List<int[]> voisinPacman = new List<int[]>();
            CheckAvailableDirection(Vector2.up, voisinPacman);
            CheckAvailableDirection(Vector2.down, voisinPacman);
            CheckAvailableDirection(Vector2.left, voisinPacman);
            CheckAvailableDirection(Vector2.right, voisinPacman);

            //On recupere le points le plus proche qui rajoute le moins de poids
            int minDistanceNeighbor = int.MaxValue;
            int nearNeighborToPacman = -1;
            foreach (int[] voisin in voisinPacman)
            {
                if (distances[voisin[1]] + voisin[0] < minDistanceNeighbor)
                {
                    minDistanceNeighbor = distances[voisin[1]] + voisin[0];
                    nearNeighborToPacman = voisin[1];
                }
            }

            int currentNeighbor = nearNeighborToPacman;

            //Si le pacman est directement a coté du fantome
            if (previous[currentNeighbor] == -1)
            {
                Vector2 direction = Vector2.zero;
                float minDistance = float.MaxValue;
                foreach (Direction
                    availableDirection
                    in
                    node.availableDirections
                )
                {
                    Vector3 newPosition =
                        transform.position +
                        new Vector3(availableDirection.direction.x,
                            availableDirection.direction.y);
                    float distance =
                        (ghost.target.position - newPosition).sqrMagnitude;

                    if (distance < minDistance)
                    {
                        direction = availableDirection.direction;
                        minDistance = distance;
                    }
                }
                ghost.movement.SetDirection (direction);
                return;
            }

            //On choisit le point de direction du fantome
            while (previous[currentNeighbor] != node.id)
            {
                currentNeighbor = previous[currentNeighbor];
            }

            //on applique la direction du fantome pour aller jusqu'au point
            ghost
                .movement
                .SetDirection(node
                    .availableDirections
                    .Find((Direction direction) =>
                        direction.node.id == currentNeighbor)
                    .direction);
        }
    }

    private void CheckAvailableDirection(
        Vector2 direction,
        List<int[]> voisinPacman
    )
    {
        RaycastHit2D hit =
            Physics2D
                .BoxCast(ghost.target.position,
                Vector2.one * 0.5f,
                0f,
                direction,
                1f,
                obstacleLayer);

        if (hit.collider == null)
        {
            Vector3Int centerCell = tilemap.WorldToCell(ghost.target.position);
            Vector3Int targetCell =
                new Vector3Int(centerCell.x, centerCell.y, centerCell.z);
            int i = 0;
            while (true)
            {
                //taille maximal de la grille
                //pour detecter les 2 teleporteurs
                if (targetCell.x < -13)
                {
                    targetCell.x = 14;
                }
                else if (targetCell.x > 13)
                {
                    targetCell.x = -14;
                }
                TileBase tile = tilemap.GetTile(targetCell);
                if (tile != null)
                {
                    Node prefab =
                        tilemap
                            .GetInstantiatedObject(targetCell)
                            .GetComponent<Node>();
                    voisinPacman.Add(new int[] { i, prefab.id });
                    break;
                }
                targetCell.x += (int) direction.x;
                targetCell.y += (int) direction.y;
                i++;
            }
        }
    }
}
