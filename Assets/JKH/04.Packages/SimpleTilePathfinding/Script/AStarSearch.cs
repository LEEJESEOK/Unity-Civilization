using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The namespace used by SimplePathFinding2D 
namespace SimplePF2D {

    // A enum used to easily and consistently reference a neighbour.
    public enum DiagonalDirectionEnum {
        UpLeft, Up, UpRight, Right, DownRight, Down, DownLeft, Left
    }
    // A single class that deals with mapping direction enums to specific coordinate offsets based on a Grid's layout.
    public class Direction {

        // Returns the coordinate offset based on a direction for a hexagonal grid layout.
        public static void GetOffset(DiagonalDirectionEnum dir, ref int xoffset, ref int yoffset, bool isEven){
            if (isEven){
                switch (dir){
                    case DiagonalDirectionEnum.UpLeft:
                        xoffset = -1;
                        yoffset = 1;
                        break;
                    case DiagonalDirectionEnum.UpRight:
                        xoffset = 0;
                        yoffset = 1;
                        break;
                    case DiagonalDirectionEnum.Right:
                        xoffset = 1;
                        yoffset = 0;
                        break;
                    case DiagonalDirectionEnum.DownRight:
                        xoffset = 0;
                        yoffset = -1;
                        break;
                    case DiagonalDirectionEnum.DownLeft:
                        xoffset = -1;
                        yoffset = -1;
                        break;
                    case DiagonalDirectionEnum.Left:
                        xoffset = -1;
                        yoffset = 0;
                        break;
                }
            }  else  {
                switch (dir)
                {
                    case DiagonalDirectionEnum.UpLeft:
                        xoffset = 0;
                        yoffset = 1;
                        break;
                    case DiagonalDirectionEnum.UpRight:
                        xoffset = 1;
                        yoffset = 1;
                        break;
                    case DiagonalDirectionEnum.Right:
                        xoffset = 1;
                        yoffset = 0;
                        break;
                    case DiagonalDirectionEnum.DownRight:
                        xoffset = 1;
                        yoffset = -1;
                        break;
                    case DiagonalDirectionEnum.DownLeft:
                        xoffset = 0;
                        yoffset = -1;
                        break;
                    case DiagonalDirectionEnum.Left:
                        xoffset = -1;
                        yoffset = 0;
                        break;
                }
            }
        }

        // Returns the coordinate offset based on a direction for a rectangular or isometric grid layout.
        public static void GetOffset(DiagonalDirectionEnum dir, ref int xoffset, ref int yoffset){

            switch (dir){
                case DiagonalDirectionEnum.UpLeft:
                    xoffset = -1;
                    yoffset = 1;
                    break;
                case DiagonalDirectionEnum.Up:
                    xoffset = 0;
                    yoffset = 1;
                    break;
                case DiagonalDirectionEnum.UpRight:
                    xoffset = 1;
                    yoffset = 1;
                    break;
                case DiagonalDirectionEnum.Right:
                    xoffset = 1;
                    yoffset = 0;
                    break;
                case DiagonalDirectionEnum.DownRight:
                    xoffset = 1;
                    yoffset = -1;
                    break;
                case DiagonalDirectionEnum.Down:
                    xoffset = 0;
                    yoffset = -1;
                    break;
                case DiagonalDirectionEnum.DownLeft:
                    xoffset = -1;
                    yoffset = -1;
                    break;
                case DiagonalDirectionEnum.Left:
                    xoffset = -1;
                    yoffset = 0;
                    break;
            }
        }
    }

    // The meat and potatoes. A class that deals with calculating a path from one position to another on a tilemap.
    // There can be multiple instances of this object. Each A* object progresses through a specified amount of
    // the path finding algorithm per frame (amortised). This means pathfinding is processed concurrently.
    // The amount of "work"/iterations performed by each of these objects can be specified in the SimplePathFinding2D object that
    // it is associated with.
    // Don't create this WITHOUT a Path or a Grid.
    public class AStarSearch{

        // A class that contains information about each node that has been traversed by the algorithm 
        private class NavWrapper{

            public NavWrapper parent;
            public NavNode node;
            public int f, g, h;
            public LinkedList<NavWrapper> openList;
            public bool isClosedFlag = false;
            public bool isOpenFlag = false;
            public bool isBlockedFlag = false;
            public bool isFreeFlag = false;

