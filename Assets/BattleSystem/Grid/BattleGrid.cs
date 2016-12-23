using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;

namespace BattleSystem
{
    public class BattleGrid
    {
        //Singleton
        private static BattleGrid instance;
        public static BattleGrid Instance
        {
            get
            {
                if (instance == null)
                    instance = new BattleGrid();
                return instance;
            }
        }

        // TODO: Remove this. Only for testing on client side.
        private static BattleGrid serverInstance;
        public static BattleGrid ServerInstance
        {
            get
            {
                if (serverInstance == null)
                    serverInstance = new BattleGrid();
                return serverInstance;
            }
        }
    
        //Variables
        public Cell[,] Map = null;
        public float TileSize = 1;
        public int widthx = 18;
        public int heighty = 32;

		public int Width
		{
			get 
			{
				return Map.GetLength(0);
			}
		}

		public int Height
		{
			get 
			{
				return	Map.GetLength(1);
			}
		}

        public int HeuristicAggression;
        public float zStart = -10F;
        public float zEnd = 10F;
    
        public Vector2D MapStartPosition;
        public Vector2D MapEndPosition;
    
        public List<string> DisallowedTags;
        public List<string> IgnoreTags;
        public bool MoveDiagonal = true;

        public List<Cell> LeftLane;
    
        //FPS
//        private float updateinterval = 1F;
//        private int frames = 0;
//        private float timeleft = 1F;
//        private int FPS = 60;
//        private int times = 0;
//        private int averageFPS = 0;
    
        int maxSearchRounds = 0;
    
        //Queue path finding to not bottleneck it
        private List<QueuePath2D> queue = new List<QueuePath2D>();
    
        private BattleGrid()
        {
//            var inv = Invoke.Instance;
            if (TileSize <= 0)
            {
                TileSize = 1;
            }

            MapStartPosition = new Vector2D();
            MapEndPosition = new Vector2D(widthx, heighty);

            Create2DMap();

            // Adding Kings
            int center = Width / 2;

            //  Card tmpCard = new Card("KingTower", "KingTower", "ABC", 1200f, 100f, 100f, 100f, 1f, 1f, 1f, 3f, ItemType.Building, TargetType.Hybrid);
            Building KING1 = new RangedBuilding("King Tower", "KingTowerTeamA", GameDataHandler.Instance.GetItemData(Constants.Cards.KingTower), new Player(), Team.Blue, new Vector2D(center, 5), this);
            Building KING2 = new RangedBuilding("King Tower", "KingTowerTeamB", GameDataHandler.Instance.GetItemData(Constants.Cards.KingTower), new Player(), Team.Red, new Vector2D(center, heighty - 6), this);
            Building ARENA1 = new RangedBuilding("Arena Tower", "Arena Tower Red", GameDataHandler.Instance.GetItemData(Constants.Cards.KingTower), new Player(), Team.Red, new Vector2D(2, 25), this);
            this.Map [center, 5].AddItem(KING1);
            this.Map [center, heighty - 6].AddItem(KING2);
            this.Map [2, 25].AddItem(ARENA1);
            BattleSystemClient.Instance.AddActionForExecution(() => 
            {
                BuildingObject.Spawn(KING1);
                BuildingObject.Spawn(KING2);
                BuildingObject.Spawn(ARENA1);
            });
//            tmpCard = new Card("Goblin", "Goblin", "ABC", 500f, 100f, 100f, 100f, 3f, 1f, 1f, 3f, ItemType.GroundMelee, TargetType.Ground);
//            Unit tmp = new Unit("TestUnit Blue", tmpCard, new Player(), Team.Blue, new Vector2D(widthx - 1, heighty - 1), this);
//            if (!this.Map [widthx - 1, heighty - 1].AddItem(tmp))
//                UnityEngine.Debug.LogError("Unable to Add A");
//            tmpCard.MoveSpeed = 1f;
//            tmp = new Unit("TestUnit Red", tmpCard, new Player(), Team.Red, new Vector2D(Width - 1, Height - 3), this);
//            if (!this.Map [widthx - 1, heighty - 3].AddItem(tmp))
//                UnityEngine.Debug.LogError("Unable to Add B");
            Debug.LogError("Initilized!!!"+StackTraceUtility.ExtractStackTrace());
//            Invoke.Instance.InvokeRepeating(() => {
//                ExecuteQueue(); }, -1, 1);
        }
    
        public Vector2D GetTeamKing(Team team)
        {
            int center = widthx / 2;
            if (team == Team.Blue)
                return new Vector2D(center, 0);
            else if (team == Team.Red)
                return new Vector2D(center, heighty - 1);
            return null;
        }

        void ExecuteQueue()
        {
            Debug.LogError("ExecuteQueue: Before Lock");
            lock (queue)
            {
                if (queue.Count > 0)
                    PathHandler(queue [0].startPos, queue [0].endPos, queue [0].Item, queue [0].storeRef);
                else
                    Debug.LogError("Queue.Count: 0");
                queue.RemoveAt(0);
            }
            Debug.LogError("ExecuteQueue: After Lock");
        }
    
