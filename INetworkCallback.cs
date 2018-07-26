// Author: Fabrizio Spadaro
// License Copyright 2018 (c)Fabrizio Spadaro
// https://twitter.com/F_adaro
// https://github.com/fabriziospadaro

public interface INetworkCallback {
    //authentication callbacks
    void OnAuthenticating();
    void OnAuthenticationFail();
    void OnAuthenticationSucess();
    //room joining callbacks
    void OnRoomJoining();
    void OnRoomJoinSucess();
    void OnRoomJoinFail();

    //local player callbacks
    void OnLocaPlayerDisconnect();
    void OnLocalPlayerConnect();
    //remote player callbacks 
    void OnRemotePlayerDisconnect();
    void OnRemotePlayerConnect();
}