            public void Init(){
                f = 0;
                g = 0;
                h = 0;
                parent = null;
                node = null;
                openList = null;
                isClosedFlag = false;
                isOpenFlag = false;
                isBlockedFlag = false;
                isFreeFlag = false;
            }
        }

        private SimplePathFinding2D pf; // A reference to the class containing all the data on the nav grid.
        private Dictionary<NavNode, NavWrapper> navNodeWrapperMap; // Does a mapping from nodes to their associated node wrapping class.

        private static Queue<NavWrapper> freeList; // A pool of avaliable navwrappers to be used instead of recreated. Shared by all AStarSearch objects.
        private static int freeListMaxSize = 8192; // The max size of the navwrapper freelist/pool. Increasing this increases memory usuage.
        private static bool initFlag = false;
        private LinkedList<NavWrapper>[] openBins; // The "open list". A series of lists of searched nodes arranged in ascending order of their F value.
        private List<Vector3Int> refList; // A pointer to the path point list of a Path object that contains this AStarObject.
        private NavWrapper currentNavWrapper;
        private NavWrapper endWrapper; // The Final node in the path.
        private GridLayout.CellLayout layout; // Caches the layout of the Grid (Hexagonal, Rectangular etc).
        private int amortizeNumber = 0; // Keeps track of the number of iterations that this object has run per frame.
        private int overridedMaxAmortizevalue = 0; // Override the max amortize value set by the SimplePathFinding2D object.
        private bool useDiagonals; // A flag that signifies that the node grid should be traversed using diagonal neighbours.
        private float maxopenbinF = 4096.0f; // The max value of the open list bins. Any value above this gets put into the final bin.
        private int maxbins = 1024; // The number of open list bins. The bigger the number the more memory, but the quicker the traversal.
        private bool isProcessingFlag = false; // Flags associated with the state of this object.
        public bool hasFailedFlag = false;

        // Assuming that SimplePathFinding2D isn't null.
        // Does some initialisation of various objects.
        public AStarSearch(SimplePathFinding2D newpf){
            pf = newpf;
            openBins = new LinkedList<NavWrapper>[maxbins];
            for (int i = 0; i < maxbins; i++){
                openBins[i] = new LinkedList<NavWrapper>();
            }
            navNodeWrapperMap = new Dictionary<NavNode, NavWrapper>();

            // Initialise our static lists.
            if (!initFlag) {
                freeList = new Queue<NavWrapper>();
                initFlag = true;
            }
        }

        // Creates a nav wrapper that stores temporary information about a nav node for use in the path finding algorithm.
        // These are grabbed from a pool if they are free to use. If not, a new one is created.
        // Only one nav wrapper can be associated with one node.
        private NavWrapper CreateNavWrapper(NavNode node){

            NavWrapper free;
            // Does a nav wrapper for this node exist already?
            if (navNodeWrapperMap.ContainsKey(node)){
                free = navNodeWrapperMap[node];
            } else { // Create a new one or dequeue a free one from the navwrapper pool
                if (freeList.Count > 0){
                    free = freeList.Dequeue();
                    free.Init();
                } else {
                    free = new NavWrapper();
                }
                navNodeWrapperMap.Add(node, free);
            }

            free.node = node;
            return free;
        }

        // Adds a navwrapper to the open list bins based on its F value.
        // Makes it easier to look up the lowest F value this way.
        private void AddToOpenList(NavWrapper navWrapper) {

            navWrapper.isOpenFlag = true;
            LinkedList<NavWrapper> list;
            // Figure out which bin we are going to place this nav wrapper based on its F value.
            if (navWrapper.f < maxopenbinF){
                int index = (int)((navWrapper.f / maxopenbinF) * maxbins);
                list = openBins[index];
            } else{
               list = openBins[maxbins - 1];
            }

            // Go through the list and place it so that the F values are in ascending order, 
            // where the first node in the linked list is the node with the lowest f value.
            LinkedListNode<NavWrapper> currentNode = list.First;
            while (currentNode != null && navWrapper.f > currentNode.Value.f){
                currentNode = currentNode.Next;
            }

            if (currentNode != null) {
                list.AddBefore(currentNode, navWrapper);
            } else {
                list.AddLast(navWrapper);
            }

            navWrapper.openList = list;
        }

