namespace CED_Roulette
{
    using Firesplash.UnityAssets.SocketIO;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class SockIOManage : MonoBehaviour
    {
        public SocketIOCommunicator sioCom;
        public static SockIOManage instance;
        [HideInInspector]
        public GameResult gameResult;
        [HideInInspector]
        public float fun;

        [System.Serializable]
        class UserInfo
        {
            public string avatarUrl;
            public string nickname;
            public int balance;
        }

        [System.Serializable]
        struct GeneralTxt
        {
            public string txt;
        }

        // Start is called before the first frame update
        void Start()
        {
            sioCom.Instance.On("connect", (string data) => {
                Debug.Log("SocketIO connected");

                PlayerData playerData = new PlayerData();
                playerData.txt = OthoMenu.instance.tokenValue;
                sioCom.Instance.Emit("user-info", JsonUtility.ToJson(playerData), false);
            });

            sioCom.Instance.On("need-recharge-balance", (string data) =>
            {
                Debug.Log("need-recharge-balance!");
                GeneralTxt generalTxt = JsonUtility.FromJson<GeneralTxt>(data);
                OthoMenu.instance.pan_Refund.SetActive(true);
                //OthoMenu.instance.txt_Refund.GetComponent <TMP_Text>().text = generalTxt.txt;
                OthoMenu.instance.txt_Refund.GetComponent<TMP_Text>().text = "https://induswin.com/#/pages/recharge/recharge";
            });

            sioCom.Instance.On("game-result", (string data) =>
            {
                gameResult = JsonUtility.FromJson<GameResult>(data);
                Debug.Log("game-result = " + data + "gameRestult.analyze" + gameResult.analyze.ToString());
                OthoMenu.instance.serverWinPoint = gameResult.wedge;
                OthoMenu.instance.trigerSpinBet = true;
            });

            sioCom.Instance.On("user-info", (string data) =>
            {
                UserInfo userInfo = JsonUtility.FromJson<UserInfo>(data);                
                fun = userInfo.balance;
                Debug.Log("received fun = " + fun);

                if (OthoMenu.instance.isSpinBet) return;

                OthoMenu.instance.txt_Cach.GetComponent<TMP_Text>().text = userInfo.balance.ToString();
                if (userInfo.balance >= 0.1f && OthoMenu.instance.pan_Refund.activeSelf)
                {
                    OthoMenu.instance.pan_Refund.SetActive(false);
                }
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

    [System.Serializable]
    public class GameResult
    {
        public int wedge;
        public int score;
        public int[] analyze;
        public int totalBetAmount;
    }

    [System.Serializable]
    public struct PlayerData
    {
        public string txt;
    }
}
