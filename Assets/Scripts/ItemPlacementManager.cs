using UnityEngine;
using System.Collections;
using BattleSystem;

public class ItemPlacementManager : MonoBehaviour
{
    public bool DetectPosition;

    public Constants.Cards SelectedItem = Constants.Cards.Goblin;
    // Use this for initialization
    void Start()
    {

    }
    
    // Update is called once per frame
    void Update()
    {
        if (!DetectPosition)
            return;
        if (Input.GetMouseButtonDown(0))
        { 
            RaycastHit hit; 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
            if (Physics.Raycast(ray, out hit))
            { 
                if (hit.transform != null)
                { 
                    Debug.Log(hit.point);
                    Spawn(hit.point);
                } 
            }
        }
    }

    void Spawn(Vector3 Pos)
    {
        Unit unit = new RangedUnit("TestUnit Blue", GameDataHandler.Instance.GetItemData(SelectedItem), new Player(), Pos.z > 16f ? Team.Red : Team.Blue, new Vector2D(Pos.x, Pos.z), BattleGrid.Instance);
//        Debug.Log(BattleGrid.Instance.Map.GetLength(0)+" "+BattleGrid.Instance.Map.GetLength(1));
        if (!BattleGrid.Instance.Map [(int)Pos.x, (int)Pos.z].AddItem(unit))
                UnityEngine.Debug.LogError("Unable to Add A");
        else
            UnitObject.Spawn(unit);
    }
}