        // The closed list is "virtual". Adding to the "closed list" just removes,
        // a nav wrapper from the open list and sets its closed flag to true.
        private void AddToCloseList(NavWrapper navWrapper) {

            if (navWrapper.openList != null){
                navWrapper.openList.Remove(navWrapper);
                navWrapper.openList = null;
            }

            navWrapper.isClosedFlag = true;
        }

        // Searches the open list bins for the nav wrapper with the lowest f value.
        private NavWrapper OpenListLookForLowestF(){
            for (int i = 0; i < maxbins; i++) {
                if (openBins[i].Count > 0) {
                    return openBins[i].First.Value;
                }
            }
            return null;
        }

        // Calculates the manhatten distance from the current node to the end node.
        private int CalculateHScore(NavNode currentNode){
            return Mathf.Abs(currentNode.GetIndex().x - endWrapper.node.GetIndex().x) + Mathf.Abs(currentNode.GetIndex().y - endWrapper.node.GetIndex().y);
        }

        // Takes a look at an adjacent node based on a set direction. Checks if this is open to update it's F value.
        // If not, creates a new navwrapper associated with this adjacent node and adds it to the open bins. (after calculating its f value).
        private void CheckAdjacent(NavWrapper currentNavWrapper, DiagonalDirectionEnum dir){

            int offsetx = 0, offsety = 0;
            if (layout == GridLayout.CellLayout.Hexagon){
                Direction.GetOffset(dir, ref offsetx, ref offsety, (currentNavWrapper.node.GetIndex().y % 2) == 0);
            } else {
                Direction.GetOffset(dir, ref offsetx, ref offsety);
            }  
            Vector3Int adjIndex = new Vector3Int(currentNavWrapper.node.GetIndex().x + offsetx, currentNavWrapper.node.GetIndex().y + offsety, 0);
            NavNode adjNode;
            adjNode = pf.GetNode(adjIndex);

            if (adjNode == null || adjNode.IsIgnorable()) {
                return;
            }

            NavWrapper adjNavWrapper = CreateNavWrapper(adjNode);// IsInOpenList(adjNode);

            // Check if its in the open list.
            if (adjNavWrapper.isOpenFlag) {

                // Recalculate the F cost.
                if (currentNavWrapper.f + NavNode.DirectionalGCost(dir) < adjNavWrapper.g){
                    adjNavWrapper.g = currentNavWrapper.g + NavNode.DirectionalGCost(dir) + adjNavWrapper.node.GetMovementPenalty();
                    adjNavWrapper.f = adjNavWrapper.h + adjNavWrapper.g;
                }
            } else {

                adjNavWrapper.parent = currentNavWrapper;
                adjNavWrapper.g = currentNavWrapper.g + NavNode.DirectionalGCost(dir) + adjNavWrapper.node.GetMovementPenalty();
                adjNavWrapper.h = CalculateHScore(currentNavWrapper.node);
                adjNavWrapper.f = adjNavWrapper.h + adjNavWrapper.g;
                AddToOpenList(adjNavWrapper);
            }
        }

        // Reset our open list
        private void ResetOpenList(){
            for (int i = 0; i < maxbins; i++){
                openBins[i].Clear();
            }
        }

        // Places all the Nav wrappers in the free pool of nav wrappers for use by another AStarSearch object.
        private void ResetNavWrapperDictionary(){

            // Cap the maximum size of the freeList;
            if (freeList.Count < freeListMaxSize){
                foreach (NavWrapper wrapper in navNodeWrapperMap.Values){
                    freeList.Enqueue(wrapper);
                }
            }
            navNodeWrapperMap.Clear();
        }

        // Reset to starting conditions.
        private void Reset(){
            pf.GetPathQueries().Remove(this);
            ResetOpenList();
            ResetNavWrapperDictionary();
            endWrapper = null;
            amortizeNumber = 0;
            overridedMaxAmortizevalue = 0;
            isProcessingFlag = false;
            currentNavWrapper = null;
            refList = null;
        }

