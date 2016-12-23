using UnityEngine;
using System.Collections;

public static class Constants {

    /// <summary>
    /// Declare constants for the Each UI_TAG you add to the Game.
    /// </summary>
    #region UI_TAGS
    // UI Tags
    public const string SIGNUP_PANEL = "SignUpPanel";
    public const string LOGIN_PANEL = "LoginPanel";
    public const string HUD_PANEL = "HudPanel";
    #endregion
    
    
    /// <summary>
    /// Declare constants for the Each UI path for the UI you add to the Game.
    /// </summary>
    #region PREFABS
    // UI Prefabs Path
    public const string SIGNUP_UI_PATH = "Prefabs/UI/SignUp/SignUpPanel";
    public const string LOGIN_UI_PATH = "Prefabs/UI/Login/LoginPanel";
    public const string HUD_UI_PATH = "Prefabs/UI/Hud/HudPanel";
    
    public const string POPUP_PATH = "Prefabs/UI/Popup/PopupPanel";
    public const string POPUP_BUTTON_PATH = "Prefabs/UI/Popup/PopupButton";
    #endregion

    public enum Cards
    {
        Goblin,
        KingTower
    }
    
    public static readonly string UNITS_PREFAB_PATH = "Prefabs/Units/";
    public static readonly string BUILDINGS_PREFAB_PATH = "Prefabs/Buildings/";
    public static readonly string PROJECTILES_PREFAB_PATH = "Prefabs/Projectiles/";

}
