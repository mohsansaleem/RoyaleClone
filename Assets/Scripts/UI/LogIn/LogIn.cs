using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Game.Managers;
namespace Game.UI
{
	public class LogIn : Widget<LogIn>
	{
		[SerializeField]
		private InputField
			_usernameInput;
		[SerializeField]
		private InputField
			_passwordInput;
		[SerializeField]
		private Button
			_loginButton;
		[SerializeField]
		private Button
			_registerButton;
		[SerializeField]
		private Text
			_statusLabel;



		#region UI Events

		public void RegisterUser ()
		{
			UIManager.Instance.ShowUI (GameUI.Signup, PreviousScreenVisibility.HidePreviousExcludingHud);
//            if (string.IsNullOrEmpty(_usernameInput.text) || string.IsNullOrEmpty(_passwordInput.text))
//            {
//                _statusLabel.text = "Please enter username/password!";
//                return;
//            }
//
//            _statusLabel.text = "Registering...";
//            Client.Client.RegisterUser(_usernameInput.text, _passwordInput.text);
//            ToggleUserInterface(false);
		}

		public void LoginUser ()
		{
			if (string.IsNullOrEmpty (_usernameInput.text) || string.IsNullOrEmpty (_passwordInput.text)) {
				_statusLabel.text = "Please enter username/password!";
				return;
			}

			_statusLabel.text = "Logging...";

			ToggleUserInterface (false);

			UIManager.Instance.ShowUI (GameUI.Hud, PreviousScreenVisibility.DequeuePreviousExcludingHud);
		}

		#endregion

		#region ClientEvents callbacks

		public void ConnectingToServer ()
		{
			_statusLabel.text = "Connecting to server!";
		}

		public void ConnectedToServer ()
		{
			_statusLabel.text = "Connected!";
		}

		public void LogInSuccess ()
		{
			_statusLabel.text = "Logged In";
		}

		public void LogInFailed (string message)
		{
			_statusLabel.text = message;
			ToggleUserInterface (true);
		}

//        public void RegistrationSuccess()
//        {
//            _statusLabel.text = "Success";
//            ToggleUserInterface(true);
//        }
//
//        public void RegistrationFailed(string param)
//        {
//            _statusLabel.text = param;
//            ToggleUserInterface(true);
//        }

		public void ConnectionErorr (string message)
		{
			_statusLabel.text = message;
			ToggleUserInterface (true);
		}

		#endregion
        
        #region Signup
        
		public void AllowLogin ()
		{ // trigger OnValueChange on every inputfield
			if (IsAllFormFieldsFilled) {
				_loginButton.interactable = true;
			}
		}
        
		bool IsAllFormFieldsFilled {
			get {
				if (string.IsNullOrEmpty (_usernameInput.text) || string.IsNullOrEmpty (_passwordInput.text)) {
					return false;
				}
				return true;
			}
		}
        #endregion

		void ToggleUserInterface (bool doEnable)
		{
			_canvasGroup.interactable = doEnable;
		}

		void ResetUI ()
		{
			_usernameInput.text = "";
			_passwordInput.text = "";
			_loginButton.interactable = false;
			//_statusLabel.text = "Enter Username and Password";
		}
	}
}