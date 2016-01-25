using UnityEngine;
using System.Collections;
using SimpleJSON;


public struct SuggestAPIResponse
{
    public string query;
    public string[] suggestions;
    public string error;

    public SuggestAPIResponse(string query, string[] suggestions, string error)
    {
        this.query = query;
        this.suggestions = suggestions;
        this.error = error;
    }

    public override string ToString()
    {
        return string.Format("SuggestAPIResponse<Query({0}), Error:({1}), Suggestions({2})>", query, error, string.Join(",",suggestions));
    }
}

public class SuggestAPI : MonoBehaviour {

    //Singleton instance
    public static SuggestAPI instance;

    public int port = 5000;
    public string host = "127.0.0.1";

    void Awake()
    {
        instance = this;
    }
	
	public static SuggestAPIResponse GetSuggestions(string query)
    {
        string url = "http://"+instance.host + ":" + instance.port + "/suggest?query=" + query;

        Debug.Log("Making request: " + url);

        WWW www = instance.GET(url);
        var jsonResponse = JSON.Parse(www.text);
        Debug.Log(jsonResponse.ToString());
        var suggestionsN = jsonResponse["suggestions"][1].Count;
        Debug.Log(suggestionsN);
        string[] suggestions = new string[suggestionsN];

        for (int i = 0; i < suggestionsN; i++)
        {
            suggestions[i] = jsonResponse["suggestions"][1].AsArray[i];
        }

        SuggestAPIResponse response = new SuggestAPIResponse(jsonResponse["query"], suggestions, jsonResponse["error"]);

        return response;

    }

    void Start()
    {
        //var sug = SuggestAPI.GetSuggestions("asdfgrtyuijhvcvghuiklkjuytyuytre"); //agriculture
        //Debug.Log(sug);
    }

    public WWW GET(string url)
    {

        WWW www = new WWW(url);
        while (!www.isDone) { }
        // check for errors
        if (www.error != null)
        {
            Debug.Log("WWW Error: " + www.error);
        }

        return www;
    }
  
}
