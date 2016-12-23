using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Game;
using Game.UI;
using System.Linq;

namespace Game.Managers
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField]
        private bool
            _allowDuplicateInQueue;

        [SerializeField]
        private bool
            _searchGameObjectsWithTag;

        [SerializeField]
        private PreviousScreenVisibility
            _previousScreenVisibility;

        [SerializeField]
        private Transform
            _parentCanvas;

        [SerializeField]
        private GameObject
            _defaultTextPanelRef;

        [SerializeField]
        private List<IWidget>
            uiQueue;

        private List<IWidget> UIQueue
        {
            set
            {
                uiQueue = value;
            }

            get
            {
                if (uiQueue == null)
                    return new List<IWidget>();
                return uiQueue;
            }
        }

        void Awake()
        {
            UIQueue = new List<IWidget>();

//			if (_searchGameObjectsWithTag) {
//				int count = _parentCanvas.childCount;
//				for (int i = 0; i < count; i++) {
//					var child = _parentCanvas.GetChild (i);
//					bool isActive = child.gameObject.activeSelf;
//					Debug.LogError ("Child Name: " + child.name);
//					if (!isActive)
//						child.gameObject.SetActive (true);
//					var widget = child.gameObject.GetComponent<IWidget> ();
//					
//					if (widget != null) {
//						
//						Debug.LogError ("Widget Name: " + widget.ToString ());
//						UIQueue.Add (widget);
//					}
//					
//					if (!isActive)
//						child.gameObject.SetActive (false);
//				}
//			}
        }

        protected override void Start()
        {
            base.Start();
        } 

        /// <summary>
        /// Shows the Screen.
        /// Previous screen would be handled according to default option. Go through UI Manager inspector properties to set Default.
        /// </summary>
        /// <param name="uiToShow">User interface to show.</param>
        public IWidget ShowUI(GameUI uiToShow)
        {
            return ShowUI(uiToShow, _previousScreenVisibility);
        }

        /// <summary>
        /// Shows the Screen.
        /// </summary>
        /// <param name="uiToShow">Screen to show.</param>
        /// <param name="previousScreenVisibility">Previous Screen Visibility.</param>
        /// <param name="param">Aditional Parameters.</param>
        public IWidget ShowUI(GameUI uiToShow, PreviousScreenVisibility previousScreenVisibility, params object[] param)
        {
            return ShowUI(uiToShow, Vector3.zero, previousScreenVisibility, _allowDuplicateInQueue, _searchGameObjectsWithTag, true, param);
        }

        /// <summary>
        /// Shows the Screen.
        /// </summary>
        /// <param name="uiToShow">Screen to show.</param>
        /// <param name="pos">Position of the Screen.</param>
        /// <param name="previousScreenVisibility">Previous screen visibility.</param>
        /// <param name="param">Aditional Parameters.</param>
        public IWidget ShowUI(GameUI uiToShow, Vector3 pos, PreviousScreenVisibility previousScreenVisibility, bool allowDuplicateInQueue, bool searchGameObjectsWithTag, bool setAsLastSibling = true, params object[] param)
        {
            // Hide or Dequeue Previous UI
            if (previousScreenVisibility != PreviousScreenVisibility.DoNothing)
            {
                if (previousScreenVisibility == PreviousScreenVisibility.DequeuePreviousExcludingHud || previousScreenVisibility == PreviousScreenVisibility.DequeuePreviousIncludingHud)
                    DequeueUI(previousScreenVisibility);
                if (previousScreenVisibility == PreviousScreenVisibility.HidePreviousExcludingHud || previousScreenVisibility == PreviousScreenVisibility.HidePreviousIncludingHud)
                    HideCurrentUI(previousScreenVisibility);
            }
            IWidget uiRef = null;


            uiRef = GetScreen(uiToShow, true, searchGameObjectsWithTag, allowDuplicateInQueue);

            ((MonoBehaviour)uiRef).transform.SetParent(_parentCanvas, false);
            ((MonoBehaviour)uiRef).transform.localPosition = pos;

            if (setAsLastSibling)
                ((MonoBehaviour)uiRef).transform.SetAsLastSibling();

            return ShowUI(uiRef, allowDuplicateInQueue, param);
        }

        /// <summary>
        /// Shows the Screen.
        /// </summary>
        /// <param name="uiToShow">Screen to show.</param>
        /// <param name="allowDuplicateInQueue">If set to <c>true</c> allow duplicate in queue.</param>
        /// <param name="param">Parameter.</param>
        public IWidget ShowUI(IWidget uiToShow, bool allowDuplicateInQueue = false, params object[] param)
        {
            if (uiToShow != null)
            {
                if (UIQueue == null)
                    UIQueue = new List<IWidget>();
                if (!UIQueue.Contains(uiToShow) || allowDuplicateInQueue)
                    UIQueue.Add(uiToShow);
                else
                {
                    UIQueue.Remove(uiToShow);
                    UIQueue.Add(uiToShow);
                }
                uiToShow.Show(param);
            }
            return uiToShow;
        }

        /// <summary>
        /// Get a Specific Screen. If there are multiple instances of a screen in the Queue. It will return the most recent Screen.
        /// </summary>
        /// <returns>The screen.</returns>
        /// <param name="ui">Screen Name.</param>
        /// <param name="initiateIfDoesNotExist">If set to <c>true</c> Instantiate the Screen if it does not exist.</param>
        /// <param name="searchGameObjectsWithTag">If set to <c>true</c> Also Search Game Objects herarchy with tag for the PreviousScreenInstance.</param>
        public IWidget GetScreen(GameUI ui, bool initiateIfDoesNotExist = false, bool searchGameObjectsWithTag = false, bool createNew = false)
        {
            object screenGameObject = null;
            string tagUI = "", prefabPath = "";

#region UI_SELECTION

            switch (ui)
            {
                case GameUI.Signup:
                    if (!createNew)
                        screenGameObject = UIQueue.LastOrDefault(s => s is SignUp);

                    tagUI = Constants.SIGNUP_PANEL;
                    prefabPath = Constants.SIGNUP_UI_PATH;
                    break;
                case GameUI.Login:
                    if (!createNew)
                        screenGameObject = UIQueue.LastOrDefault(s => s is LogIn);

                    tagUI = Constants.SIGNUP_PANEL;
                    prefabPath = Constants.LOGIN_UI_PATH;
                    break;
                case GameUI.Hud:
                    if (!createNew)
                        screenGameObject = UIQueue.LastOrDefault(s => s is Hud);

                    tagUI = Constants.HUD_PANEL;
                    prefabPath = Constants.HUD_UI_PATH;
                    break;
            }

#endregion

            if (searchGameObjectsWithTag && screenGameObject == null)
                screenGameObject = GameObject.FindGameObjectWithTag(tagUI);
            if (screenGameObject == null && initiateIfDoesNotExist)
                screenGameObject = Instantiate(PrefabsManager.Instance.Load(prefabPath), Vector3.zero, Quaternion.identity)as GameObject;

            if (screenGameObject != null)
            {
                if (screenGameObject is GameObject)
                    return ((GameObject)screenGameObject).GetComponent(typeof(IWidget)) as IWidget;
                if (screenGameObject is IWidget)
                    return screenGameObject as IWidget;
            }
            return null;
        }

        /// <summary>
        /// Shows the Last Screen in the Queue.
        /// </summary>
        public IWidget ShowPreviousUI()
        {
            if (UIQueue.Count > 0)
                return this.ShowUI(UIQueue [UIQueue.Count - 1]);
            return null;
        }

        public IWidget GetTop()
        {
            return UIQueue.Count > 0 ? UIQueue [UIQueue.Count - 1] : null;
        }

        /// <summary>
        /// Pops the screen. Call this function when you want to pop a screen from the stack
        /// </summary>
        public IWidget PopScreen()
        {
            DequeueUI(PreviousScreenVisibility.DequeuePreviousExcludingHud);
            return ShowPreviousUI();
        }

        /// <summary>
        /// Hides the Screen.
        /// </summary>
        /// <param name="uiToHide">Screen to hide.</param>
        public void HideUI(GameUI uiToHide)
        {
            IWidget uiRef = GetScreen(uiToHide);
			
            HideUI(uiRef);
        }

        /// <summary>
        /// Hides the Previous Screen.
        /// </summary>
        /// <param name="previousScreenVisibility">Previous Screen visibility.</param>
        public void HideCurrentUI(PreviousScreenVisibility previousScreenVisibility)
        {
            if (UIQueue.Count > 0)
            {
                IWidget screen = UIQueue [UIQueue.Count - 1];
                if (previousScreenVisibility == PreviousScreenVisibility.DequeuePreviousIncludingHud || !(screen is Hud))
                {
                    this.HideUI(screen);
                }
            }
        }

        /// <summary>
        /// Dequeues the Previous Screen from the Screens Queue.
        /// </summary>
        /// <param name="previousScreenVisibility">Previous Screen Visibility.</param>
        public void DequeueUI(PreviousScreenVisibility previousScreenVisibility)
        {
            if (UIQueue.Count > 0)
            {
                IWidget screen = UIQueue [UIQueue.Count - 1];
                if (previousScreenVisibility == PreviousScreenVisibility.DequeuePreviousIncludingHud || !(screen is Hud))
                {
                    UIQueue.RemoveAt(UIQueue.Count - 1);
                    this.DestroyUI(screen);
                }
            }
        }

        /// <summary>
        /// Hides a Specific Screen.
        /// </summary>
        /// <param name="uiToHide">Screen to hide.</param>
        private void HideUI(IWidget uiToHide)
        {
            if (uiToHide != null)
            {
                uiToHide.Hide();
            }
        }

        /// <summary>
        /// Destroies the ScreenObject.
        /// </summary>
        /// <param name="uiToHide">User interface to hide.</param>
        private void DestroyUI(IWidget uiToHide)
        {
            if (uiToHide != null)
            {
                uiToHide.Destroy();
            }
        }
    }
}