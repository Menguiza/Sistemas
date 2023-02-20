using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class ApiManager : MonoBehaviour
{
    public List<Character> characters { get; private set; }
    public UnityEvent charactersLoaded;
    public static ApiManager manager;
    public int[] deck = { 1, 32, 358, 578, 601 };

    [SerializeField] private TMP_InputField field;
    private string api = "https://rickandmortyapi.com/api/character/";
    private int index = 0;

    private void Awake()
    {
        characters = new List<Character>();
        manager = this;
    }

    public void Request()
    {
        characters.Clear();
        GetCharactersMethod(deck);
    }

    public void GetCharactersMethod(int[] cards)
    {
        StartCoroutine(GetCharacter(cards));
    }

    public void SetImage(int index, RawImage img)
    {
        StartCoroutine(DownloadImage(characters.ElementAt(index).image, img));
    }
    
    IEnumerator GetCharacter(int[] cards) 
    {
        UnityWebRequest www = UnityWebRequest.Get(api + cards[index]);
        yield return www.SendWebRequest();
 
        if(www.result != UnityWebRequest.Result.Success) 
        {
            Debug.Log("Connection Error: " + www.error);
        }
        else 
        {
            if (www.responseCode == 200)
            {
                string responseText = www.downloadHandler.text;

                Character character = JsonUtility.FromJson<Character>(responseText);
                characters.Add(character);
            }
            else
            {
                string message = "Status: " + www.responseCode;
                message += "\nContent-type: " + www.GetResponseHeader("content-type");
                message += "\nError: " + www.error;
                Debug.Log(message);
            }
        }

        if (index < 4)
        {
            index++;
            StartCoroutine(GetCharacter(cards));
        }
        else
        {
            index = 0;
            charactersLoaded.Invoke();
            StopCoroutine(GetCharacter(deck));
        }
    }
    
    IEnumerator DownloadImage(string url, RawImage targetTxt)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            targetTxt.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string species;
    public string image;
}