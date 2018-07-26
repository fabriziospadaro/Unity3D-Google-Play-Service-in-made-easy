// Author: Fabrizio Spadaro
// License Copyright 2018 (c)Fabrizio Spadaro
// https://twitter.com/F_adaro
// https://github.com/fabriziospadaro

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NetworkMessagesCollection{
    //keyvaluepair of senderid-msginfromation
    List<KeyValuePair<string, MessageInformation>> Messages;

    public NetworkMessagesCollection(){
        Messages = new List<KeyValuePair<string, MessageInformation>>();
    }
    public void ProcessMessage(bool isReliable, string senderID, byte[] data){
        
        string decodDATA = System.Text.Encoding.Default.GetString(data);
        string[] splitdata = decodDATA.Split(new string[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);

        //VARIABLE DECODIFICATION TEMPLATE
        //[0]= TYPE
        //[1]= NAME 
        //[2][3][4][N..] VALUE

        if (splitdata[0] == "string")
            AddMessage(senderID, splitdata[1], splitdata[2]);
        else if (splitdata[0] == "int")
            AddMessage(senderID, splitdata[1], System.Convert.ToInt32(splitdata[2]));
        else if (splitdata[0] == "float")
            AddMessage(senderID, splitdata[1], GetFloat(splitdata[2]));
        else if (splitdata[0] == "Vector3"){
            Vector3 v = new Vector3();
            v.x = GetFloat(splitdata[2]);
            v.y = GetFloat(splitdata[3]);
            v.z = GetFloat(splitdata[4]);
            AddMessage(senderID, splitdata[1], v);
        }
        else if (splitdata[0] == "Vector2"){
            Vector2 v = new Vector2();
            v.x = GetFloat(splitdata[2]);
            v.y = GetFloat(splitdata[3]);
            AddMessage(senderID, splitdata[1], v);
        }
        else if (splitdata[0] == "bool"){
            bool b = (splitdata[2].ToLower() == "true") ? true : false;
            AddMessage(senderID, splitdata[1], b);
        }
        else if (splitdata[0] == "Texture2D"){
            Texture2D t = new Texture2D(1, 1);

            byte[] pixeldata = Convert.FromBase64String(splitdata[2]);
            t.LoadImage(pixeldata);
            t.Apply();
            AddMessage(senderID, splitdata[1], t);
        }
    }
    public void AddMessage(string SenderID, string Key, object Value){
        Messages.Add(new KeyValuePair<string, MessageInformation>(SenderID, new MessageInformation(Key, Value)));
    }
    public object GetValuesOf(string variableName, string senderID){
        //return the list of varables name sent by a specific ID
        foreach (KeyValuePair<string, MessageInformation> o in Messages)
            if ((string)o.Key == senderID && variableName == o.Value.name){
                object outo = o.Value.value;
                ClearVariable(o.Value, senderID);
                return outo;
            }

        return null;
    }
    public void ClearVariable(MessageInformation msg, string senderID){
        //clear all the values of that variable
        Messages.Remove(new KeyValuePair<string, MessageInformation>(senderID, msg));
    }

    public void Reset() {
        Messages.Clear();
    }
    float GetFloat(string s){
        return float.Parse(s, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
    }

}
