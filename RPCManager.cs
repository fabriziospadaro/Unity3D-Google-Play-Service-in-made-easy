using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPCManager{
    private MultiPlayerCore MC;
    public enum RPCOption { Everyone, Others };

    public RPCManager(MultiPlayerCore core) {
        MC = core;
    }

    public void UpdateVariable(ref int var,string varname,string Sender_id) {
        object o = MC.Net_Messages.GetValuesOf(varname, Sender_id);

        if (o != null)
            var = (int)o;
    }
  public void UpdateVariable(ref string var, string varname, string Sender_id){
    object o = MC.Net_Messages.GetValuesOf(varname, Sender_id);

        if (o != null)
            var = (string)o;
    }
    public void UpdateVariable(ref bool var, string varname, string Sender_id){
      object o = MC.Net_Messages.GetValuesOf(varname, Sender_id);

        if (o != null)
            var = (bool)o;
    }
    public void UpdateVariable(ref Vector2 var, string varname, string Sender_id){
      object o = MC.Net_Messages.GetValuesOf(varname, Sender_id);
      
      if (o != null)
            var = (Vector2)o;
    }
    public void UpdateVariable(ref Vector3 var, string varname, string Sender_id){
      object o = MC.Net_Messages.GetValuesOf(varname, Sender_id);

        if (o != null)
            var = (Vector3)o;
    }
    public void UpdateVariable(ref float var, string varname, string Sender_id){
      object o = MC.Net_Messages.GetValuesOf(varname, Sender_id);

        if (o != null)
            var = (float)o;
    }
    public void UpdateVariable(ref Texture2D var, string varname, string Sender_id){
      object o = MC.Net_Messages.GetValuesOf(varname, Sender_id);

        if (o != null)
            var = (Texture2D)o;
    }

    public void Send(RPCOption mode, MessageInformation message, bool reliable = true){
        string outdata = "";
        System.Type t = message.value.GetType();
        //CODIFICA IN BASE AL TIPO DI INPUT
        if (t.Equals(typeof(string)))
            outdata = "string:" + message.name + ":" + message.value;
        else if (t.Equals(typeof(int)))
            outdata = "int:" + message.name + ":" + message.value.ToString();
        else if (t.Equals(typeof(float)))
            outdata = "float:" + message.name + ":" + message.value.ToString();
        else if (t.Equals(typeof(Vector2)))
            outdata = "Vector2:" + message.name + ":" + ((Vector2)message.value).x + ":" + ((Vector2)message.value).y;
        else if (t.Equals(typeof(Vector3)))
            outdata = "Vector3:" + message.name + ":" + ((Vector3)message.value).x + ":" + ((Vector3)message.value).y + ":" + ((Vector3)message.value).z;
        else if (t.Equals(typeof(bool)))
            outdata = "bool:" + message.name + ":" + message.value.ToString();
        else if (t.Equals(typeof(Texture2D))){
            Texture2D te = ((Texture2D)message.value);
            outdata = "Texture2D:" + message.name + ":";
            byte[] pixelsdata = te.EncodeToJPG();
            outdata += System.Convert.ToBase64String(pixelsdata);
        }
        else{
            Debug.LogError("data type not supported");
            return;
        }

        byte[] dataBYTE = System.Text.ASCIIEncoding.Default.GetBytes(outdata);
        Debug.LogWarning("Sent data of " + dataBYTE.Length + "bytes");

        switch (mode){
            case RPCOption.Everyone:
                MC.PgamePlatform_instance.RealTime.SendMessageToAll(reliable, dataBYTE);
                break;
        }
    
    }
        

}
