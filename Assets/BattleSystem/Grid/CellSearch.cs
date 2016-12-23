using System;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem
{
    public class CellSearch : IComparable<CellSearch>
    {
        private int id = 0;
        public int F = 0;
        public float Fv = 0F;
    
        public int ID
        {
            get
            {
                return id;
            }
            private set
            {
                this.id = value;
            }
        }
    
        public CellSearch(int i, int f)
        {
            id = i;
            F = f;
        }
    
        public CellSearch(int i, float f)
        {
            id = i;
            Fv = f;
        }
    
    
        public int CompareTo(CellSearch b)
        {
            return this.F.CompareTo(b.F);
        }
    }

    public class SortOnFvalue : IComparer<CellSearch>
    {
        public int Compare(CellSearch a, CellSearch b)
        {
            if (a.F > b.F)
                return 1;
            else if (a.F < b.F)
                return -1;
            else
                return 0;
        }
    }
}