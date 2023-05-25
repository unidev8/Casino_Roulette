using Firesplash.UnityAssets.SocketIO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SockIOManage : MonoBehaviour
{
    public SocketIOCommunicator sioCom;
    public static SockIOManage instance;

    struct PlayerData
    {
        public string token;
    }

    // Start is called before the first frame update
    void Start()
    {
        sioCom.Instance.On("connect", (string data) => {
            Debug.Log("SocketIO connected");

            PlayerData playerData = new PlayerData();
            playerData.token = OthoMenu.instance.tokenValue;
            sioCom.Instance.Emit("enterRoom", JsonUtility.ToJson(playerData), false);
        });

        sioCom.Instance.On("GetServerResult", (string data) =>
        {
            //Debug.Log("myInfo:data = " + data);
            //MyInfo myInfo = JsonUtility.FromJson<MyInfo>(data);
            //SocketIOManager.instance.txt_FUN.GetComponent<TMP_Text>().text = myInfo.balance;
            //txt_ID.GetComponent<TMP_Text>().text = myInfo.username;
            //GameManager.instance.playerID = myInfo.userId;
        });
    }

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