        float overalltimer = 0;
        int iterations = 0;
        //Go through one 
//    void Update()
//    {
//        timeleft -= Time.deltaTime;
//        frames++;
//        
//        if (timeleft <= 0F)
//        {
//            FPS = frames;
//            averageFPS += frames;
//            times++;
//            timeleft = updateinterval;
//            frames = 0;
//        }
//        
//        float timer = 0F;
//        float maxtime = 1000 / FPS;
//        //Bottleneck prevention
//        while (queue.Count > 0 && timer < maxtime)
//        {
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
        //           Invoke.Instance.InvokeOnce(()=>{PathHandler(queue [0].startPos, queue [0].endPos, queue [0].storeRef);},0);
//            StartCoroutine(PathHandler(queue [0].startPos, queue [0].endPos, queue [0].storeRef));
//            //queue[0].storeRef.Invoke(FindPath(queue[0].startPos, queue[0].endPos));
//            queue.RemoveAt(0);
//            sw.Stop();
//            //print("Timer: " + sw.ElapsedMilliseconds);
//            timer += sw.ElapsedMilliseconds;
//            overalltimer += sw.ElapsedMilliseconds;
//            iterations++;
//        }
//        
//        DrawMapLines();
//    }
    
    #region map
        //-------------------------------------------------INSTANIATE MAP-----------------------------------------------//
    
        	private void Create2DMap()
        {
            //Find positions for start and end of map
			int startX = 0;//(int)MapStartPosition.x;
			int startY = 0;//(int)MapStartPosition.y;
            int endX = (int)MapEndPosition.x;
            int endY = (int)MapEndPosition.y;
        
            //Find tile width and height
            int TileCountAlongX = (int)((endX - startX) / TileSize);
            int TileCountAlongY = (int)((endY - startY) / TileSize);
        
            //Set map up
            Map = new Cell[widthx, heighty];
            int size = TileCountAlongX * TileCountAlongY;
            SetListsSize(widthx * heighty);
        
            //Fill up Map
            for (int i = 0; i < heighty; i++)
            {
                for (int j = 0; j < widthx; j++)
                {
                    float x = startX + (j * TileSize) + (TileSize / 2); //Position from where we raycast - X
                    float y = startY + (i * TileSize) + (TileSize / 2); //Position from where we raycast - Z
					int ID = (i * widthx) + j; //ID we give to our Cell!
                
//                float dist = 20;
//                
//                RaycastHit[] hit = Physics.SphereCastAll(new Vector2D(x, y, zStart), Tilesize / 4, Vector2D.forward, dist);
//                bool free = true;
//                float maxZ = Math.Infinity;
//                
//                foreach (RaycastHit h in hit)
//                {
//                    if (DisallowedTags.Contains(h.transform.tag))
//                    {
//                        if (h.point.z < maxZ)
//                        {
//                            //It is a disallowed walking tile, make it false
//                            Map [j, i] = new Cell(j, i, y, ID, x, 0, false); //Non walkable tile!
//                            free = false;
//                            maxZ = h.point.z;
//                        }
//                    } else if (IgnoreTags.Contains(h.transform.tag))
//                    {
//                        //Do nothing we ignore these tags
//                    } else
//                    {
//                        if (h.point.z < maxZ)
//                        {
//                            //It is allowed to walk on this tile, make it walkable!
//                            Map [j, i] = new Cell(j, i, y, ID, x, h.point.z, true); //walkable tile!
//                            free = false;
//                            maxZ = h.point.z;
//                        }
//                    }
//                }
//                //We hit nothing set tile to false
//                if (free == true)
//                {
                    Map [j, i] = new Cell(j, i, y, ID, x, 0, true, true);//Non walkable tile! 
//                }
                }
            }
//            for(int i=0;i<5;i++)
            Map[0,16].walkable = false;
            Map[0,17].walkable = false;

            //for(int i=15;i<45;i++)
            Map[widthx-1,16].walkable = false;
            Map[widthx-1,17].walkable = false;

            for(int i=3;i<16;i++)
                Map[i,16].walkable = false;

            for(int i=3;i<16;i++)
                Map[i,17].walkable = false;

            LeftLane = new List<Cell>();
            for (int j=8; j<24; j++)
            {
                Map [2, j].IsLeftLane = true;
                LeftLane.Add(Map[2,j]);
            }
            LeftLane.Add(Map[2,28]);
            for (int j=2; j<6; j++)
            {
                Map [j, 29].IsLeftLane = true;
                LeftLane.Add(Map[j,29]);
            }
        }
    
    #endregion //End map
    
        //---------------------------------------SETUP PATH QUEUE---------------------------------------//
    
