﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;
using UnityEngine.EventSystems;


namespace TalesFromTheRift
{

    public class CanvasKeyboard : MonoBehaviour 
	{
        public SwypeController swyper;

        //public SuggestAPI suggestor;
        public GameObject outputfield;

        #region CanvasKeyboard Instantiation

        public enum CanvasKeyboardType
		{
			ASCIICapable
		}
		
		public static CanvasKeyboard Open(Canvas canvas, GameObject inputObject = null, CanvasKeyboardType keyboardType = CanvasKeyboardType.ASCIICapable)
		{
			// Don't open the keyboard if it is already open for the current input object
			CanvasKeyboard keyboard = GameObject.FindObjectOfType<CanvasKeyboard>();
            
			if (keyboard == null || (keyboard != null && keyboard.inputObject != inputObject))
			{
				Close();
				keyboard = Instantiate<CanvasKeyboard>(Resources.Load<CanvasKeyboard>("CanvasKeyboard"));
				keyboard.transform.SetParent(canvas.transform, false);
				//keyboard.inputObject = inputObject;
                
            }
			return keyboard;
		}
		
		public static void Close()
		{
			CanvasKeyboard[] kbs = GameObject.FindObjectsOfType<CanvasKeyboard>();
			foreach (CanvasKeyboard kb in kbs)
			{
				kb.CloseKeyboard();
			}
		}
		
		public static bool IsOpen 
		{
			get
			{
				return GameObject.FindObjectsOfType<CanvasKeyboard>().Length != 0;
			}
		}

		#endregion

		public GameObject inputObject;

		public string text 
		{
			get
			{
				if (inputObject != null) 
				{
					Component[] components = inputObject.GetComponents(typeof(Component));
					foreach (Component component in components)
					{
						PropertyInfo prop = component.GetType().GetProperty("text", BindingFlags.Instance | BindingFlags.Public);
						if (prop != null)
						{
							return prop.GetValue(component, null)  as string;;
						}
					}
					return inputObject.name;
				}
				return "";
			}
			
			set 
			{
				if (inputObject != null) 
				{
					Component[] components = inputObject.GetComponents(typeof(Component));
					foreach (Component component in components)
					{
						PropertyInfo prop = component.GetType().GetProperty("text", BindingFlags.Instance | BindingFlags.Public);
						if (prop != null)
						{
							prop.SetValue(component, value, null);
							return;
						}
					}
					inputObject.name = value;
				}
			}
		}

		#region Keyboard Receiving Input

		public void SendKeyString(string keyString)
		{
			if (keyString.Length == 1 && keyString[0] == 8/*ASCII.Backspace*/)
			{
				if (text.Length > 0)
				{
					text = text.Remove(text.Length - 1); 
				}
			}
			else
			{
				text += keyString;
			}

			// Workaround: Restore focus to input fields (because Unity UI buttons always steal focus)
			ReactivateInputField(inputObject.GetComponent<InputField>());

		}

		public void CloseKeyboard()
		{
            gameObject.SetActive(false);
			//Destroy(gameObject);
		}

        public void OpenKeyboard()
        {
            gameObject.SetActive(true);
        }

		#endregion

        public string getSuggestion()
        {
            SuggestAPIResponse response = SuggestAPI.GetSuggestions(text);
            string[] suggestion = response.suggestions;

            int i = 0;
            foreach (string s in suggestion)
            {
                i++;
                Debug.Log("suggestion"+i+": " + s);
            }

            Debug.Log("Error: " + response.error);

            if (outputfield != null && response.error == "None" && suggestion.Length>0)
            {
                Component[] components = outputfield.GetComponents(typeof(Component));
                foreach (Component component in components)
                {
                    PropertyInfo prop = component.GetType().GetProperty("text", BindingFlags.Instance | BindingFlags.Public);
                    if (prop != null)
                    {
                        string currentText = prop.GetValue(component, null) as string;
                        //string word = suggestion[suggestion.Length-1];
                        string word = suggestion[0]; //Take the first instead of the last
                        prop.SetValue(component, currentText + " " + word,null);
                        return word;
                    
                    }
                }
                
            }
            return null;
        }


		#region Steal Focus Workaround

		void ReactivateInputField(InputField inputField)
		{
			if (inputField != null)
			{
				StartCoroutine(ActivateInputFieldWithoutSelection(inputField));
			}
		}

		IEnumerator ActivateInputFieldWithoutSelection(InputField inputField)
		{
			inputField.ActivateInputField();

			// wait for the activation to occur in a lateupdate
			yield return new WaitForEndOfFrame();

			// make sure we're still the active ui
			if (EventSystem.current.currentSelectedGameObject == inputField.gameObject)
			{
				// To remove hilight we'll just show the caret at the end of the line
				inputField.MoveTextEnd(false);
			}
		}

		#endregion

	}
}