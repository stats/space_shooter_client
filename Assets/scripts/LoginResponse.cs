using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LoginResponse
{
    public string success;
    public string token;
    public long expiresIn;
    public string username;

    public static LoginResponse CreateFromJSON(string jsonString)
    {
        LoginResponse response = JsonUtility.FromJson<LoginResponse>(jsonString);
        response.expiresIn = System.DateTimeOffset.Now.ToUnixTimeMilliseconds() + (response.expiresIn * 1000);
        return response;
    }
}