        public void InsertInQueue(Vector2D startPos, Vector2D endPos, Item item, Action<List<Vector2D>> listMethod)
        {
            QueuePath2D q = new QueuePath2D(startPos, endPos, item, listMethod);
            lock (queue)
            {
                queue.Add(q);
            }
            ExecuteQueue();
        }
    
    #region astar
        //---------------------------------------FIND PATH: A*------------------------------------------//
    
        private Cell[] openList;
        private Cell[] closedList;
        private Cell startCell;
        private Cell endCell;
        private Cell currentCell;
        //Use it with KEY: F-value, VALUE: ID. ID's might be looked up in open and closed list then
        private List<CellSearch> sortedOpenList = new List<CellSearch>();
    
        private void SetListsSize(int size)
        {
            openList = new Cell[size];
            closedList = new Cell[size];
        }
    
        public void PathHandler(Vector2D startPos, Vector2D endPos, Item item, Action<List<Vector2D>> listMethod)
        {
//            Invoke.Instance.InvokeOnce(() => {
            FindPath(startPos, endPos, item, listMethod);//}, 0);
            //yield return StartCoroutine(SinglePath(startPos, endPos, listMethod));
        }
    
//        public void FindPath(Vector2D startPos, Vector2D endPos, Item item, List<Cell> cellsList)
//        {
////            FindPath( startPos,  endPos,  item, 
////            }));
//        }

        public void VectorsToCells(List<Vector2D> vectors, List<Cell> cellsList)
        {
            foreach (var vec in vectors)
            {
//				Debug.Log((int)vec.x+" "+(int)vec.y);
//				Debug.Log(Map.GetLength(0)+" "+Map.GetLength(1));
                cellsList.Add(Map [(int)vec.x, (int)vec.y]);
            }

        }

        public void FindPath(Vector2D startPos, Vector2D endPos, Item item, Action<List<Vector2D>> listMethod)
        {
//            Debug.LogError("Find Path");
            //The list we returns when path is found
            List<Vector2D> returnPath = new List<Vector2D>();
            bool endPosValid = true;

            //Find start and end nodes, if we cant return null and stop!
            SetStartAndEndCell(startPos, endPos, item);
        
            if (startCell != null)
            {
//                if (endCell == null)
//                {
//                    endPosValid = false;
//                    FindEndCell(endPos, item);
//                    if (endCell == null)
//                    {
//                        //still no end node - we leave and sends an empty list
//                        maxSearchRounds = 0;
//                        listMethod(new List<Vector2D>());
//                        return;
//                    }
//                }
                //Clear lists if they are filled
                Array.Clear(openList, 0, openList.Length);
                Array.Clear(closedList, 0, openList.Length);
                if (sortedOpenList.Count > 0)
                {
                    sortedOpenList.Clear();
                }
            
                //Insert start node
                openList [startCell.ID] = startCell;
                BHInsertCell(new CellSearch(startCell.ID, startCell.F));
            
                bool endLoop = false;
                while (!endLoop)
                {
                    if (sortedOpenList.Count == 0)
                    {
//                        Debug.Log("If we have no nodes on the open list AND we are not at the end, then we got stucked! return empty list then.");
//                        listMethod(new List<Vector2D>());
//                        return;
                        endLoop = true;
                        continue;
                    }
                
                    //Get lowest node and insert it into the closed list
                    int id = BHGetLowest();

                    currentCell = openList [id];
                    closedList [currentCell.ID] = currentCell;
//                    Debug.Log("Adding "+currentCell+" to path");
                    openList [id] = null;
                    //sortedOpenList.RemoveAt(0);
                
                    if (NeighbourCellCheckForEnd())
                    {
                        endLoop = true;
                        continue;
                    }
                    //Now look at neighbours, check for unwalkable tiles, bounderies, open and closed listed nodes.
                    NeighbourCheck(item);
                }
            
            
                while (currentCell.parent != null)
                {
                    returnPath.Add(new Vector2D(currentCell.xCoord, currentCell.yCoord));
                    currentCell = currentCell.parent;
                }
            
                returnPath.Add(startPos);
                returnPath.Reverse();
            
//                if (endPosValid)
                {
//                    returnPath.Add(endPos);
                }
            
//                if (returnPath.Count > 2 && endPosValid)
                if (returnPath.Count > 2)
                {
                    //Now make sure we do not go backwards or go to long
                    if (Vector2D.Distance(returnPath [returnPath.Count - 1], returnPath [returnPath.Count - 3]) < Vector2D.Distance(returnPath [returnPath.Count - 3], returnPath [returnPath.Count - 2]))
                    {
                        returnPath.RemoveAt(returnPath.Count - 2);
                    }
                    if (Vector2D.Distance(returnPath [1], startPos) < Vector2D.Distance(returnPath [0], returnPath [1]))
                    {
                        returnPath.RemoveAt(0);
                    }
                }
                maxSearchRounds = 0;
                listMethod(returnPath);
            
            } else
            {
                maxSearchRounds = 0;
                listMethod(new List<Vector2D>());
            }
        }
    
