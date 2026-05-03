using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Vivox;

public class VivoxManager : MonoBehaviour
{
    public static VivoxManager Instance;

    async void Awake()
    {
        Instance = this;

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        await VivoxService.Instance.InitializeAsync();

        Debug.Log("Vivox listo");
    }

    public async void Login()
    {
        await VivoxService.Instance.LoginAsync();
        Debug.Log("Logueado en Vivox");
    }

    public async void JoinVoice(string channelName)
    {
        await VivoxService.Instance.JoinGroupChannelAsync(
            channelName,
            ChatCapability.AudioOnly
        );

        Debug.Log("Entraste al canal: " + channelName);
    }
}