using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BattleSystem;
using Game.Managers;
using Game.UI;
using Game;

public class BattleSystemClient :  Singleton<BattleSystemClient>
{
    [SerializeField]
    public bool
        DrawMapInEditor; 

    private object lockObject = new object();
    private List<Action> threadActionsQueue = new List<Action>();


    BattleGrid BattleGrid;

    #region BACKGROUND_CALLS_HANDLING

    void Start()
    {
        BattleGrid = BattleGrid.Instance;
        StartCoroutine("ExcecuteAction");
    }

    IEnumerator ExcecuteAction()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            
            lock (lockObject)
            {
                while (threadActionsQueue.Count > 0)
                {
                    //                      Debug.LogError ("Thread List" + threadActionsQueue [0].ToString ());
                    try
                    {
                        threadActionsQueue [0]();
                    } catch (Exception ex)
                    {
                        Debug.LogError("Queue Execution Error: " + ex.ToString());
                    }
                    threadActionsQueue.RemoveAt(0);
                }
            }
        }
    }

    public void AddActionForExecution(Action actionToAdd)
    {
        lock (lockObject)
        {
            threadActionsQueue.Add(actionToAdd);
        }
    }



    public void OnTableStateChanged(IActionMessage innerMessage)
    {
        //Debug.LogError ("Adding to Queue: " + JsonConvert.SerializeObject (innerMessage));
        AddActionForExecution(() => TableStateChanged(innerMessage));
    }

    #endregion BACKGROUND_CALLS_HANDLING

    public void TableStateChanged(IActionMessage innerMessage)
        //public void Listen(TableStateEventArgs e)
    {
    }


    #region GIZMOS
    //---------------------------------------DRAW MAP IN EDITOR-------------------------------------//
    void OnDrawGizmosSelected()
    {
        if (DrawMapInEditor == true && BattleGrid != null && BattleGrid.Map != null)
        {
            for (int i = 0; i < BattleGrid.Map.GetLength(1); i++)
            {
                for (int j = 0; j < BattleGrid.Map.GetLength(0); j++)
                {
                    if (BattleGrid.Map [j, i] == null)
                        continue;
                    
                    Gizmos.color = (BattleGrid.Map [j, i].walkable) ? ((BattleGrid.Map [j, i].IsLeftLane || (BattleGrid.Map [j, i].IsRightLane) ? new Color(0, 0, 0.8F, 0.5F) : new Color(0, 0.8F, 0, 0.5F))) : new Color(0.8F, 0, 0, 0.5F) ;
                    Gizmos.DrawCube(new Vector3(BattleGrid.Map [j, i].xCoord, BattleGrid.Map [j, i].yCoord, BattleGrid.Map [j, i].zCoord + 0.1F), new Vector3(BattleGrid.TileSize, BattleGrid.TileSize, 0.5F));
                }
            }
        }
    }
    
    void DrawMapLines()
    {
        if (DrawMapInEditor == true && BattleGrid != null && BattleGrid.Map != null)
        {
            for (int i = 0; i < BattleGrid.Map.GetLength(1); i++)
            {
                for (int j = 0; j < BattleGrid.Map.GetLength(0); j++)
                {
                    for (int y = i - 1; y < i + 2; y++)
                    {
//						Debug.Log("DrawMapLines4");
                        for (int x = j - 1; x < j + 2; x++)
                        {
//							Debug.Log("DrawMapLines5");
                            if (y < 0 || x < 0 || y >= BattleGrid.Map.GetLength(1) || x >= BattleGrid.Map.GetLength(0))
                                continue;
//							Debug.Log("DrawMapLines6");
                            if (!BattleGrid.Map [x, y].walkable || !BattleGrid.Map [x, y].flyable)
                                continue;
//							Debug.Log("DrawMapLines7");
                            Vector2 start = new Vector2(BattleGrid.Map [j, i].xCoord, BattleGrid.Map [j, i].yCoord);
                            Vector2 end = new Vector2(BattleGrid.Map [x, y].xCoord, BattleGrid.Map [x, y].yCoord);
                            
                            UnityEngine.Debug.DrawLine(start, end, Color.green);
                        }
                    }
                }
            }
        }
    }
    #endregion GIZMOS

    void Awake()
    {
        DrawMapLines();
        var tmp = BattleSystemClient.Instance;
    }

    void OnDestroy()
    {
        BattleSystem.Invoke.Instance.RemoveAllInvokes();

        threadActionsQueue.Clear();
        threadActionsQueue = null;
    }
}