        // Find start and end Cell
        private void SetStartAndEndCell(Vector2D start, Vector2D end, Item item)
        {
            //startCell = FindClosestCell(start, item);
//            Debug.Log("Start: "+start.x+" "+start.y);
            startCell = Map [(int)start.x, (int)start.y];

            endCell = Map [(int)end.x, (int)end.y];
        }
    
        public Cell FindClosestCell(Vector2D pos, Item item)
        {
            int x = (int)pos.x;//((MapStartPosition.x < 0F) ? Math.Floor(((pos.x + Math.Abs(MapStartPosition.x)) / TileSize)) : Math.Floor((pos.x - MapStartPosition.x) / TileSize));
            int y = (int)pos.y;//((MapStartPosition.y < 0F) ? Math.Floor(((pos.y + Math.Abs(MapStartPosition.y)) / TileSize)) : Math.Floor((pos.y - MapStartPosition.y) / TileSize));
        
            Cell n = Map [x,y];
        
            if (item.IsGround && n.walkable || item.IsAir && n.flyable)
            {
//                Debug.Log("returning: "+n.x+", "+n.y);
                return n;//new Cell(x, y, n.yCoord, n.ID, n.xCoord, n.zCoord, n.walkable, n.flyable);
            } else
            {
                //If we get a non walkable tile, then look around its neightbours
                for (int i = y - 1; i < y + 2; i++)
                {
                    for (int j = x - 1; j < x + 2; j++)
                    {
//                        Debug.Log("returning :"+j+" "+i);
                        //Check they are within bounderies
                        if (i > -1 && i < Map.GetLength(0) && j > -1 && j < Map.GetLength(1))
                        {
                            if (item.IsGround && Map [j, i].walkable || item.IsAir && Map [j, i].flyable)
                            {
                                return Map[j,i]; //new Cell(j, i, Map [j, i].yCoord, Map [j, i].ID, Map [j, i].xCoord, Map [j, i].zCoord, Map [j, i].walkable, Map [j, i].flyable);
                            }
                        }
                    }
                }
                Debug.Log("Null");
                return null;
            }
        }

        public Vector2D FindNearestLanePos(Vector2D Pos)
        {
            float   MinDis      = float.MaxValue;
            int     MinDisIndex = 0;
            for (int i = 0; i < LeftLane.Count; i++)
            {
                if(Vector2D.Distance(Pos,Cell.GetVector2D(LeftLane[i])) < MinDis)
                {
                    MinDis = Vector2D.Distance(Pos,Cell.GetVector2D(LeftLane[i]));
                    MinDisIndex = i;
                }
            }
            return Cell.GetVector2D(LeftLane[MinDisIndex]);
        }

