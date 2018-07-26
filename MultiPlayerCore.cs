// Author: Fabrizio Spadaro
// License Copyright 2018 (c)Fabrizio Spadaro
// https://twitter.com/F_adaro
// https://github.com/fabriziospadaro

using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;

public class MultiPlayerCore : RealTimeMultiplayerListener {
    
    private INetworkCallback Inetwork_callback;
    public NetworkMessagesCollection Net_Messages;

    public RPCManager RPC_Manager;

    public ParticipantInformation LocalPlayer;
    public List<ParticipantInformation> RemotePlayers;

    public CloudManager Cloud_Manager;

    private bool initialized;
    private bool authenticated;

    public PlayGamesPlatform PgamePlatform_instance;

  public MultiPlayerCore(INetworkCallback net_callback) {
        authenticated = false;
        Inetwork_callback = net_callback;
        Net_Messages = new NetworkMessagesCollection();
        RPC_Manager = new RPCManager(this);
        Cloud_Manager = new CloudManager("CloudData");
	}

    public void Login() {
        InitializeGooglePlayService(true);
    }

    public void Logout() {
        UnauthService();
    }

    public void FindGame(int min_players,int max_players, System.Collections.Hashtable gameoption) {
        CreateQuickGame((uint)min_players,(uint)max_players,0);
    }
    private void InitializeGooglePlayService(bool login) {

        Inetwork_callback.OnAuthenticating();
        if (!initialized){
            PlayGamesClientConfiguration cfg = new PlayGamesClientConfiguration.Builder().AddOauthScope("profile").EnableSavedGames().Build();

            PlayGamesPlatform.InitializeInstance(cfg);
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
            PgamePlatform_instance = PlayGamesPlatform.Instance;
            initialized = true;
        }
        if(login)
            AuthService();
    }

    private void UnauthService() {
        PgamePlatform_instance.SignOut();
        LocalPlayer.Reset();
        authenticated = false;
    }
    private void AuthService(){
        if (!IsAuthenticated()){
            Social.localUser.Authenticate((bool sucess) =>{

                if (sucess == true) {
                    Inetwork_callback.OnAuthenticationSucess();
                    authenticated = true;
                    LocalPlayer.RequestAvatar();

                    LocalPlayer = new ParticipantInformation(Social.localUser.userName, true);
                    Cloud_Manager.LoadData();
                }
                else{
                    Inetwork_callback.OnAuthenticationFail();
                    authenticated = false;
                }
                
            }
            );
        }
        else {
            Inetwork_callback.OnAuthenticationSucess();
            authenticated = true;
            LocalPlayer.RequestAvatar();

            Cloud_Manager.LoadData();
        }
    
    }
    public bool IsAuthenticated(){
        //return !Social.localUser.authenticated;
        return authenticated;
    }
    public void CreateQuickGame(uint minPlayers = 1,uint maxPlayers = 1,uint pLevel=1) {
        Inetwork_callback.OnRoomJoining();
        PgamePlatform_instance.RealTime.CreateQuickGame(minPlayers,maxPlayers,pLevel,this);
    }

    public void OnParticipantLeft(Participant participant){
        Inetwork_callback.OnRemotePlayerDisconnect();
    }

    public void OnPeersConnected(string[] participantIds){
    }

    public void OnPeersDisconnected(string[] participantIds){
    }
    
    public void OnRoomConnected(bool success){
        if (success) {
            Inetwork_callback.OnRoomJoinSucess();
            SyncPlayerList();
            SendAvatarWhenLoaded();
            FindRemoteAvatar();
        }
		else
            Inetwork_callback.OnRoomJoinFail();
    }

    public void OnRoomSetupProgress(float percent){
        //PgamePlatform_instance.RealTime.ShowWaitingRoomUI();
    }

    public void OnLeftRoom(){
        Inetwork_callback.OnLocaPlayerDisconnect();
    }

    public void LeaveRoom(){
        PgamePlatform_instance.RealTime.LeaveRoom();
        Net_Messages.Reset();
    }
    public void SyncPlayerList(){
        LocalPlayer.SetID(PgamePlatform_instance.RealTime.GetSelf().ParticipantId);
        RemotePlayers = new List<ParticipantInformation>(1);
        foreach (Participant p in PgamePlatform_instance.RealTime.GetConnectedParticipants()){
            if (p.ParticipantId != LocalPlayer.ParticipantID){
                ParticipantInformation pi = new ParticipantInformation(p.DisplayName, false);
                pi.SetID(p.ParticipantId);
                RemotePlayers.Add(pi);
            }
        }
    }

    public int GetSeed(){
        //il seed è formato dall' hashcode del partecipantid di tutti i giocatori
        int seed = 0;
        seed += LocalPlayer.ParticipantID.GetHashCode();
        foreach (ParticipantInformation p in RemotePlayers)
            seed += p.ParticipantID.GetHashCode();

        return seed;
    }

    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data){
        Debug.LogWarning("Recived data of " + data.Length + "bytes");
        Net_Messages.ProcessMessage(isReliable,senderId,data);
    }

    private void FindRemoteAvatar() {
        foreach(ParticipantInformation p in RemotePlayers) {
            while(p.Avatar == null) {
                RPC_Manager.UpdateVariable(ref p.Avatar, "Avatar", p.ParticipantID);
                //wait seconds
            }
        }
    }
    private void SendAvatarWhenLoaded() {
        //wait till the localplayer avatar is loaded
        while(LocalPlayer.Avatar == null) {
            //wait seconds
        }
        RPC_Manager.Send(RPCManager.RPCOption.Everyone, new MessageInformation("Avatar",LocalPlayer.Avatar));
    }

}
