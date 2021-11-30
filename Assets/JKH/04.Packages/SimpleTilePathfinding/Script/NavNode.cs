using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The namespace used by SimplePathFinding2D 
namespace SimplePF2D {

    // A class that stores data about the individual cells/nodes of the Nav grid.
    // Stores information as to whether this node is blocked or has some kind of movement penalty.
    public class NavNode {

        private SimplePathFinding2D pf; // The Object that contains the Nav grid.
        private bool isBlockedFlag; // If a node is blocked then that node cannot be traversed during path finding. (like walls for example)
        private Vector3Int index; // The Nav grid position of this nav node.
        private int movementpenalty; // A movement penalty associated with the node. Useful if you want a node to be traversable but not desirable when pathfinding.

        public NavNode(SimplePathFinding2D newpf){
            pf = newpf;
        }

        public void SetIndex(Vector3Int newIndex){
            index = newIndex;
        }

        public Vector3Int GetIndex(){
            return index;
        }

        public void SetBlocked(bool newIsBlockedFlag){

            isBlockedFlag = newIsBlockedFlag;

            // Debug - draws the nodes in debug mode;
            if (pf.DebugDrawNavGrid){
                if (isBlockedFlag){
                    pf.SelectedNavigationTileMap.SetTile(pf.NavToTile(index), pf.BlockTile);
                } else if (movementpenalty == 0) {
                    pf.SelectedNavigationTileMap.SetTile(pf.NavToTile(index), null);
                }
            }
        }

        public bool IsBlocked() {
            return isBlockedFlag;
        }

        public bool IsIgnorable(){
            return IsBlocked();
        }

        public void Reset(){
            movementpenalty = 0;
        }

        // The "cost" to moving to neighbour/adjacent nodes.
        public static int DirectionalGCost(DiagonalDirectionEnum dir){
            
            switch (dir) {
                case DiagonalDirectionEnum.UpLeft:
                    return 14;
                case DiagonalDirectionEnum.Up:
                    return 10;
                case DiagonalDirectionEnum.UpRight:
                    return 14;
                case DiagonalDirectionEnum.Right:
                    return 10;
                case DiagonalDirectionEnum.DownRight:
                    return 14;
                case DiagonalDirectionEnum.Down:
                    return 10;
                case DiagonalDirectionEnum.DownLeft:
                    return 14;
            }

            return 10;
        }

        public int GetMovementPenalty(){
            return movementpenalty;
        }

        // Sets a movement penalty for this node. Draws the tile if debug mode is selected. 
        public void SetMovementPenalty(int val, UnityEngine.Tilemaps.TileBase tile = null){

            movementpenalty = val;

            // Debug - draws the nodes in debug mode;
            if (pf.DebugDrawNavGrid)
            {
                if (movementpenalty == 0)
                {
                    pf.SelectedNavigationTileMap.SetTile(pf.NavToTile(index), null);
                }
                else
                {
                    pf.SelectedNavigationTileMap.SetTile(pf.NavToTile(index), tile);
                }
            }
        }

    }

}