        public bool NeighbourCellCheckForEnd()
        {
            int x = currentCell.x;
            int y = currentCell.y;
            
            for (int i = y - 1; i < y + 1; i++)
            {
                for (int j = x - 1; j < x + 1; j++)
                {
                    //Check it is within the bounderies
                    if (i > -1 && i < Map.GetLength(1) && j > -1 && j < Map.GetLength(0))
                    {
                        //Dont check for the current node.
                        if (i != y || j != x)
                        {
                            if(Map[j,i].ID == endCell.ID)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public Item FindTargetInSight(Item item)
        {
//            Debug.Log("FindTargetInSight: "+(int)item.Card.SightRange);
            return FindTargetAround(item, item.Position.xi,item.Position.yi, (int)item.Card.SightRange);
        }

        private Item FindTargetAround(Item item, int x, int z, int Tiles)
        {
//            Debug.LogError("UnitInSight: item Team: " + item.Team + ", Target: " + item.TargetType + ", Opponent: " + item.GetOpponent + ", " + (int)item.Position.x + ", " + (int)item.Position.y);
            Item tmp = null;
//            UnityEngine.Debug.LogError("Sight: " + sight);
            for (int i = 0; i < Tiles; i++)
            {
                if (item.TargetType == TargetType.Building)
                    tmp = UnitCellCheck<Building>(item.TargetType, item.GetOpponent, x, z, i);
                else 
                    tmp = UnitCellCheck<Item>(item.TargetType, item.GetOpponent, x, z, i);
                if (tmp != null)
                    return tmp;
            }
            return null;
        }

        public List<Item> FindTargetsinRange(Projectile proj)
        {
//            Debug.Log(string.Format("{0},{1},{2} :",proj.Owner.TargetType, proj.Owner.GetOpponent, proj.Target.xi));
            return UnitsCellCheck<Item>(proj.Owner.TargetType, proj.Owner.GetOpponent, proj.Target.xi, proj.Target.yi, proj.Range);
        }

        public Item FindTargetInRange(Item item)
        {
//            Debug.Log("FindTargetInSight: "+(int)item.Card.Range);
            return FindTargetAround(item, item.Position.xi,item.Position.yi, (int)item.Card.Range);
        }

        public bool HasGroundTarget<T>(Cell cell, TargetType targetType, Team team)
        {
            return ((targetType == TargetType.Ground || targetType == TargetType.Hybrid || targetType == TargetType.Any || targetType == TargetType.Building || targetType == TargetType.Defensive) && cell.HasGroundOfTeam<T>(team));
        }

        public bool HasAirTarget<T>(Cell cell, TargetType targetType, Team team)
        {
            return ((targetType == TargetType.Air || targetType == TargetType.Hybrid || targetType == TargetType.Any || targetType == TargetType.Building || targetType == TargetType.Defensive) && cell.HasGroundOfTeam<T>(team));
        }

        public List<Item> UnitsInSight<T>(Item item)
        {
            List<Item> items = new List<Item>();
            for (int i = 1; i < item.Card.SightRange; i++)
            {
                var tmpItems = UnitsCellCheck<T>(item.TargetType, item.GetOpponent, (int)item.Position.x, (int)item.Position.y, i);
                foreach (var tmp in tmpItems)
                    items.Add(tmp);
            }
            return items;
        }

        public Item UnitInSight<T>(Item item)
        {
            for (int i = 1; i < item.Card.SightRange; i++)
            {
                var tmpItem = UnitCellCheck<T>(item.TargetType, item.GetOpponent, (int)item.Position.x, (int)item.Position.y, i);
                if (tmpItem != null)
                    return tmpItem;
            }
            return null;
        }


        private List<Item> UnitsCellCheck<T>(TargetType targetType, Team team, int x, int z, int r)
        {
            List<Item> nodes = new List<Item>();
            
            for (int i = z - r; i < z + r + 1; i++)
            {
                for (int j = x - r; j < x + r + 1; j++)
                {
                    //Check that we are within bounderis, and goes in ring around our end pos
                    if (i > -1 && j > -1 && i < Map.GetLength(1) && j < Map.GetLength(0) /*&& ((i < z - r + 1 || i > z + r - 1) || (j < x - r + 1 || j > x + r - 1))*/)
                    {
//                        Debug.Log("Checking: "+j+","+i);
                        //if it is walkable put it on the right list
                        if (HasGroundTarget<T>(Map [j, i], targetType, team))
                            nodes.Add(Map [j, i].GetGroundOfTeam<T>(team));
                        if (HasAirTarget<T>(Map [j, i], targetType, team))
                            nodes.Add(Map [j, i].GetAirOfTeam<T>(team));
                    }
                }
            }
            
            return nodes;
        }

        public Item UnitCellCheck<T>(TargetType targetType, Team team, int x, int z, int r)
        {
//            Debug.LogError("UnitCellCheck: Map.GetLength(0): " + Map.GetLength(0) + ", Map.GetLength(1): " + Map.GetLength(1));
            for (int i = z - r; i < z + r + 1; i++)
            {
                for (int j = x - r; j < x + r + 1; j++)
                {
//                    Debug.LogError("Loop: " + i + ", " + j);
                    //Check that we are within bounderis, and goes in ring around our end pos
                    if (i > -1 && j > -1 && i < Map.GetLength(1) && j < Map.GetLength(0) && ((i < z - r + 1 || i > z + r - 1) || (j < x - r + 1 || j > x + r - 1)))
//                    if (i > -1 && j > -1 && i < Map.GetLength(1) && j < Map.GetLength(0))
                    {
//                        Debug.LogError("Checking: " + i + ", " + j);
                        if (HasGroundTarget<T>(Map [j, i], targetType, team))
                            return Map [j, i].GetGroundOfTeam<T>(team);
                        if (HasAirTarget<T>(Map [j, i], targetType, team))
                            return Map [j, i].GetAirOfTeam<T>(team);
                    }
                }
            }
            
            return null;
        }

        private void FindEndCell(Vector2D pos, Item item)
        {
            int x = (int)pos.x;//(int)((MapStartPosition.x < 0F) ? Math.Floor(((pos.x + Math.Abs(MapStartPosition.x)) / TileSize)) : Math.Floor((pos.x - MapStartPosition.x) / TileSize));
            int y = (int)pos.y;//((MapStartPosition.y < 0F) ? Math.Floor(((pos.y + Math.Abs(MapStartPosition.y)) / TileSize)) : Math.Floor((pos.y - MapStartPosition.y) / TileSize));
        
            Cell closestCell = Map [x, y];
            List<Cell> walkableCells = new List<Cell>();
        
            int turns = 20;
        
            while (walkableCells.Count < 1 && maxSearchRounds < (int)10 / TileSize)
            {
                walkableCells = EndCellNeighbourCheck(item, x, y, turns);
                turns++;
                maxSearchRounds++;
            }
        
            if (walkableCells.Count > 0) //If we found some walkable tiles we will then return the nearest
            {
                int lowestDist = 99999999;
                Cell n = null;
            
                foreach (Cell node in walkableCells)
                {
                    int i = GetHeuristics(closestCell, node);
                    if (i < lowestDist)
                    {
                        lowestDist = i;
                        n = node;
                    }
                }
                endCell = new Cell(n.x, n.y, n.yCoord, n.ID, n.xCoord, n.zCoord, n.walkable, n.flyable);
            }
        }

        private List<Cell> EndCellNeighbourCheck(Item item, int x, int z, int r)
        {
            List<Cell> nodes = new List<Cell>();
        
            for (int i = z - r; i < z + r + 1; i++)
            {
                for (int j = x - r; j < x + r + 1; j++)
                {
                    //Check that we are within bounderis, and goes in ring around our end pos
                    if (i > -1 && j > -1 && i < Map.GetLength(0) && j < Map.GetLength(1) && ((i < z - r + 1 || i > z + r - 1) || (j < x - r + 1 || j > x + r - 1)))
                    {
                        //if it is walkable put it on the right list
                        if (item.IsGround && Map [j, i].walkable || item.IsAir && Map [j, i].flyable)
                        {
                            nodes.Add(Map [j, i]);
                        }
                    }
                }
            }
        
            return nodes;
        }

        private void NeighbourCheck(Item item)
        {
            int x = currentCell.x;
            int y = currentCell.y;
        
            for (int i = y - 1; i < y + 2; i++)
            {
                for (int j = x - 1; j < x + 2; j++)
                {
                    //Check it is within the bounderies
                    if (i > -1 && i < Map.GetLength(1) && j > -1 && j < Map.GetLength(0))
                    {
                        //Dont check for the current node.
                        if (i != y || j != x)
                        {
                            //Check the node is walkable
                            if (item.IsGround && Map [j, i].walkable || item.IsAir && Map [j, i].flyable)
                            {
                                //We do not recheck anything on the closed list
                                if (!OnClosedList(Map [j, i].ID))
                                {
                                    //If it is not on the open list then add it to
                                    if (!OnOpenList(Map [j, i].ID))
                                    {
                                        Cell addCell = new Cell(Map [j, i].x, Map [j, i].y, Map [j, i].yCoord, Map [j, i].ID, Map [j, i].xCoord, Map [j, i].zCoord, Map [j, i].walkable, Map [j, i].flyable, currentCell);
                                        addCell.H = GetHeuristics(Map [j, i].x, Map [j, i].y);
                                        addCell.G = GetMovementCost(x, y, j, i) + currentCell.G;
                                        addCell.F = addCell.H + addCell.G;
                                        //Insert on open list
                                        openList [addCell.ID] = addCell;
                                        //Insert on sorted list
                                        BHInsertCell(new CellSearch(addCell.ID, addCell.F));
                                        //sortedOpenList.Add(new CellSearch(addCell.ID, addCell.F));
                                    } else
                                    {
                                        ///If it is on openlist then check if the new paths movement cost is lower
                                        Cell n = GetCellFromOpenList(Map [j, i].ID);
                                        if (currentCell.G + GetMovementCost(x, y, j, i) < openList [Map [j, i].ID].G)
                                        {
                                            n.parent = currentCell;
                                            n.G = currentCell.G + GetMovementCost(x, y, j, i);
                                            n.F = n.G + n.H;
                                            BHSortCell(n.ID, n.F);
                                            //ChangeFValue(n.ID, n.F);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

//        private void NonDiagonalNeighborCheck()
//        {
//            int x = currentCell.x;
//            int y = currentCell.y;
//        
//            for (int i = y - 1; i < y + 2; i++)
//            {
//                for (int j = x - 1; j < x + 2; j++)
//                {
//                    //Check it is within the bounderies
//                    if (i > -1 && i < Map.GetLength(1) && j > -1 && j < Map.GetLength(0))
//                    {
//                        //Dont check for the current node.
//                        if (i != y || j != x)
//                        {
//                            //Check that we are not moving diagonal
//                            if (GetMovementCost(x, y, j, i) < 14)
//                            {
//                                //Check the node is walkable
//                                if (Map [j, i].walkable)
//                                {
//                                    //We do not recheck anything on the closed list
//                                    if (!OnClosedList(Map [j, i].ID))
//                                    {
//                                        //If it is not on the open list then add it to
//                                        if (!OnOpenList(Map [j, i].ID))
//                                        {
//                                            Cell addCell = new Cell(Map [j, i].x, Map [j, i].y, Map [j, i].yCoord, Map [j, i].ID, Map [j, i].xCoord, Map [j, i].zCoord, Map [j, i].walkable, Map [j, i].flyable, currentCell);
//                                            addCell.H = GetHeuristics(Map [j, i].x, Map [j, i].y);
//                                            addCell.G = GetMovementCost(x, y, j, i) + currentCell.G;
//                                            addCell.F = addCell.H + addCell.G;
//                                            //Insert on open list
//                                            openList [addCell.ID] = addCell;
//                                            //Insert on sorted list
//                                            BHInsertCell(new CellSearch(addCell.ID, addCell.F));
//                                            //sortedOpenList.Add(new CellSearch(addCell.ID, addCell.F));
//                                        } else
//                                        {
//                                            ///If it is on openlist then check if the new paths movement cost is lower
//                                            Cell n = GetCellFromOpenList(Map [j, i].ID);
//                                            if (currentCell.G + GetMovementCost(x, y, j, i) < openList [Map [j, i].ID].G)
//                                            {
//                                                n.parent = currentCell;
//                                                n.G = currentCell.G + GetMovementCost(x, y, j, i);
//                                                n.F = n.G + n.H;
//                                                BHSortCell(n.ID, n.F);
//                                                //ChangeFValue(n.ID, n.F);
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
    
        private void ChangeFValue(int id, int F)
        {
            foreach (CellSearch ns in sortedOpenList)
            {
                if (ns.ID == id)
                {
                    ns.F = F;
                }
            }
        }
    
        //Check if a Cell is already on the openList
        private bool OnOpenList(int id)
        {
            return (openList [id] != null) ? true : false;
        }
    
        //Check if a Cell is already on the closedList
        private bool OnClosedList(int id)
        {
            return (closedList [id] != null) ? true : false;
        }
    
        private int GetHeuristics(int x, int y)
        {
            //Make sure heuristic aggression is not less then 0!
            int HA = (HeuristicAggression < 0) ? 0 : HeuristicAggression;
            return (int)(Math.Abs(x - endCell.x) * (10F + (10F * HA))) + (int)(Math.Abs(y - endCell.y) * (10F + (10F * HA)));
        }
    
        private int GetHeuristics(Cell a, Cell b)
        {
            //Make sure heuristic aggression is not less then 0!
            int HA = (HeuristicAggression < 0) ? 0 : HeuristicAggression;
            return (int)(Math.Abs(a.x - b.x) * (10F + (10F * HA))) + (int)(Math.Abs(a.y - b.y) * (10F + (10F * HA)));
        }
    
        private int GetMovementCost(int x, int y, int j, int i)
        {
            //Moving straight or diagonal?
            return (x != j && y != i) ? 14 : 10;
        }
    
        private Cell GetCellFromOpenList(int id)
        {
            return (openList [id] != null) ? openList [id] : null;
        }
    
    #region Binary_Heap (min)
    
        private void BHInsertCell(CellSearch ns)
        {
            //We use index 0 as the root!
            if (sortedOpenList.Count == 0)
            {
                sortedOpenList.Add(ns);
                openList [ns.ID].sortedIndex = 0;
                return;
            }
        
            sortedOpenList.Add(ns);
            bool canMoveFurther = true;
            int index = sortedOpenList.Count - 1;
            openList [ns.ID].sortedIndex = index;
        
            while (canMoveFurther)
            {
                int parent = (int)Math.Floor(((double)(index - 1) / 2));
            
                if (index == 0) //We are the root
                {
                    canMoveFurther = false;
                    openList [sortedOpenList [index].ID].sortedIndex = 0;
                } else
                {
                    if (sortedOpenList [index].F < sortedOpenList [parent].F)
                    {
                        CellSearch s = sortedOpenList [parent];
                        sortedOpenList [parent] = sortedOpenList [index];
                        sortedOpenList [index] = s;
                    
                        //Save sortedlist index's for faster look up
                        openList [sortedOpenList [index].ID].sortedIndex = index;
                        openList [sortedOpenList [parent].ID].sortedIndex = parent;
                    
                        //Reset index to parent ID
                        index = parent;
                    } else
                    {
                        canMoveFurther = false;
                    }
                }
            }
        }
    
        private void BHSortCell(int id, int F)
        {
            bool canMoveFurther = true;
            int index = openList [id].sortedIndex;
            sortedOpenList [index].F = F;
        
            while (canMoveFurther)
            {
                int parent = (int)Math.Floor((double)(index - 1) / 2);
            
                if (index == 0) //We are the root
                {
                    canMoveFurther = false;
                    openList [sortedOpenList [index].ID].sortedIndex = 0;
                } else
                {
                    if (sortedOpenList [index].F < sortedOpenList [parent].F)
                    {
                        CellSearch s = sortedOpenList [parent];
                        sortedOpenList [parent] = sortedOpenList [index];
                        sortedOpenList [index] = s;
                    
                        //Save sortedlist index's for faster look up
                        openList [sortedOpenList [index].ID].sortedIndex = index;
                        openList [sortedOpenList [parent].ID].sortedIndex = parent;
                    
                        //Reset index to parent ID
                        index = parent;
                    } else
                    {
                        canMoveFurther = false;
                    }
                }
            }
        }
    
        private int BHGetLowest()
        {
        
            if (sortedOpenList.Count == 1) //Remember 0 is our root
            {
                int ID = sortedOpenList [0].ID;
                sortedOpenList.RemoveAt(0);
                return ID;
            } else if (sortedOpenList.Count > 1)
            {
                //save lowest not, take our leaf as root, and remove it! Then switch through children to find right place.
                int ID = sortedOpenList [0].ID;
                sortedOpenList [0] = sortedOpenList [sortedOpenList.Count - 1];
                sortedOpenList.RemoveAt(sortedOpenList.Count - 1);
                openList [sortedOpenList [0].ID].sortedIndex = 0;
            
                int index = 0;
                bool canMoveFurther = true;
                //Sort the list before returning the ID
                while (canMoveFurther)
                {
                    int child1 = (index * 2) + 1;
                    int child2 = (index * 2) + 2;
                    int switchIndex = -1;
                
                    if (child1 < sortedOpenList.Count)
                    {
                        switchIndex = child1;
                    } else
                    {
                        break;
                    }
                    if (child2 < sortedOpenList.Count)
                    {
                        if (sortedOpenList [child2].F < sortedOpenList [child1].F)
                        {
                            switchIndex = child2;
                        }
                    }
                    if (sortedOpenList [index].F > sortedOpenList [switchIndex].F)
                    {
                        CellSearch ns = sortedOpenList [index];
                        sortedOpenList [index] = sortedOpenList [switchIndex];
                        sortedOpenList [switchIndex] = ns;
                    
                        //Save sortedlist index's for faster look up
                        openList [sortedOpenList [index].ID].sortedIndex = index;
                        openList [sortedOpenList [switchIndex].ID].sortedIndex = switchIndex;
                    
                        //Switch around idnex
                        index = switchIndex;
                    } else
                    {
                        break;
                    }
                }
                return ID;
            
            } else
            {
                return -1;
            }
        }
    
    #endregion
    
    #endregion //End astar region!
    
    
    #region DynamicSupport
    
        public void DynamicMapEdit(List<Vector2D> checkList, Action<List<Vector2D>> listMethod)
        {
            listMethod.Invoke(DynamicFindClosestCells(checkList));
        }
    
        public void DynamicRedoMapEdit(List<Vector2D> ids)
        {
            foreach (Vector2D v in ids)
            {
                Map [(int)v.x, (int)v.y].walkable = true;
            }
        }
    
        private List<Vector2D> DynamicFindClosestCells(List<Vector2D> vList)
        {
            List<Vector2D> returnList = new List<Vector2D>();
            foreach (Vector2D pos in vList)
            {
                int x = (int)((MapStartPosition.x < 0F) ? Math.Floor(((pos.x + Math.Abs(MapStartPosition.x)) / TileSize)) : Math.Floor((pos.x - MapStartPosition.x) / TileSize));
                int y = (int)((MapStartPosition.y < 0F) ? Math.Floor(((pos.y + Math.Abs(MapStartPosition.y)) / TileSize)) : Math.Floor((pos.y - MapStartPosition.y) / TileSize));
            
                if (x >= 0 && x < Map.GetLength(0) && y >= 0 && y < Map.GetLength(1))
                {
                    if (Map [x, y].walkable)
                    {
                        Map [x, y].walkable = false;
                        returnList.Add(new Vector2D(x, y));
                    }
                }
            }
        
            return returnList;
        }

        public void DynamicAirMapEdit(List<Vector2D> checkList, Action<List<Vector2D>> listMethod)
        {
            listMethod.Invoke(DynamicFindAirClosestCells(checkList));
        }
        
        public void DynamicAirRedoMapEdit(List<Vector2D> ids)
        {
            foreach (Vector2D v in ids)
            {
                Map [(int)v.x, (int)v.y].flyable = true;
            }
        }
        
        private List<Vector2D> DynamicFindAirClosestCells(List<Vector2D> vList)
        {
            List<Vector2D> returnList = new List<Vector2D>();
            foreach (Vector2D pos in vList)
            {
                int x = (int)((MapStartPosition.x < 0F) ? Math.Floor(((pos.x + Math.Abs(MapStartPosition.x)) / TileSize)) : Math.Floor((pos.x - MapStartPosition.x) / TileSize));
                int y = (int)((MapStartPosition.y < 0F) ? Math.Floor(((pos.y + Math.Abs(MapStartPosition.y)) / TileSize)) : Math.Floor((pos.y - MapStartPosition.y) / TileSize));
                
                if (x >= 0 && x < Map.GetLength(0) && y >= 0 && y < Map.GetLength(1))
                {
                    if (Map [x, y].flyable)
                    {
                        Map [x, y].flyable = false;
                        returnList.Add(new Vector2D(x, y));
                    }
                }
            }
            
            return returnList;
        }
    
    #endregion
    
    }
}
