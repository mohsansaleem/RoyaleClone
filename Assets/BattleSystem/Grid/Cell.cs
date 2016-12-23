using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class Cell
    {
        private List<Item> items = new List<Item>();

        public int x = 0;
        public int y = 0;
        public float yCoord = 0;
        public float xCoord = 0;
        public float zCoord = 0;
        public int ID = 0;
        public bool walkable = true;
        public bool flyable = true;
        public Cell parent = null;
        
        public int F = 0;
        public int H = 0;
        public int G = 0;

        public bool IsLeftLane;
        public bool IsRightLane;

        //Use for faster look ups
        public int sortedIndex = -1;
        
        public Cell(int indexX, int indexY, float height, int idValue, float xcoord, float zcoord, bool w, bool f, Cell p = null)
        {
            x = indexX;
            y = indexY;
            yCoord = height;
            ID = idValue;
            xCoord = xcoord;
            zCoord = zcoord;
            walkable = w;
            flyable = f;
            parent = p;
            F = 0;
            G = 0;
            H = 0;
        }

        public static bool operator ==(Cell c1, Cell c2)
        {
            if (ReferenceEquals(c1, null) && ReferenceEquals(c2, null))
                return true;

            if (ReferenceEquals(c1, null) && !ReferenceEquals(c2, null))
                return false;

            if (!ReferenceEquals(c1, null) && ReferenceEquals(c2, null))
                return false;

            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Cell c1, Cell c2)
        {
            if (ReferenceEquals(c1, null) && ReferenceEquals(c2, null))
                return false;
            
            if (ReferenceEquals(c1, null) && !ReferenceEquals(c2, null))
                return true;
            
            if (!ReferenceEquals(c1, null) && ReferenceEquals(c2, null))
                return true;
            return c1.x != c2.x || c1.y != c2.y;
        }

        public bool HasGroundOfTeam<T>(Team team = Team.Any)
        {
            return Items.Where(item => (item is T) && item.IsGround && (team == Team.Any || item.Team == team)).Count() > 0;
        }

        public bool HasGround<T>()
        {
////            Debug.Log("Items: "+items.Count);
            bool ret = Items.Where(item => (item is T) && item.IsGround).Count() > 0;
////            Debug.Log("Ret: "+ret);
            return ret;
        }

        public Item GetGroundOfTeam<T>(Team team = Team.Any)
        {
            return Items.Where(item => (item is T) && item.IsGround && (team == Team.Any || item.Team == team)).SingleOrDefault();
        }

        public Item GetGround<T>()
        {
            return Items.Where(item => (item is T) && item.IsGround).SingleOrDefault();
        }

        public bool HasAirOfTeam<T>(Team team = Team.Any)
        {
            return Items.Where(item => (item is T) && item.IsAir && (team == Team.Any || item.Team == team)).Count() > 0;
        }
        
        public bool HasAir<T>()
        {
            return Items.Where(item => (item is T) && item.IsAir).Count() > 0;
        }

        public Item GetAirOfTeam<T>(Team team = Team.Any)
        {
            return Items.Where(item => (item is T) && item.IsAir && (team == Team.Any || item.Team == team)).SingleOrDefault();
        }

        public Item GetAir<T>()
        {
            return Items.Where(item => (item is Unit) && item.IsAir).SingleOrDefault();
        }

//        public List<T> GetTargetItems<T>(Team targetTteam,TargetType targetType)
//        {
//            List<T> itemsList = new List<T>();
//
//            if( targetType == TargetType.Any)
//
//        }

        public List<Item> Items
        {
            get
            {
                return items;
            }
        }

        public bool AddItemForcefully(Item item)
        {
            MakePlaceForItem(item);

            if (item is Unit && (item as Unit).IsGround)
                walkable = false;
            else if (item is Unit && (item as Unit).IsAir)
                flyable = false;
            
            items.Add(item);
            item.Position = new Vector2D(x, y);

            return true;
        }

        public bool AddItem(Item item)
        {
            // TODO: Make weight inteligent
            if ((item.IsGround && this.HasGround<Item>()) || (item.IsAir && this.HasAir<Item>()))
            {
                if((item.IsGround && GetGround<Item>().Weight > item.Weight) || (item.IsAir && GetAir<Item>().Weight > item.Weight))
                {
                    Debug.LogError("Weight is less \n"+ StackTraceUtility.ExtractStackTrace());
                    return false;
                }
                if(!MakePlaceForItem(item))
                {
                    Debug.LogError("!MakePlaceForItem(item): \n"+ StackTraceUtility.ExtractStackTrace());
                    return false;
                }
            }
//            if (((item.IsGround && HasGround<Item>() && (GetGround<Item>().Weight < item.Weight)) || (item.IsAir && HasAir<Item>() && (GetAir<Item>().Weight < item.Weight))) && !MakePlaceForItem(item))
//                return false;
//            if (item is Unit && item.IsGround)
            items.Add(item);
            item.Position = new Vector2D(x, y);

            if (item.ItemType == ItemType.Building)
            {
                walkable = false;
                //Debug.Log("returning :"+x+" "+y);
            }
//            else if (item is Unit && item.IsAir)
//                flyable = false;

//            Debug.Log("Unit Added: ("+x+","+y+")"+item.Id);

            return true;
        }

        public bool HasItem(Item item)
        {
            return items.Contains(item);
        }

        public bool RemoveItem(Item item)
        {
            if (!items.Contains(item))
                return true;

//            Debug.LogError("item.IsGround: " + item.IsGround + ", item.IsAir: " + item.IsAir);
            if (item.IsGround)
            {
                walkable = true;

            } else if (item.IsAir)
                flyable = true;

            return items.Remove(item);
        }

        public bool MakePlaceForItem(Item item)
        {
//            Debug.Log("MakePlaceForItem");
            Debug.LogError("MakePlaceForItem start \n"+ StackTraceUtility.ExtractStackTrace());
            if (!(item is Unit))
                return false;

            Cell cell = null;
//            Debug.Log("Pos: " + item.Position.x + ", " + item.Position.y);
            cell = BattleGrid.Instance.FindClosestCell(new Vector2D(x, y), item);

            if (cell != null)
            {
                var ret = MoveToCell(item.IsGround ? GetGround<Item>(): GetAir<Item>(), cell);
//                var ret = MoveToCell(item, cell);
                Debug.LogError("ret: "+ret +". Cell walkable: "+cell.walkable);
            }
            else
                Debug.LogError("cell is null");

            return false;
        }

        public static Vector2D GetVector2D(Cell cell)
        {
            return new Vector2D(cell.x,cell.y);
        }

        public bool MoveToCell(Item item, Cell cell)
        {
//            Debug.LogError("MoveToCell, From: " + this.x + ", " + this.y + " To: " + cell.x + ", " + cell.y);

            return HasItem(item) && cell.AddItem(item) && RemoveItem(item);
        }

        public float Distance(Cell from)
        {
            return Mathf.Sqrt((this.x - from.x) * (this.x - from.x) + (this.y - from.y) * (this.y - from.y));
        }

        public override string ToString()
        {
            return string.Format("("+x+","+y+")");
        }
    }
}
