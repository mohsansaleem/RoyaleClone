using System;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem
{
    public class QueuePath2D
    {
        public Vector2D startPos;
        public Vector2D endPos;
        public Item Item;
        public Action<List<Vector2D>> storeRef;
    
        public QueuePath2D(Vector2D sPos, Vector2D ePos, Item item, Action<List<Vector2D>> theRefMethod)
        {
            startPos = sPos;
            endPos = ePos;
            Item = item;
            storeRef = theRefMethod;
        }
    }
}
