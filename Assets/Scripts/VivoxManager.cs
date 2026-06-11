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

        
    }

    public async void Login()
    {
        await VivoxService.Instance.LoginAsync();
        
    }

    public async void JoinVoice(string channelName)
    {
        await VivoxService.Instance.JoinGroupChannelAsync(
            channelName,
            ChatCapability.AudioOnly
        );

        
    }
}