using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine.EventSystems;

public class SwypeController : MonoBehaviour
{

    private List<char>  charList;
    private readonly int nTopSuggestions = 3;
    private string[] topSuggestions;
    public GameObject[] SuggestionFields;
    private List<char> text;
    private List<float> durations;
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
        text = new List<char>();
        SuggestionFields = FindSuggestionFields();
        anchorOld = -1;
        focusOld = -1;
        topSuggestions = new string[nTopSuggestions];
        isTyping = false;
        EventSystem.current.SetSelectedGameObject(OutputField.gameObject, null);
        OutputField.OnPointerClick(new PointerEventData(EventSystem.current));
        OutputField.caretPosition = 0;
        durations = new List<float>();
    }

    private GameObject[] FindSuggestionFields()
    {
        List<GameObject> fields = GameObject.FindGameObjectsWithTag("SuggestionField").ToList();
        return fields.OrderBy(go => go.name).ToArray<GameObject>();
    }

    public void Update()
    {
        GetHighlightedText();
    }

	
     public void AddCharacter(char character, float duration)
    {
        charList.Add(character);
        InputString.text = new string(charList.ToArray());
        if (duration > 0)
            durations.Add(duration);
    }

    public void clearChars()
    {
        if (charList.Count != 0) //sometimes happens with backspace
            EndOfInput();
        else
            charList.Clear();
    }

    public void Typing(char character)
    {
        if (!isTyping) //first character
        {
            charList.Add(character);
            text.Add(character);
            isTyping = true;
            WriteText();
        }
        else if(isTyping)
        {
            charList.Add(character);
            text.Add(character);
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
        
        Debug.Log("input string: ");
        string[] suggestions = new string[0];

        if (input.Length > 1)
        {
            bool allCaps = input.All(c => char.IsUpper(c));
            string[] words = getWords();
            string lastWord = "";
            Debug.Log(words.Length+ "Content" + words[0] +  ";");
            if (words.Length > 1)//words always containts an empty string for some reason
            {
                lastWord = words[words.Length - 2];
            }
			input+= "*" + FloatToString(durations) + "*" +  lastWord; //character string + durations + last word
            durations.Clear();
			
            SuggestAPIResponse response = SuggestAPI.GetSuggestions(input.ToLower());//send durations
            for (int i = 0; i < durations.Count; i++)
            {
                //Debug.Log("Character: " + input[i] + " Time: " + durations[i]);
            }
            durations.Clear();

            if(allCaps) //make lowercase response uppercase again
            {
                suggestions = response.suggestions.ToList().ConvertAll(s => s.ToUpper()).ToArray();
            }
            else 
                suggestions = response.suggestions;
        }
        else if (input.Length == 1)// user types 1 single character
        {
            text.Add(input[0]);
            WriteText();
            return topSuggestions;
        }
        else
            return null; //nothing is entered, prevents null pointers later on. 
        
        for (int i = 0; i < nTopSuggestions; i++)
        {
            if (suggestions.Length > i)
            {
                string suggestion = suggestions[i] + " ";//adding spacing
                SetText(SuggestionFields[i], suggestion);
                topSuggestions[i] = suggestion;

            }

            else
            {
                topSuggestions[i] = "";
                SetText(SuggestionFields[i], "");
            }
        }
        text.AddRange(topSuggestions[0].ToCharArray());//first and most likely suggestion
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
        string output = new string(text.ToArray());

        OutputField.text = output;
        OutputField.caretPosition = OutputField.text.Length;
    }

    public void setSuggestion(int index)
    {
        if(SuggestionFields[index].GetComponentInChildren<Text>().text != "")
        {
            string[] words = getWords();
            string LastWord = words[words.Length - 2];//last space also included
            foreach (string s in words)
                Debug.Log("Word:" + s + ";");
            Debug.Log(text.Count - 1 - LastWord.Length + " " + LastWord.Length);
            text.RemoveRange(text.Count - 1 - LastWord.Length, LastWord.Length + 1); //remove some extra cause of " " spacing
            text.AddRange((topSuggestions[index]).ToCharArray());
            Debug.Log("Suggestion:" + topSuggestions[index] + ";");

            WriteText();
        }

    }

    private void clearSuggestions()
    {
        foreach(GameObject field in SuggestionFields)
        {
            SetText(field, "");
        }
    }


    public string[] getWords()
    {
        string fullText = new string(text.ToArray());
        return fullText.Split(' ');
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

    //Unused
    public void SelectText(Vector3 pos)
    {
        Debug.Log("Postion: " + pos.ToString());
        float width = OutputField.GetComponent<RectTransform>().rect.width;
        float height = OutputField.GetComponent<RectTransform>().rect.height;
        //Debug.Log("Rect pos: " + OutputField.GetComponent<RectTransform>().transform.InverseTransformPoint(pos));
        //Debug.Log("width: " + width + " height:  " + height);
        Vector2 selectPos = new Vector2(pos.x + width / 2, pos.y + height / 2);
        //Debug.Log("Selected position : " + selectPos);
    }

    public void backspace()
    {
        text.RemoveAt(text.Count - 1);
        clearSuggestions();
        WriteText();
    }

    private string FloatToString(List<float> floats)
    {
        string s = "";
        foreach (float f in floats)
        {
            s += f.ToString() + ";";
        }
        return s;

    }


}
