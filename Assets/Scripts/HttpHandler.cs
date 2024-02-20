using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
public class HttpHandler : MonoBehaviour
{
    public RawImage[] avatars;
    public TextMeshProUGUI[] Type;
    public TextMeshProUGUI[] characterName;
    public TextMeshProUGUI usernameDisplay;
    public int avatarIndex = 0;
    private int currentUserId = 1;

    private string fakeApiUrl = "https://my-json-server.typicode.com/juandagongoradiaz/API_EntregaJDG";
    private string RickAndMortyUrl = "https://rickandmortyapi.com/api";

    public void InitiateRequest()
    {
        StartCoroutine("FetchUserData", currentUserId);
    }

    IEnumerator FetchUserData(int userId)
    {
        UnityWebRequest request = UnityWebRequest.Get(fakeApiUrl + "/users/" + userId);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                UserData user = JsonUtility.FromJson<UserData>(request.downloadHandler.text);
                usernameDisplay.text = "Player: " + user.name;

                foreach (int card in user.deck)
                {
                    StartCoroutine("FetchCharacter", card);
                }
                avatarIndex = 0;
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
    }

    IEnumerator FetchCharacter(int id)
    {
        UnityWebRequest request = UnityWebRequest.Get(RickAndMortyUrl + "/character/" + id);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                CharacterData character = JsonUtility.FromJson<CharacterData>(request.downloadHandler.text);
                characterName[avatarIndex].text = character.name;
                Type[avatarIndex].text = character.species;
                StartCoroutine(DownloadAvatar(character.image, avatarIndex));
                avatarIndex++;
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
    }

    IEnumerator DownloadAvatar(string url, int index)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            avatars[index].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    public void FetchUserDataFromButton(int userId)
    {
        if (currentUserId != userId)
            {

            currentUserId = userId;
            StartCoroutine("FetchUserData", currentUserId);

        }
       
    }
}

[System.Serializable]
public class JsonData
{
    public CharacterData[] results;
    public UserData[] users;
}

[System.Serializable]
public class CharacterData
{
    public int id;
    public string name;
    public string species;
    public string image;
}

[System.Serializable]
public class UserData
{
    public int id;
    public string name;
    public int[] deck;
}