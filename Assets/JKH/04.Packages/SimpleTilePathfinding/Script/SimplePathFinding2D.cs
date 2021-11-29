using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SimplePF2D; // -- This is the namespace used for all Simple Path Finding Objects.

// This MUST always be a component a GameObject that has a Grid component. It is used in conjunction with Grid.
// Handles the Navigation grid/mesh as well as processing paths queries.
public class SimplePathFinding2D : MonoBehaviour {

    //그리드 3차 배열에 맞게 해야한다.
    private Grid grid; // A reference to the Grid component attached to the same GameObject as this component.
    private NavNode[,] navNodes; // A array of navigation nodes/cells that make up the navigation grid. The 2D index of this array is referred to as Nav coordinates.
    private GameObject debugMarker; // A optional GameObject that will be instantiated to display paths when this debug option is selected.
    private int navMinX = -128; // These are the max and min world coordinates of the nav grid.
    private int navMinY = -128;
    private int navMaxX = 128;
    private int navMaxY = 128;
    private bool initflag = false; // A flag signifying this object has finished initialising.

    private LinkedList<AStarSearch> pathqueries; // A list of path queries that are waiting or are still being processed.

    private List<GameObject> debugPathFindPoints; // A list of debug marker game objects used to display paths.
    private bool debugDrawPathsFlag = false; // Flags used to display the marker paths or to display the tiles of the navigation tile map at run time.
    private bool debugDrawNavGridFlag = false;

    // User selected info.
    public Tilemap SelectedNavigationTileMap; // The tile map used as a mask to generate the navigation grid. 1 Tile represents 1 Navigation node in the grid.
    public Tile BlockTile; // A specific Tile which represents that the Navigation node is blocked at this position.
    public List<Tilemap> ColliderTileMaps; // A list of Tilemaps that contain collision data about your grid.
    public List<Tile> NavigationMovementPenaltyTiles; // A list of different tiles (which should be part of the selected navigation tilemap), that can have different movement penalty values.
    public List<int> CorrespondingMovementPenalty; // A list that directly corresponds to the above list that specifies the movement penalty for each tile in that list.
    public int NavigationMapWidth = 256; // User inputted width and height of the navigation grid with it's centre at 0, 0, 0
    public int NavigationMapHeight = 256;
    public bool DynamicAmortisation = true; // Linearly scales between min and max iterations per frame depending on the amount of concurrently being processed. When this is turned off
                                            // the amortisation defaults to the Maximum iterations per frame value.
    public int PathLoad = 64; // The number of paths to process per frame. You may want to process a small amount of paths per frame with large iterations or vice versa. Adjust this to get the performance you want.
    public int MinimumPathIterationsPerFrame = 8; // The amount of iterations the AStar algorithm will perform per frame for each path. Linearly scales between min and max if dynamic amortisation is turned on.
    public int MaximumPathIterationsPerFrame = 512;
    public bool DebugDrawPaths = false;
    public bool DebugDrawNavGrid = false;
    public GameObject DebugMarkerObject = null;

    // Draws a new debug path.
    public void DebugAddPathMarker(List<Vector3Int> refList){

        if (debugDrawPathsFlag && debugMarker != null){
            for (int i = 0; i < refList.Count; i++){
                GameObject obj = Object.Instantiate(debugMarker);
                obj.transform.position = NavToWorld(refList[i]);
                debugPathFindPoints.Add(obj);
            }
        }

    }

    // Clears all current drawn debug paths.
    public void DebugClearPathMarker(){

        if (debugDrawPathsFlag){
            for (int i = 0; i < debugPathFindPoints.Count; i++) {
                Destroy(debugPathFindPoints[i]);
            }
            debugPathFindPoints.Clear();
        }
    }

    // Returns the layout of the current grid (Rectangle, Hexagon, Isometric, IsometricZAsY
    public GridLayout.CellLayout GetGridLayout(){
        return grid.cellLayout;
    }

    // Transforms world coordinates to Navigation grid coordinates that can be used to index the navigation grid array.
    public Vector3Int WorldToNav(Vector3 worldPos){
        Vector3Int tilePos = SelectedNavigationTileMap.layoutGrid.WorldToCell(worldPos);
        return TileToNav(tilePos);
    }

