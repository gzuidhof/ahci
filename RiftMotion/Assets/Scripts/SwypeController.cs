using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;


public class SwypeController : MonoBehaviour
{

    private List<char>  charList;
    private readonly int nTopSuggestions = 3;
    public GameObject[] SuggestionFields;
    public List<string> text;
    public InputField OutputField;
    public InputField InputString;
    private int selectionBegin;
    private int selectionEnd;
    private int anchorOld;
    private int focusOld; 

    

	// Use this for initialization
	void Start () {
        charList = new List<char>();
        text = new List<string>();
        SuggestionFields = new GameObject[nTopSuggestions];
        for (int i = 0; i < nTopSuggestions; i++)
        {
            string tag = "SuggestionField" + i;
            SuggestionFields[i] = GameObject.FindGameObjectWithTag((tag));
            //SetText(SuggestionFields[i], "Suggestion");
        }
        anchorOld = -1;
        focusOld = -1;



	}

    public void Update()
    {
        GetHighlightedText();
    }

	
     public void AddCharacter(char character)
    {
        charList.Add(character);
        InputString.text = new string(charList.ToArray());
    }

    public string[] EndOfInput()
    {
        string[] topSuggestions = new string[nTopSuggestions];//not sure wat we hier mee willen doen, maar komt vast ooit van pas
        string input = new string(charList.ToArray());
        charList.Clear();
        
        Debug.Log("input string: " + input);
        string[] suggestions = new string[0];

        if (input.Length > 1)
        {
            SuggestAPIResponse response = SuggestAPI.GetSuggestions(input);
            suggestions = response.suggestions;
        }
        else if(input.Length == 1)// user types 1 single character
        {
            addToTextField(input);
            return topSuggestions;
        }


        

        
        
        for (int i = 0; i < nTopSuggestions; i++)
        {
            if (suggestions.Length > i)
            {
                string suggestion = suggestions[i];
                SetText(SuggestionFields[i], suggestion);
                topSuggestions[i] = suggestion;

            }

            else
            {
                topSuggestions[i] = "";
                SetText(SuggestionFields[i], "");
            }
        }
        addToTextField(topSuggestions[0]);
        input = "";
        return topSuggestions; //maak hier later een empty list/array/enumarable van
    }

    private void SetText(GameObject field, string s)
    {
        if(field != null)
            field.GetComponent<Text>().text = s;
        /*
        Component[] components = field.GetComponents(typeof(Component));
        foreach (Component component in components)
        {
            PropertyInfo prop = component.GetType().GetProperty("text", BindingFlags.Instance | BindingFlags.Public);
            if (prop!= null)
            {
                prop.SetValue(component, s, null);
            }

        }
        */

    }

    private void addToTextField(string nextWord)
    {
        text.Add(nextWord);
        string output = "";
        foreach(string s in text)
        {
            Debug.Log(s);
            if (s.Length > 1)
                output += s + " ";
            else
                output += s;
        }
        Debug.Log("Total text: " + output);

        OutputField.text = output;
    }

    public void GetHighlightedText()
    {
        if (OutputField)//Outputfield sometimes throws a nullreference exception for no apperent reason
        {
            int anchor = OutputField.selectionAnchorPosition;
            int focus = OutputField.selectionFocusPosition;

            if (anchor != anchorOld || focus != focusOld)
            {
                if (anchor < focus)
                {
                    //dragging from left to right
                    selectionBegin = anchor;
                    selectionEnd = focus;
                }
                else
                {
                    //dragging from right tot left
                    selectionBegin = focus;
                    selectionEnd = anchor;
                }

                anchorOld = anchor;
                focusOld = focus;

                Debug.Log("Selctionbegin: " + selectionBegin + " SelectionEnd: " + selectionEnd);
                Debug.Log("Anchor: " + OutputField.selectionAnchorPosition + " Focus: " + OutputField.selectionFocusPosition);
                Debug.Log("Selected: " + new string(OutputField.text.Skip(selectionBegin).Take(selectionEnd - selectionBegin).ToArray()));
            }

        }                 
        
    }

}