        // Goes through the traversed nodes and generates a list of path points.
        private void UnpackPath(List<Vector3Int> refPath){

            NavWrapper currentWrapper = endWrapper;
            while (currentWrapper != null){
                Vector3Int point = currentWrapper.node.GetIndex();
                refPath.Add(point);
                currentWrapper = currentWrapper.parent;
            }

            refPath.Reverse();
        }

        private bool CreatePath(NavNode startNode, NavNode endNode){

            if (startNode == null || endNode == null || startNode.IsIgnorable() || endNode.IsIgnorable()){
                // The start and/or end tile are invalid.
                return false;
            }

            endWrapper = CreateNavWrapper(endNode);
            currentNavWrapper = CreateNavWrapper(startNode);
            AddToOpenList(currentNavWrapper);
            isProcessingFlag = true;
            hasFailedFlag = false;

            pf.GetPathQueries().AddLast(this);

            return true;
        }

        // Takes the inputted coordinates and finds the nodes associated with them. Then begins to create a path.
        public bool StartPath(Vector3Int startPos, Vector3Int endPos, List<Vector3Int> refPath, bool searchUsesDiagonals, int amortizeoverride = 0)
        {

            // If this Path is currently processing another path then reset it.
            if (isProcessingFlag)
            {
                Reset();
            }
            useDiagonals = searchUsesDiagonals;
            NavNode startNode = pf.GetNode(startPos);
            NavNode endNode = pf.GetNode(endPos);
            refList = refPath;

            if (amortizeoverride > 0) {
                overridedMaxAmortizevalue = amortizeoverride;
            }

            return CreatePath(startNode, endNode);
        }

        // Run every frame. Only performs a certain number of iterations per frame based on the passed
        // in max Amortize value.
        public void Process(int maxAmortize){

            bool isSuccessFlag = false;
            bool isFailedFlag = false;
            layout = pf.GetGridLayout();

            // Use are programatic override of the amortize value if we have set one.
            if (overridedMaxAmortizevalue == 0) { overridedMaxAmortizevalue = maxAmortize; }

            while (true){
 
                if (amortizeNumber >= overridedMaxAmortizevalue) {
                    break;
                }

                currentNavWrapper = OpenListLookForLowestF();
                if (currentNavWrapper == null){ // i.e. the Open list is empty.
                    isFailedFlag = true;
                    break;
                }

                AddToCloseList(currentNavWrapper);

                // We have found the target node.
                if (currentNavWrapper.node == endWrapper.node){
                    isSuccessFlag = true;
                    break;
                }

                // Is this a hexagonal grid?
                if (layout == GridLayout.CellLayout.Hexagon){

                    CheckAdjacent(currentNavWrapper, DiagonalDirectionEnum.UpLeft);
                    CheckAdjacent(currentNavWrapper, DiagonalDirectionEnum.UpRight);
                    CheckAdjacent(currentNavWrapper, DiagonalDirectionEnum.Right);
                    CheckAdjacent(currentNavWrapper, DiagonalDirectionEnum.DownRight);
                    CheckAdjacent(currentNavWrapper, DiagonalDirectionEnum.DownLeft);
                    CheckAdjacent(currentNavWrapper, DiagonalDirectionEnum.Left);
                } else {

                    if (useDiagonals){
                        CheckAdjacent(currentNavWrapper, DiagonalDirectionEnum.UpLeft);
                        CheckAdjacent(currentNavWrapper, DiagonalDirectionEnum.UpRight);
                        CheckAdjacent(currentNavWrapper, DiagonalDirectionEnum.DownRight);
                        CheckAdjacent(currentNavWrapper, DiagonalDirectionEnum.DownLeft);
                    }
                    CheckAdjacent(currentNavWrapper, DiagonalDirectionEnum.Up);
                    CheckAdjacent(currentNavWrapper, DiagonalDirectionEnum.Right);
                    CheckAdjacent(currentNavWrapper, DiagonalDirectionEnum.Down);
                    CheckAdjacent(currentNavWrapper, DiagonalDirectionEnum.Left);
                }
                amortizeNumber++;
            }

            if (isSuccessFlag) {
                UnpackPath(refList); // Create a list of path points.
                pf.DebugAddPathMarker(refList);
                Reset();
            } else if (isFailedFlag) {
           
                hasFailedFlag = true;
                Reset();
            } else {
                // Amortized, continue on till next frame.
                amortizeNumber = 0;
            }
        }
    }
}