    // Transforms navigation grid coordinates to world coordinates. Does some transformation based on the swizzle of the grid. 
    // Returns the centre point of the cell based on the layout.
    public Vector3 NavToWorld(Vector3Int navPos){

        Vector3Int tilePos = NavToTile(navPos);
        // CellToWorld returns an offset from the centre of the tile depending on the tile layout.
        Vector3 cellpos = SelectedNavigationTileMap.layoutGrid.CellToWorld(tilePos);
        Vector3 newcellpos = Vector3.zero;
        Vector3 centreoffset = Vector3.zero;

        switch (GetGridLayout()){
            case GridLayout.CellLayout.Rectangle:
                centreoffset.x = SelectedNavigationTileMap.layoutGrid.cellSize.x / 2.0f;
                centreoffset.y = SelectedNavigationTileMap.layoutGrid.cellSize.y / 2.0f;
                centreoffset.z = 0.0f;        
                break;
            case GridLayout.CellLayout.Hexagon:
                centreoffset.x = 0.0f;
                centreoffset.y = 0.0f;
                centreoffset.z = 0.0f;
                break;
                
            case GridLayout.CellLayout.Isometric:
                centreoffset.x = 0.0f;
                centreoffset.y = SelectedNavigationTileMap.layoutGrid.cellSize.y / 2.0f;
                centreoffset.z = 0.0f;
                break;
            case GridLayout.CellLayout.IsometricZAsY:
                centreoffset.x = 0.0f;
                centreoffset.y = SelectedNavigationTileMap.layoutGrid.cellSize.y / 2.0f;
                centreoffset.z = 0.0f;
                break;
                
        }
        // Adjust for swizzle.
        switch (SelectedNavigationTileMap.layoutGrid.cellSwizzle){
            default:
                newcellpos.x = centreoffset.x;
                newcellpos.y = centreoffset.y;
                newcellpos.z = centreoffset.z;
                break;
            case GridLayout.CellSwizzle.XZY:
                newcellpos.x = centreoffset.x;
                newcellpos.y = centreoffset.z;
                newcellpos.z = centreoffset.y;
                break;
            case GridLayout.CellSwizzle.YXZ:
                newcellpos.x = centreoffset.y;
                newcellpos.y = centreoffset.x;
                newcellpos.z = centreoffset.z;
                break;
            case GridLayout.CellSwizzle.YZX:
                newcellpos.x = centreoffset.y;
                newcellpos.y = centreoffset.z;
                newcellpos.z = centreoffset.x;
                break;
            case GridLayout.CellSwizzle.ZXY:
                newcellpos.x = centreoffset.z;
                newcellpos.y = centreoffset.x;
                newcellpos.z = centreoffset.y;
                break;
            case GridLayout.CellSwizzle.ZYX:
                newcellpos.x = centreoffset.z;
                newcellpos.y = centreoffset.y;
                newcellpos.z = centreoffset.z;
                break;
        }

        return cellpos + newcellpos;
    }

    public Vector3Int NavToTile(Vector3Int navPos){
        navPos.x = navPos.x + navMinX;
        navPos.y = navPos.y + navMinY;
        return navPos;
    }

    public Vector3Int TileToNav(Vector3Int tilePos){
        tilePos.x = tilePos.x + navMaxX;
        tilePos.y = tilePos.y + navMaxY;
        return tilePos;
    }

    // Does a range check and returns the Nav node at the specified coordinate/index.
    public NavNode GetNode(Vector3Int index){
        if (index.x < 0 || index.x >= navMaxX * 2 || index.y < 0 || index.y >= navMaxY * 2) {
            return null;
        }
        return navNodes[index.x, index.y];
    }

    // Does a range check and returns the Nav node at the specified coordinate/index.
    public NavNode GetNode(int x, int y){
        if (x < 0 || y >= navMaxX * 2 || y < 0 || y >= navMaxY * 2)
        {
            return null;
        }
        return navNodes[x, y];
    }

