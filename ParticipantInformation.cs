using System.Collections;
using UnityEngine;

public class ParticipantInformation{
    public string Username;
    public string ParticipantID;
    public Texture2D Avatar;
    public bool IsLocal;

	public ParticipantInformation(string user, bool local){
		Username = user;
		IsLocal = local;
		Avatar = null;
	}

    public void MarkAsLocal(){
        IsLocal = true;
    }
    public void MarkAsRemote(){
        IsLocal = false;
    }
    public void SetID(string id){
        ParticipantID = id;
    }

    public void RequestAvatar(){
        if (!Avatar){
            while (!Social.localUser.image){
                Debug.Log("Loading User Image..");
                //wait a bit
            }

            Avatar = Social.localUser.image;
            Debug.Log("User image Loaded");
        }
    }
    public Sprite GetAvatarSprite(){
        return Sprite.Create(Avatar, new Rect(0, 0, Avatar.width, Avatar.height), Vector2.one / 2f);
    }

    public void Reset(){
        Username = "";
        IsLocal = false;
        Avatar =  null;
        ParticipantID = "";
    }
}
