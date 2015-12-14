using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;

public class SwypeController : MonoBehaviour
{

    private List<char>  charList;
    private readonly int nTopSuggestions = 3;
    public GameObject OutputField;
    public GameObject[] SuggestionFields;
    public List<string> text;
    

	// Use this for initialization
	void Start () {
        charList = new List<char>();
        text = new List<string>();
        SuggestionFields = new GameObject[nTopSuggestions];
        OutputField = GameObject.FindGameObjectWithTag("TextOutput");
        for (int i = 0; i < nTopSuggestions; i++)
        {
            string tag = "SuggestionField" + i;
            SuggestionFields[i] = GameObject.FindGameObjectWithTag((tag));
            SetText(SuggestionFields[i], "Suggestion");
        }



	}
	
     public void AddCharacter(char character)
    {
        charList.Add(character);
    }

    public string[] EndOfInput()
    {
        string input = new string(charList.ToArray());
        charList.Clear();
        
        Debug.Log("input string: " + input);

        SuggestAPIResponse response = SuggestAPI.GetSuggestions(input);
        string[] suggestions = response.suggestions;

        
        string[] topSuggestions = new string[nTopSuggestions];//not sure wat we hier mee willen doen, maar komt vast ooit van pas
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
        Component[] components = field.GetComponents(typeof(Component));
        foreach (Component component in components)
        {
            PropertyInfo prop = component.GetType().GetProperty("text", BindingFlags.Instance | BindingFlags.Public);
            if (prop!= null)
            {
                prop.SetValue(component, s, null);
            }

        }

    }

    private void addToTextField(string nextWord)
    {
        text.Add(nextWord);
        string output = "";
        foreach(string s in text)
        {
            Debug.Log(s);
            output += s + " ";
        }
        Debug.Log("Total text: " + output);

        SetText(OutputField, output);
    }

}
