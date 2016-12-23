using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GameDataHandler {

    private static string Metadata = "{\"Cards\": " +
        "{\"Goblin\": {\"Name\": \"Goblin\",\"HP\": \"500\",\"MoveSpeed\": \"1\",\"SightRange\": \"6\",\"Range\": \"4\",\"TargetType\": \"Any\",\"HitSpeed\": \"1.5\",\"ProjectileSpeed\": \"1.5\",\"ProjectileRange\": \"2\",\"Damage\": \"200\"}," +
            "\"KingTower\": {\"Name\": \"KingTower\",\"HP\":\"1000\",\"Damage\": \"200\",\"Range\": \"6\",\"HitSpeed\": \"1.5\",\"ProjectileSpeed\": \"1.5\",\"ProjectileRange\": \"2\",\"TargetType\": \"Any\"}}}";
    private JObject Cards ;

    private static GameDataHandler _Instance;
    public static GameDataHandler Instance
    {
        get
        {
            if(_Instance == null)
            {
                _Instance = new GameDataHandler(Metadata);
            }
            return _Instance;
        }
    }

    GameDataHandler(string metadata)
    {
        JObject rootObj = JObject.Parse(metadata);
        Cards = rootObj ["Cards"] as JObject;
    }

    public BattleSystem.Card GetItemData(Constants.Cards Card)
    {
        return JsonConvert.DeserializeObject<BattleSystem.Card>(GameDataHandler.Instance.Cards [Card.ToString()].ToString());
    }
}
