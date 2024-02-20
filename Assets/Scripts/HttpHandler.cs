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
    public TextMeshProUGUI[] playerName;
    public TextMeshProUGUI[] characterName;
    public TextMeshProUGUI usernameDisplay;
    public int avatarIndex = 0;
    private int currentUserId = 1;

    public GameObject startButton;
    public GameObject nextButton;
    public GameObject prevButton;

    private string fakeApiUrl = "https://my-json-server.typicode.com/FakeUser123/JsonPlaceholder";
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
                usernameDisplay.text = "Player: " + user.username;

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
                playerName[avatarIndex].text = character.species;
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

    public void StartGame()
    {
        startButton.SetActive(false);
        nextButton.SetActive(true);
        prevButton.SetActive(true);
    }

    public void Next()
    {
        currentUserId++;
        if (currentUserId > 3)
            currentUserId = 1;
        StartCoroutine("FetchUserData", currentUserId);
    }

    public void Previous()
    {
        currentUserId--;
        if (currentUserId < 1)
            currentUserId = 3;
        StartCoroutine("FetchUserData", currentUserId);
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
    public string username;
    public int[] deck;
}