    // Returns a nav node from inputted world coordinates.
    public NavNode GetNode(Vector3 pos){
        return GetNode(WorldToNav(pos));
    }

    public NavNode[,] GetNavGrid(){
        return navNodes;
    }

    // Sets a tile on the navigation tilemap to be blocked,
    public void SetNavTileBlocked(Vector3Int tilepos, bool isBlocked){

        Vector3Int navpos = TileToNav(tilepos);
        SelectedNavigationTileMap.SetTile(tilepos, BlockTile);
        GetNode(navpos).SetBlocked(isBlocked);
    }

    public void SetNavTileMovementPenalty(Vector3Int tilepos, int val, TileBase tile = null){

        Vector3Int navpos = TileToNav(tilepos);
        GetNode(navpos).SetMovementPenalty(val, tile);
        SelectedNavigationTileMap.SetTile(tilepos, tile);
    }

    // Returns a rectangle with min and max corners that contains all the nav nodes within the passed in bounds.
    // Returns false if this fails, true if it succeeds.
    public bool GetNavNodesInBounds(Bounds bound, ref Vector3Int min, ref Vector3Int max){

        // We can call this before we actually finished initialising Simple path finding, so make sure we check first.
        if (navNodes == null) { return false; }

        Vector3Int minNavPos = WorldToNav(bound.min);
        Vector3Int maxNavPos = WorldToNav(bound.max);

        // Clamp the bounds so we don't index outside of the nav grid.
        if (minNavPos.x < 0) { minNavPos.x = 0; }
        if (minNavPos.x >= navMaxX * 2) { return false; } // The bounds are totally outside the nav area.
        if (minNavPos.y < 0) { minNavPos.y = 0; }
        if (minNavPos.y >= navMaxX * 2) { return false; }

        if (maxNavPos.x < 0) { return false; } // The bounds are totally outside the nav area.
        if (maxNavPos.x >= navMaxX * 2) { maxNavPos.x = (navMaxX * 2) - 1; } 
        if (maxNavPos.y < 0) { return false; }
        if (maxNavPos.y >= navMaxY * 2) { maxNavPos.y = (navMaxY * 2) - 1; }

        min.x = minNavPos.x;
        max.x = maxNavPos.x + 1; // Add the 1 so it's a non inclusive maximum.
        min.y = minNavPos.y;
        max.y = maxNavPos.y + 1;

        return true;
    }

    // Creates a 2D array of NavNodes between the user specified width and height around the point 0,0,0.
    // Uses the user selected tile map as a mask to create blocked and unblocked node as well as giving
    // certain nodes movement penalties. If debug mode is not selected all tiles from the navigation
    // tile map will be removed.
    private void CreateNavMesh(){

        Vector3Int pos = new Vector3Int();
        int w = navMaxX - navMinX;
        int l = navMaxY - navMinY;
        navNodes = new NavNode[w, l];

        // Create the Nav Array
        for (int x = 0; x < w; x++){
            for (int y = 0; y < l; y++){
                pos.x = x;
                pos.y = y;
                navNodes[x, y] = new NavNode(this);
                navNodes[x, y].SetIndex(pos);
            }
        }

        // Look at collider maps and set the navigation map accordingly
        for (int i = 0; i < ColliderTileMaps.Count; i++) {

            for (int x = 0; x < w; x++){
                for (int y = 0; y < l; y++){
                    pos.x = x;
                    pos.y = y;
                    TileBase tile = ColliderTileMaps[i].GetTile(NavToTile(pos));
                    if (tile != null) {
                        SelectedNavigationTileMap.SetTile(NavToTile(pos), BlockTile);
                    }
                }
            }
        }

        // Go through the navigation map and set up the navigation grid.
        for (int x = 0; x < w; x++) {
            for (int y = 0; y < l; y++) {

                pos.x = x;
                pos.y = y;
                navNodes[x, y] = new NavNode(this);
                navNodes[x, y].SetIndex(pos);

                if (x == 0 || x == w - 1 || y == 0 || y == l - 1){ // Make the edges of the grid blocked.
                    SelectedNavigationTileMap.SetTile(NavToTile(pos), BlockTile);
                    navNodes[x, y].SetBlocked(true);
                }else{
                    TileBase tile = SelectedNavigationTileMap.GetTile(NavToTile(pos));
                    if (tile == BlockTile) {
                        navNodes[x, y].SetBlocked(true);
                    } else if (tile != null){
                        
                        if (NavigationMovementPenaltyTiles.Count == CorrespondingMovementPenalty.Count)
                        {
                            // Loop through and add movement penalties to all the user added tiles.
                            for (int i = 0; i < NavigationMovementPenaltyTiles.Count; i++)
                            {
                                if (tile == NavigationMovementPenaltyTiles[i])
                                {
                                    navNodes[x, y].SetMovementPenalty(CorrespondingMovementPenalty[i], tile);
                                    break;
                                }
                            }
                        }
                        navNodes[x, y].SetBlocked(false);
                    }
                }

                // Remove all tiles in the tilemap if debug is not selected.
                if (!debugDrawNavGridFlag){
                    SelectedNavigationTileMap.SetTile(NavToTile(pos), null);
                }
            }
        }

    }

