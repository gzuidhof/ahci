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
    private string[] topSuggestions;
    public GameObject[] SuggestionFields;
    public List<string> text;
    public InputField OutputField;
    public InputField InputString;
    private int selectionBegin;
    private int selectionEnd;
    private int anchorOld;
    private int focusOld;
    private bool isTyping;

    

	// Use this for initialization
	void Start () {
        charList = new List<char>();
        text = new List<string>();
        SuggestionFields = GameObject.FindGameObjectsWithTag("SuggestionField");
        anchorOld = -1;
        focusOld = -1;
        topSuggestions = new string[nTopSuggestions];
        isTyping = false;



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

    public void Typing(char character)
    {
        if (!isTyping) //first character
        {
            charList.Add(character);
            text.Add(new string(charList.ToArray()));
            isTyping = true;
            WriteText();
        }
        else if(isTyping)
        {
            charList.Add(character);
            text[text.Count - 1] = new string(charList.ToArray());
            WriteText();
        }
        if (character == ' ')
        {
            isTyping = false;
            charList.Clear();
            InputString.text = "";
        } 

        
    }

    public string[] EndOfInput()
    {
        string input = new string(charList.ToArray());
        charList.Clear();
        
        Debug.Log("input string: " + input);
        string[] suggestions = new string[0];

        if (input.Length > 1)
        {
            SuggestAPIResponse response = SuggestAPI.GetSuggestions(input);
            suggestions = response.suggestions;
        }
        else if (input.Length == 1)// user types 1 single character
        {
            text.Add(input);
            WriteText();
            return topSuggestions;
        }
        else
            return null; //nothing is entered, prevents null pointers later on. 
        
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
        text.Add(topSuggestions[0]);//first and most likely suggestion
        WriteText();
        input = "";
        return topSuggestions; //maak hier later een empty list/array/enumarable van
    }

    private void SetText(GameObject field, string s)
    {
        if (field != null)
            field.GetComponentInChildren<Text>().text = s;
    }

    private void WriteText()
    {
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

    public void setSuggestion(int index)
    {
        Debug.Log(index);
        text[text.Count-1] = topSuggestions[index];
        WriteText();
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
