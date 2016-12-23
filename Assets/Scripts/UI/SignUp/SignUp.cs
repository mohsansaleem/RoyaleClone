using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Game.Managers;

namespace Game.UI
{
	public class SignUp : Widget<SignUp>
	{
		[SerializeField]
		private InputField
			_firstNameInput;
		[SerializeField]
		private InputField
			_lastNameInput;
		[SerializeField]
		private InputField
			_gradeInput;
		[SerializeField]
		private InputField
			_usernameInput;
		[SerializeField]
		private InputField
			_passwordInput;
		[SerializeField]
		private InputField
			_passwordVerifyInput;
		[SerializeField]
		private InputField
			_parentsEmailInput;
		[SerializeField]
		private Button
			_signupButton;
		[SerializeField]
		private Text
			_statusLabel;

		#region UserInterface implementation

		override public void Show (params object[] param)
		{
			_canvasGroup.alpha = 1;
//            _canvasGroup.interactable = true;
			ToggleUserInterface (true);
			_canvasGroup.blocksRaycasts = true;
			ResetUI ();
			_canvasGroup.transform.SetAsLastSibling ();
		}

		override public void Hide ()
		{
			_canvasGroup.alpha = 0;
//            _canvasGroup.interactable = false;
			ToggleUserInterface (false);
			_canvasGroup.blocksRaycasts = false;
		}

		#endregion

		#region UI Events

		public void CloseUI ()
		{
			UIManager.Instance.PopScreen ();
		}

		public void RegisterUser ()
		{
			if (string.IsNullOrEmpty (_usernameInput.text) || string.IsNullOrEmpty (_passwordInput.text)) {
				_statusLabel.text = "Please enter username/password!";
				return;
			}

			// TODO : remove the following line and set actual reference of _gradeInput from inspector
//            _gradeInput.text = "1";

			_statusLabel.text = "Registering...";
//            Client.Client.RegisterUser(_usernameInput.text, _passwordInput.text, _firstNameInput.text,
//                                       _lastNameInput.text, _gradeInput.text, _parentsEmailInput.text);
			ToggleUserInterface (false);

			RegistrationSuccess ();
		}

//        public void LoginUser()
//        {
//            if (string.IsNullOrEmpty(_usernameInput.text) || string.IsNullOrEmpty(_passwordInput.text))
//            {
//                _statusLabel.text = "Please enter username/password!";
//                return;
//            }
//
//            _statusLabel.text = "Logging...";
//            Client.Client.LoginUser(_usernameInput.text, _passwordInput.text);
//            ToggleUserInterface(false);
//        }

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

//        public void LogInSuccess()
//        {
//            _statusLabel.text = "Logged In";
//        }

//        public void LogInFailed(string message)
//        {
//            _statusLabel.text = message;
//            ToggleUserInterface(true);
//        }

		public void RegistrationSuccess ()
		{
			_statusLabel.text = "Success";


			UIManager.Instance.ShowUI (GameUI.Login, PreviousScreenVisibility.DequeuePreviousExcludingHud);
		}

		public void RegistrationFailed (string param)
		{
			_statusLabel.text = param;
			ToggleUserInterface (true);
		}

		public void ConnectionErorr (string message)
		{
			_statusLabel.text = message;
			ToggleUserInterface (true);
		}

        #endregion
        
        #region Signup
		public void VerifyPassword ()
		{ // trigger OnEndEdit of verify field
//            Debug.LogError("VerifyPassword: " + _passwordInput.text + " - " + _passwordVerifyInput.text);
			if (_passwordInput.text.Equals (_passwordVerifyInput.text)) {
				_statusLabel.text = "";
			} else {
				_statusLabel.text = "Password don't match";
			}
		}
        
		public void AllowSignup ()
		{ // trigger OnValueChange on every inputfield
			if (IsAllFormFieldsFilled && _passwordInput.text.Equals (_passwordVerifyInput.text)) {
				_signupButton.interactable = true;
			} else
				_signupButton.interactable = false;
		}

		bool IsAllFormFieldsFilled {
			get {
				if (string.IsNullOrEmpty (_firstNameInput.text) || string.IsNullOrEmpty (_lastNameInput.text) 
//                    || string.IsNullOrEmpty(_gradeInput.text) // TODO : uncomment this line after actual implementation
					|| string.IsNullOrEmpty (_usernameInput.text) 
					|| string.IsNullOrEmpty (_passwordInput.text) || string.IsNullOrEmpty (_passwordVerifyInput.text) 
					|| string.IsNullOrEmpty (_parentsEmailInput.text)) {
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
			_firstNameInput.text = "";
			_lastNameInput.text = "";
//            _gradeInput.text = "1";     // TODO : dummy value, till the drop down list of grade is not made
			_usernameInput.text = "";
			_passwordInput.text = "";
			_passwordVerifyInput.text = "";
			_parentsEmailInput.text = "";
			_signupButton.interactable = false;
			_statusLabel.text = "All fields are mandatory";
		}
	}
}