    public LinkedList<AStarSearch> GetPathQueries(){
        return pathqueries;
    }

    // Returns a flag that signifies whether this component has been initialised or not.
    public bool IsInitialised(){
        return initflag;
    }

    // Start is called before the first frame update
    void Start(){

        grid = GetComponent<Grid>();
        if (grid == null){
            Debug.Log("SimplePF2D: This script requires a Grid component attached to its GameObject.");
            return;
        }

        debugMarker = DebugMarkerObject;

        if (SelectedNavigationTileMap == null){
            Debug.Log("SimplePF2D: No navigation tile map selected.");
            return;
        }
      
        if (BlockTile == null){
            Debug.Log("SimplePF2D: A Block Tile must be selected.");
            return;
        }

        NavigationMapWidth = (NavigationMapWidth / 2) * 2;
        NavigationMapHeight = (NavigationMapHeight / 2) * 2;
        if (NavigationMapWidth <= 2 || NavigationMapHeight <= 2)
        {
            Debug.Log("SimplePF2D: Navigation map width/height must be more than or equal to 4 tiles/units.");
            return;
        }

        navMinX = -(NavigationMapWidth / 2);
        navMinY = -(NavigationMapHeight / 2);
        navMaxX = (NavigationMapWidth / 2);
        navMaxY = (NavigationMapHeight / 2);

        debugDrawPathsFlag = DebugDrawPaths;
        if (debugDrawPathsFlag)
        {
            debugPathFindPoints = new List<GameObject>();
        }
        debugDrawNavGridFlag = DebugDrawNavGrid;

        pathqueries = new LinkedList<AStarSearch>();
        CreateNavMesh();
        initflag = true;

        print("done");
    }

    // Update is called once per frame
    void Update(){
        if (!initflag) { return; }

        // Loop through the list of paths that need to be processed.
        if (pathqueries.Count > 0){

            // Calculate the number of iterations for this frame
            int load = pathqueries.Count;  
            int amortiseval = MaximumPathIterationsPerFrame;
            if (load > PathLoad) {
                if (DynamicAmortisation){
                    amortiseval = MinimumPathIterationsPerFrame;
                }
                load = PathLoad;
            } else if (DynamicAmortisation) {
                float qratio = 1.0f - ((float)(pathqueries.Count - 1) / (float)PathLoad);
                amortiseval = (int)(qratio * (float)((MaximumPathIterationsPerFrame - MinimumPathIterationsPerFrame)) + MinimumPathIterationsPerFrame);
            }

            int ctr = 0;
            LinkedListNode<AStarSearch> currentnode = pathqueries.First;
            while (ctr < load){
                // A link list ensure we can remove elements from the list as we cycle through it. Possibly an issue for garbage collection somewhere down the line
                // The current node can be removed from the list during process so we cache the next node here first.
                LinkedListNode<AStarSearch> nextnode = currentnode.Next;
                currentnode.Value.Process(amortiseval);
                currentnode = nextnode;
                ctr++;
            }

        }


    }
}

