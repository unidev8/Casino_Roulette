namespace CED_Roulette
{
    using DG.Tweening;
    using DG.Tweening.Core.Easing;
    using DG.Tweening.Plugins;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class OthoMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private GameObject txt_TotalBet;
        public GameObject txt_Cach;
        [SerializeField]
        private GameObject txt_Win;
        [SerializeField]
        private GameObject btn_PayTable;
        [SerializeField]
        private GameObject pan_WholeBoard;
        [SerializeField]
        private GameObject btn_Rollet;
        [SerializeField]
        private GameObject btn_Neighbour;
        [SerializeField]
        private GameObject btn_Back;
        [SerializeField]
        private GameObject btn_Back_2;
        [SerializeField]
        private GameObject btn_Undo;
        [SerializeField]
        private GameObject btn_Clear;
        [SerializeField]
        private GameObject btn_Double;
        [SerializeField]
        private GameObject btn_Rebet;
        [SerializeField]
        private GameObject btn_RebetAndSpin;
        [SerializeField]
        private GameObject txtr_Render;
        [SerializeField]
        private GameObject obj_CanvasWorld;
        [SerializeField]
        private GameObject obj_CanvasScreen;
        [SerializeField]
        private GameObject scr_InsideNumG_01;
        [SerializeField]
        private GameObject scr_InsideNumG_02;
        [SerializeField]
        private GameObject scr_InsideNumG_03;
        [SerializeField]
        private GameObject src_InsideNumG2_0;
        [SerializeField]
        private GameObject scr_InsideNumG2_1;
        [SerializeField]
        private GameObject scr_InsideNumG2_2;
        [SerializeField]
        private GameObject scr_InsideNumG2_3;
        [SerializeField]
        private GameObject scr_InsideNumG2_4;
        [SerializeField]
        private GameObject scr_InsideNumG2_5;
        [SerializeField]
        private GameObject scr_InsideNumG2_6;
        [SerializeField]
        private GameObject src_InsideNumG2_7;
        [SerializeField]
        private GameObject txt_neighbourCount;
        [SerializeField]
        private GameObject pan_Analystic;
        [SerializeField]
        private GameObject txt_Value;
        [SerializeField]
        private GameObject pan_SquareControlGroup;
        [SerializeField]
        private GameObject pan_RoundControlGroup;
        [SerializeField]
        private GameObject pan_NeighbourCounterGroup;
        [SerializeField]
        private GameObject opt_SwitchBtton;
        [SerializeField]
        private GameObject grp_ViewMode;
        [SerializeField]
        private GameObject grp_PresetBets;
        [SerializeField]
        private GameObject grp_LastedNum;
        [SerializeField]
        private GameObject grp_HotColdNum;
        [SerializeField]
        private GameObject grp_RedBlack;
        [SerializeField]
        private GameObject grp_OddEven;
        [SerializeField]
        private GameObject grp_Dozen;
        [SerializeField]
        private GameObject grp_Column;
        [SerializeField]
        private GameObject img_Rotate;

        [SerializeField]
        private GameObject prefabChip;
        [SerializeField]
        private Sprite spriteChip_B;
        [SerializeField]
        private Sprite spriteChip_M;
        [SerializeField]
        private Sprite spriteChip_Y;
        [SerializeField]
        private Sprite spriteChip_R;
        [SerializeField]
        private Sprite spriteChip_G;
        [SerializeField]
        private Sprite spriteChip_MB;
        [SerializeField]
        private Camera canvasCam;
        [SerializeField]
        private GameObject winMark;
        [SerializeField]
        private GameObject winPan;
        [SerializeField]
        private GameObject txt_WinPan;
        [SerializeField]
        private GameObject pan_AutoSpin;
        [SerializeField]
        private GameObject btn_AutoSpin;
        private int nAutoCount = 0, currentAutoCount = 0;

        [HideInInspector]
        public bool tiggerWinStat = false;
        private bool beginBet = false; // false : there is no chip, true: there are one more chips.
        [HideInInspector]
        public bool isSpinBet = false;
        [HideInInspector]
        public bool trigerSpinBet = false;
        private enum boardPanState
        {
            ViewSpine,
            ViewSquare,
            ViewRound
        }
        private boardPanState currentPanState = boardPanState.ViewSquare;

        private bool isScreenSpaceMenu = true;
        [HideInInspector]
        public int winNum;
        private Vector3 realtimeMousePos;
        private PointerEventData canvasEventData;
        private byte neighbourCount = 1;
        private string[] snakeOfIdx = { "05", "24", "16", "33","01", "20", "14", "31","09", "22", "18", "29","07", "28", "12", "35","03", "26", "00",
                "32","15", "19", "04", "21","02","25", "17", "34", "06","27","13", "36", "11", "30","08","23", "10"};

        private float[] betValues = { 0.1f, 0.5f, 1f, 5f, 10f, 25f };
        private sbyte curBetValuesIdx = 2;// index of current BetValuse
        private byte chipPos; // positon in button. ex: center 0, left 1, top 2, right 3, down 4, lefttop 5, righttop 6. rightdown 7, leftdown 8
        [HideInInspector]
        public Dictionary<string, Chip> dicChipObjects = new Dictionary<string, Chip>();// to send server and delete the chip value to undo historically. 
        private int historyIndex = 0;

        private class ChipHistory
        {
            public int historyIdx;
            public string chipName;
            public string roundChipName;
            public float chipValue;
        }

        private List<ChipHistory> chipHistorys = new List <ChipHistory>();

        private float rectWidth;// chip postion button rect width
        private float rectHeight;// chip postion button rect height
        private bool isRoundPanel = false;
        private string[] arrayRoundGroup_1 = { "RoundGroup_1", "05-3", "10-2", "13-3", "23-2", "27-3", "33-3" };
        private string[] arrayRoundGroup_2 = { "RoundGroup_2", "01-0", "06-3", "14-3", "17-3", "31-3" };
        private string[] arrayRoundGroup_3 = { "RoundGroup_3", "02-5", "02-5", "04-3", "12-3", "18-3", "19-3", "25-6", "25-6", "32-3" };
        private string[] arrayRoundGroup_4 = { "RoundGroup_4", "03-1", "12-3", "26-0", "32-3" };

        private List<int> lstLatestNum = new List<int>();
        private List<int> lstHotNum = new List<int>();
        private int[] arrayRedBlack = new int[3];
        private int[] arrayOddEven = new int[3];
        private int[] arrayDozen = new int[3];
        private int[] arrayColumn = new int[3];
        private bool isSquarButtonHover = false;
        private GameObject hoverSquarButtonObj;
        private byte prevChipPos = 0;

        public Sprite[] sprites;
        //public Texture2D spriteSheet;
        public GameObject zoomGoal;
        [HideInInspector]
        public int serverWinPoint = -1;
        private bool chipAniState = false;

        //private int[] arrayColdNum = new int[] { 12, 13, 26, 6, 14 };
        [HideInInspector]
        public string tokenValue = "";

        // Screen resize
        [SerializeField]
        private CanvasScaler canvasScaler;
        private int resolutionX;
        private int resolutionY;
        [SerializeField]
        private GameObject Img_WorldBG;
        [SerializeField]
        private GameObject img_ScreenBG;
        [SerializeField]
        private GameObject pan_Static;
        [SerializeField]
        private GameObject pan_Static_2;

        public GameObject pan_Refund;
        [SerializeField]
        private GameObject pan_Info;
        public GameObject txt_Refund;


        float expandRatioX = 1f, expandRatioY = 1f;

        private bool isMobile = false;

        public static OthoMenu instance;



        private void Awake()
        {
            if (!instance)
                instance = this;

            if (UseJSLib.IsAndroid() || UseJSLib.IsIos())
            {
                isMobile = true;
                //Debug.Log("this platform is Mobile!");
            }
            else
            {
                isMobile = false;
                //Debug.Log("this platform isnot Mobile!");
            }

            resolutionX = ConstVars.designeWidth;
            resolutionY = ConstVars.designeHeight;

        }
        // Start is called before the first frame update
        void Start()
        {
            txt_neighbourCount.GetComponent<TMP_Text>().text = neighbourCount.ToString();
            txt_Value.GetComponent<TMP_Text>().text = betValues[curBetValuesIdx].ToString();
            pan_SquareControlGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(-25f, 2.15f);
            pan_SquareControlGroup.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);

            arrayRedBlack = new int[] { 0, 0, 0 };
            arrayOddEven = new int[] { 0, 0, 0 };
            arrayDozen = new int[] { 0, 0, 0 };
            arrayColumn = new int[] { 0, 0, 0 };

            UpdateInfos();

            if (isMobile)
            {
                MoblieUI();
                //MobileModeUI(UseJSLib.GetWindowWidth());
            }

            string url = UseJSLib.GetSearchParams();
            if (url != "")
                tokenValue = url.Substring(url.IndexOf("=") + 1); //HttpUtility.ParseQueryString(new Uri(url).Query).Get("cert");
                                                                  //Debug.Log("_URL = " + url + ", tokenValue = " + tokenValue);
        }

        private void MoblieUI()
        {
            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
        }

        public void ExitGame()
        {
            Application.ExternalCall("quitted");
        }

        public void Refund()
        {
            //if (!canBet) return;

            //PlayerIno myInfo = new PlayerIno();
            //myInfo.userId = GameManager.instance.playerID;
            //SocketIOManager.instance.sioCom.Instance.Emit("refund", JsonUtility.ToJson(myInfo), false);
        }

        public void OpenUrl(GameObject thisObj)
        {
            //thisObj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "https://induswin.com/#/pages/recharge/recharge";
            Debug.Log("OpenURL =" + thisObj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text);            
            {
                //StartCoroutine(OpenInSelfTabCoroutine(thisObj));                
                Application.OpenURL("https://induswin.com/#/pages/recharge/recharge");
            }
        }

        IEnumerator OpenInSelfTabCoroutine(GameObject obj)
        {
            yield return new WaitForSeconds(0.5f); // wait for the browser to open the new tab
            string script = "window.location.href =" + obj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text; // JavaScript to reload the current page
            Application.ExternalEval(script); // 
        }

        private void DeskTopModeUI(int screenWidth)
        {
            //Debug.Log("BrowserScreenWidth = " + screenWidth + ", GameScreenWidth = " + Screen.width + ", GameScreenHeight = " + Screen.height);

            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);

            expandRatioX = (float)Screen.width / (float)ConstVars.designeWidth;
            expandRatioY = (float)Screen.height / (float)ConstVars.designeHeight;

            //GameObject panel_View = pan_WholeBoard;
            img_ScreenBG.GetComponent<RectTransform>().localScale = new Vector3(expandRatioX, expandRatioY, 1f);
            Img_WorldBG.GetComponent<RectTransform>().localScale = new Vector3(expandRatioX, expandRatioY, 1f);
            pan_Static.GetComponent<RectTransform>().localScale = new Vector3(expandRatioX, expandRatioY, 1f);
            pan_WholeBoard.GetComponent<RectTransform>().localScale = new Vector3(expandRatioX, expandRatioY, 1f);
            pan_Static_2.GetComponent<RectTransform>().localScale = new Vector3(expandRatioX, expandRatioY, 1f);

            resolutionX = Screen.width;
            resolutionY = Screen.height;
        }

        private void LateUpdate()
        {
            if (resolutionX == Screen.width && resolutionY == Screen.height) return;
            DeskTopModeUI(Screen.width);
        }
        public void OnBetRoundInside(GameObject selObj)
        {
            if (!BetButtonState()) return;

            if (isScreenSpaceMenu)
                btn_Rollet.SetActive(true);

            string buttonIdx = selObj.name.Substring(selObj.name.Length - 2);
            int curIdx = Array.IndexOf(snakeOfIdx, buttonIdx);
            if (curIdx == -1)
            {
                //Debug.Log(buttonIdx + " found at index " + curIdx);
                return;
            }

            int startIdx = curIdx - neighbourCount; // snake start idx
            int endIdx = curIdx + neighbourCount; // snake end idx
            int j = 0;
            for (int i = startIdx; i <= endIdx; i++)
            {
                // calculate snake idex for loop array.
                if (i < 0)
                    j = snakeOfIdx.Length - (-i);
                else if (i > snakeOfIdx.Length - 1)
                    j = i - snakeOfIdx.Length;
                else
                    j = i;

                string curSnakeObjIdx = snakeOfIdx[j];
                string strObjectNameInRound = "Round_" + curSnakeObjIdx;
                //Debug.Log("curSnakeObjName = " + curSnakeObjName);

                GameObject chipInRoundObj = GameObject.Find(strObjectNameInRound);
                OnBetAction(chipInRoundObj);
                historyIndex--;
                string strObjectNameInSquare = "Squar_" + curSnakeObjIdx;
                GameObject ChipInSquareObj = GameObject.Find(strObjectNameInSquare);
                OnBetAction(ChipInSquareObj);
                historyIndex--;
            }
            historyIndex++;
        }

        public void OnBetOutSide(GameObject selObj)
        {
            if (!BetButtonState()) return;

            if (isScreenSpaceMenu)
                btn_Rollet.SetActive(true);

            float curBetValue = betValues[curBetValuesIdx];

            if (!CheckTotalValue(curBetValue))
                return;

            GameObject selChipObj = GameObject.Find("Chip_" + selObj.name.Substring(6));

            if (selChipObj)
            {
                float chipValue = float.Parse(selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text) + curBetValue;
                ChangeChipSprite(selChipObj, chipValue);

                selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();

                dicChipObjects[selChipObj.name].chipValue = chipValue;

                AddHistory(selChipObj.name, curBetValue);
                historyIndex++;
            }
            else
            {
                Vector3 vChipPos = new Vector3(-0f, 0f, 0f);
                GameObject chipInstant = GameObject.Instantiate(prefabChip, vChipPos, Quaternion.identity);
                chipInstant.name = "Chip_" + selObj.name.Substring(6);
                //Debug.Log("NewChipObject Name = " + chipInstant.name);        

                chipInstant.transform.SetParent(selObj.transform, false);
                chipInstant.GetComponent<RectTransform>().anchoredPosition = vChipPos;
                chipInstant.transform.GetChild(0).GetComponent<TMP_Text>().text = curBetValue.ToString();
                                
                Chip chipObj = new Chip();
                chipObj.name = chipInstant.name;
                chipObj.buttonName = selObj.name.Substring(6);
                chipObj.positionInButton = 0;
                chipObj.chipValue = curBetValue;
                //chipObj.historyIndex = historyIndex;
                dicChipObjects.Add(chipInstant.name, chipObj);

                AddHistory(chipInstant.name, curBetValue);
                historyIndex++;
                ChangeChipSprite(chipInstant, curBetValue);
            }

            TotalValue();
        }

        public void OnBetRoundOutSide(GameObject selObj)
        {
            if (!BetButtonState()) return;

            String[] objNames = null;
            if (selObj.name == "Round_RoundGroup_1")
            {
                objNames = arrayRoundGroup_1;
            }
            else if (selObj.name == "Round_RoundGroup_2")
            {
                objNames = arrayRoundGroup_2;
            }
            else if (selObj.name == "Round_RoundGroup_3")
            {
                objNames = arrayRoundGroup_3;
            }
            else if (selObj.name == "Round_RoundGroup_4")
            {
                objNames = arrayRoundGroup_4;
            }

            float curBetValue = betValues[curBetValuesIdx];

            float allBetVaule = curBetValue * (objNames.Length -1 );
            
            if (!CheckTotalValue(allBetVaule))
                return;

            foreach (string strObjName in objNames)
            {
                GameObject selChipObj = null;
                if (strObjName.Substring (0, 2) == "Ro")
                {
                    selChipObj = GameObject.Find("RoundChip_" + strObjName);

                    AddHistory("RoundChip_" + strObjName, curBetValue, true);
                }                   
                else
                {
                    selChipObj = GameObject.Find("Chip_" + strObjName);

                    AddHistory("Chip_" + strObjName, curBetValue);
                }                    

                if (selChipObj)
                {
                    float chipValue = float.Parse(selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text) + curBetValue;
                    ChangeChipSprite(selChipObj, chipValue);

                    selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();

                    dicChipObjects[selChipObj.name].chipValue = chipValue;
                    //Debug.Log("dicChipObjects name = " + dicChipObjects[selChipObj.name].name);
                }
                else
                {
                    Vector3 vChipPos = new Vector3(0f, 0f, 0f);
                    GameObject chipInstant = GameObject.Instantiate(prefabChip, vChipPos, Quaternion.identity);
                                        
                    string strParentName = "";
                    GameObject parentObj = null;
                    Chip chipObj = new Chip();

                    if (strObjName.Substring(0, 2) == "Ro")
                    {
                        strParentName = "Round_" + strObjName;
                        parentObj = GameObject.Find(strParentName);

                        chipInstant.name = "RoundChip_" + strObjName;
                        chipObj.buttonName = strObjName;
                        chipObj.positionInButton = 0;
                    }
                    else
                    {
                        strParentName = "Squar_" + strObjName.Substring(0, 2);
                        parentObj = GameObject.Find(strParentName);
                        vChipPos = GetRecPos(parentObj, int.Parse(strObjName.Substring(3, 1)));

                        chipInstant.name = "Chip_" + strObjName;
                        chipObj.buttonName = strObjName.Substring(0, 2);
                        chipObj.positionInButton = byte.Parse(strObjName.Substring(3, 1));
                    }

                    chipInstant.transform.SetParent(parentObj.transform, false);
                    chipInstant.GetComponent<RectTransform>().anchoredPosition = vChipPos;
                    chipInstant.transform.GetChild(0).GetComponent<TMP_Text>().text = curBetValue.ToString();

                    chipObj.name = chipInstant.name;

                    chipObj.chipValue = curBetValue;
                    //chipObj.historyIndex = historyIndex;
                    dicChipObjects.Add(chipInstant.name, chipObj);

                    //Debug.Log("current history value: " + historyIndex);
                    
                    ChangeChipSprite(chipInstant, curBetValue);
                }
            }
            historyIndex++;
            TotalValue();
        }
        private bool BetButtonState()
        {
            if (isSpinBet) return false;
            if (btn_Rebet.activeSelf)
            {
                foreach (Chip chip in dicChipObjects.Values)
                {
                    GameObject chipObj = GameObject.Find(chip.name);
                    GameObject.Destroy(chipObj);
                }
                dicChipObjects.Clear();
                btn_Rebet.SetActive(false);
                btn_RebetAndSpin.SetActive(false);
            }
            beginBet = true;
            return true;
        }

        public void OnBetAction(GameObject selObj) // for number buttons
        {
            if (!BetButtonState()) return;

            ViewRightDownButton(true);
            if (isScreenSpaceMenu)
                btn_Rollet.SetActive(true);

            //Debug.Log("width = " + rectWidth + ", heigth = " + rectHeight);
            //Debug.Log("thisUI Pos = " + selObj.transform.position + ", canvasEventData = " + canvasEventData.position  );
            if (!isRoundPanel)
            {
                // Get Rect Part of clicked point
                Vector2 canvasPos;
                if (isScreenSpaceMenu)
                {
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(selObj.GetComponent<RectTransform>(),
                        canvasEventData.position, null, out canvasPos))
                    {
                        //Debug.Log("Mouse Position in Canvas Space: " + canvasPos);
                    }
                }
                else
                {
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(selObj.GetComponent<RectTransform>(),
                        canvasEventData.position, canvasCam, out canvasPos))
                    {
                        //Debug.Log("Mouse Position in Canvas Space: " + canvasPos);
                    }
                }

                rectWidth = selObj.GetComponent<RectTransform>().rect.width;
                rectHeight = selObj.GetComponent<RectTransform>().rect.height;
                GetChipPos(canvasPos);

                // Top line of square number group is blocked.
                int selIdx = int.Parse(selObj.name.Substring(selObj.name.Length - 2));
                if (selIdx % 3 == 0)
                {
                    if (chipPos == 5 || chipPos == 2 || chipPos == 6)
                    {
                        return;
                    }
                }
                // clear all square button hover
                int min = selIdx - 3;
                if (min < 0) min = 0;
                int max = selIdx + 3;
                if (max > 36) max = 36;
                for (int i = min; i <= max; i++)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    GameObject.Find("Squar_" + i.ToString("D2")).GetComponent<Button>().OnPointerExit(canvasEventData);
                }
            }
            else
            {
                chipPos = 0;
            }

            CreatChipOrChangeText(selObj);
            TotalValue();
        }

        public void CreatChipOrChangeText(GameObject selObj)
        {
            int selObjIdx = int.Parse(selObj.name.Substring(selObj.name.Length - 2));
            string strSelObjIdx = selObjIdx.ToString("D2");

            int sideObjIdx;
            string strSideObjIdx = "";

            int sideObjIdx_1, sideObjIdx_2, sideObjIdx_3;
            string strSideObjIdx_1 = "", strSideObjIdx_2 = "", strSideObjIdx_3 = "";

            GameObject sideChipObj = null, selChipObj = null;
            GameObject sideChipObj_1 = null, sideChipObj_2 = null, sideChipObj_3 = null;

            float curBetValue = betValues[curBetValuesIdx];
            if (!CheckTotalValue(curBetValue))
                return;

            switch (chipPos)
            {
                case 1:
                    {
                        sideObjIdx = selObjIdx - 3;
                        if (sideObjIdx > 0)
                        {
                            strSideObjIdx = sideObjIdx.ToString("D2");
                        }
                        sideChipObj = GameObject.Find("Chip_" + strSideObjIdx + "-3"); // side chipPos of chipPos 1 is 3
                        selChipObj = GameObject.Find("Chip_" + strSelObjIdx + "-" + chipPos.ToString());

                        break;
                    }
                case 3:
                    {
                        sideObjIdx = selObjIdx + 3;
                        if (sideObjIdx < 37)
                        {
                            strSideObjIdx = sideObjIdx.ToString("D2");
                        }
                        sideChipObj = GameObject.Find("Chip_" + strSideObjIdx + "-1"); // side chipPos of chipPos 1 is 3
                        selChipObj = GameObject.Find("Chip_" + strSelObjIdx + "-" + chipPos.ToString());

                        break;
                    }
                case 2:
                    {
                        sideObjIdx = selObjIdx + 1; // 2's side is 4 and objIdx is add 1
                        if (sideObjIdx < 37)
                        {
                            strSideObjIdx = sideObjIdx.ToString("D2");
                        }
                        sideChipObj = GameObject.Find("Chip_" + strSideObjIdx + "-4"); // side chipPos of chipPos 1 is 3
                        selChipObj = GameObject.Find("Chip_" + strSelObjIdx + "-" + chipPos.ToString());

                        break;
                    }
                case 4:
                    {
                        sideObjIdx = selObjIdx - 1;
                        if (sideObjIdx > 0)
                        {
                            strSideObjIdx = sideObjIdx.ToString("D2");
                        }
                        sideChipObj = GameObject.Find("Chip_" + strSideObjIdx + "-2"); // side chipPos of chipPos 1 is 3
                        selChipObj = GameObject.Find("Chip_" + strSelObjIdx + "-" + chipPos.ToString());

                        break;
                    }
                case 5:
                    {
                        sideObjIdx_1 = selObjIdx - 3;
                        sideObjIdx_2 = selObjIdx - 2;
                        sideObjIdx_3 = selObjIdx + 1;
                        if (sideObjIdx_1 > 0)
                        {
                            strSideObjIdx_1 = sideObjIdx_1.ToString("D2");
                        }
                        if (sideObjIdx_2 > 0)
                        {
                            strSideObjIdx_2 = sideObjIdx_2.ToString("D2");
                        }
                        if (sideObjIdx_3 < 37)
                        {
                            strSideObjIdx_3 = sideObjIdx_3.ToString("D2");
                        }

                        sideChipObj_1 = GameObject.Find("Chip_" + strSideObjIdx_1 + "-6");
                        sideChipObj_2 = GameObject.Find("Chip_" + strSideObjIdx_2 + "-7");
                        sideChipObj_3 = GameObject.Find("Chip_" + strSideObjIdx_3 + "-8");
                        selChipObj = GameObject.Find("Chip_" + strSelObjIdx + "-" + chipPos.ToString());

                        break;
                    }
                case 6:
                    {
                        sideObjIdx_1 = selObjIdx + 1;
                        sideObjIdx_2 = selObjIdx + 4;
                        sideObjIdx_3 = selObjIdx + 3;
                        if (sideObjIdx_1 < 37)
                        {
                            strSideObjIdx_1 = sideObjIdx_1.ToString("D2");
                        }
                        if (sideObjIdx_2 < 37)
                        {
                            strSideObjIdx_2 = sideObjIdx_2.ToString("D2");
                        }
                        if (sideObjIdx_3 < 37)
                        {
                            strSideObjIdx_3 = sideObjIdx_3.ToString("D2");
                        }

                        sideChipObj_1 = GameObject.Find("Chip_" + strSideObjIdx_1 + "-7");
                        sideChipObj_2 = GameObject.Find("Chip_" + strSideObjIdx_2 + "-8");
                        sideChipObj_3 = GameObject.Find("Chip_" + strSideObjIdx_3 + "-5");
                        selChipObj = GameObject.Find("Chip_" + strSelObjIdx + "-" + chipPos.ToString());

                        break;
                    }
                case 7:
                    {
                        sideObjIdx_1 = selObjIdx + 3;
                        sideObjIdx_2 = selObjIdx + 2;
                        sideObjIdx_3 = selObjIdx - 1;
                        if (sideObjIdx_1 < 37)
                        {
                            strSideObjIdx_1 = sideObjIdx_1.ToString("D2");
                        }
                        if (sideObjIdx_2 < 37)
                        {
                            strSideObjIdx_2 = sideObjIdx_2.ToString("D2");
                        }
                        if (sideObjIdx_3 > 0)
                        {
                            strSideObjIdx_3 = sideObjIdx_3.ToString("D2");
                        }

                        sideChipObj_1 = GameObject.Find("Chip_" + strSideObjIdx_1 + "-8");
                        sideChipObj_2 = GameObject.Find("Chip_" + strSideObjIdx_2 + "-5");
                        sideChipObj_3 = GameObject.Find("Chip_" + strSideObjIdx_3 + "-6");
                        selChipObj = GameObject.Find("Chip_" + strSelObjIdx + "-" + chipPos.ToString());

                        break;
                    }
                case 8:
                    {
                        sideObjIdx_1 = selObjIdx - 1;
                        sideObjIdx_2 = selObjIdx - 4;
                        sideObjIdx_3 = selObjIdx - 3;
                        if (sideObjIdx_1 > 0)
                        {
                            strSideObjIdx_1 = sideObjIdx_1.ToString("D2");
                        }
                        if (sideObjIdx_2 > 0)
                        {
                            strSideObjIdx_2 = sideObjIdx_2.ToString("D2");
                        }
                        if (sideObjIdx_3 > 0)
                        {
                            strSideObjIdx_3 = sideObjIdx_3.ToString("D2");
                        }

                        sideChipObj_1 = GameObject.Find("Chip_" + strSideObjIdx_1 + "-5");
                        sideChipObj_2 = GameObject.Find("Chip_" + strSideObjIdx_2 + "-6");
                        sideChipObj_3 = GameObject.Find("Chip_" + strSideObjIdx_3 + "-7");
                        selChipObj = GameObject.Find("Chip_" + strSelObjIdx + "-" + chipPos.ToString());

                        break;
                    }
                case 0: // Round Panel's  pos is only 0!
                    {
                        selChipObj = null;
                        // although there is in round panel, it make to call two type of Round and Num button click function.
                        if (selObj.name.Substring(0, 5) == "Squar")
                            selChipObj = GameObject.Find("Chip_" + strSelObjIdx + "-" + chipPos.ToString());
                        else
                            selChipObj = GameObject.Find("RoundChip_" + strSelObjIdx + "-0");

                        if (selChipObj)
                        {
                            float chipValue = float.Parse(selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text) + curBetValue;
                            ChangeChipSprite(selChipObj, chipValue);
                            selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();
                            dicChipObjects[selChipObj.name].chipValue = chipValue;

                            AddHistory(selChipObj.name, curBetValue);
                            historyIndex++;
                        }
                        else
                        {
                            CreatChip(selObj);
                        }
                        break;
                    }
            }

            if (1 <= chipPos && chipPos <= 4)
            {
                if (sideChipObj || selChipObj) //There is no this (sideChipObj && selChipObj) 
                {
                    Change2ChipText(sideChipObj, selChipObj, curBetValue);
                }
                else
                {
                    CreatChip(selObj);
                }
            }
            else if (5 <= chipPos && chipPos <= 8)
            {
                if (sideChipObj_1 || sideChipObj_2 || sideChipObj_3 || selChipObj) //There is no this (sideChipObj && selChipObj) 
                {
                    Change4ChipText(sideChipObj_1, sideChipObj_2, sideChipObj_3, selChipObj, curBetValue);
                }
                else
                {
                    CreatChip(selObj);
                }

            }
        }

        private void Change2ChipText(GameObject sideChipObj, GameObject selChipObj, float curBetValue)
        {
            if (sideChipObj)
            {
                float chipValue = float.Parse(sideChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text) + curBetValue;
                ChangeChipSprite(sideChipObj, chipValue);
                sideChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();
                dicChipObjects[sideChipObj.name].chipValue = chipValue;

                AddHistory(sideChipObj.name, curBetValue);
                historyIndex++;
            }
            else if (selChipObj)
            {
                float chipValue = float.Parse(selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text) + curBetValue;
                ChangeChipSprite(selChipObj, chipValue);
                selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();
                dicChipObjects[selChipObj.name].chipValue = chipValue;

                AddHistory(selChipObj.name, curBetValue);
                historyIndex++;
            }
        }

        private void Change4ChipText(GameObject sideChipObj_1, GameObject sideChipObj_2, GameObject sideChipObj_3, GameObject selChipObj, float curBetValue)
        {
            if (sideChipObj_1)
            {
                float chipValue = float.Parse(sideChipObj_1.transform.GetChild(0).GetComponent<TMP_Text>().text) + curBetValue;
                ChangeChipSprite(sideChipObj_1, chipValue);
                sideChipObj_1.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();
                dicChipObjects[sideChipObj_1.name].chipValue = chipValue;

                AddHistory(sideChipObj_1.name, curBetValue);
                historyIndex++;
            }
            else if (sideChipObj_2)
            {
                float chipValue = float.Parse(sideChipObj_2.transform.GetChild(0).GetComponent<TMP_Text>().text) + curBetValue;
                ChangeChipSprite(sideChipObj_2, chipValue);
                sideChipObj_2.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();
                dicChipObjects[sideChipObj_2.name].chipValue = chipValue;

                AddHistory(sideChipObj_2.name, curBetValue);
                historyIndex++;
            }
            else if (sideChipObj_3)
            {
                float chipValue = float.Parse(sideChipObj_3.transform.GetChild(0).GetComponent<TMP_Text>().text) + curBetValue;
                ChangeChipSprite(sideChipObj_3, chipValue);
                sideChipObj_3.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();
                dicChipObjects[sideChipObj_3.name].chipValue = chipValue;

                AddHistory(sideChipObj_3.name, curBetValue);
                historyIndex++;
            }
            else if (selChipObj)
            {
                float chipValue = float.Parse(selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text) + curBetValue;
                ChangeChipSprite(selChipObj, chipValue);
                selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();
                dicChipObjects[selChipObj.name].chipValue = chipValue;

                AddHistory(selChipObj.name, curBetValue);
                historyIndex++;
            }
        }

        private void AddHistory (string chipName, float curBetValue, bool isRoundChip = false)
        {
            //Debug.Log("historyIndex = " + historyIndex + ", chipName = " + chipName);
            ChipHistory chipHistoryObjec = new ChipHistory();

            chipHistoryObjec.historyIdx = historyIndex;

            if (!isRoundChip)
            {
                chipHistoryObjec.chipName = chipName;
                chipHistoryObjec.roundChipName = "";
            }                           
            else
            {
                chipHistoryObjec.chipName = "";
                chipHistoryObjec.roundChipName = chipName;
            }
                

            chipHistoryObjec.chipValue = curBetValue;
            chipHistorys.Add(chipHistoryObjec);
        }

        private void CreatChip(GameObject selObj)
        {
            string strSelObjIdx = selObj.name.Substring(selObj.name.Length - 2);

            Vector3 vChipPos = new Vector3(0f, 0f, 0f);

            GameObject chipInstant = GameObject.Instantiate(prefabChip, vChipPos, Quaternion.identity);
            if (selObj.name.Substring(0, 5) == "Squar") // although there is in round panel, it make to call two type of Round and Num button click function. 
                chipInstant.name = "Chip_" + strSelObjIdx + "-" + chipPos.ToString();
            else
                chipInstant.name = "RoundChip_" + strSelObjIdx + "-0";
            //Debug.Log("NewChipObject Name = " + chipInstant.name);        

            chipInstant.transform.SetParent(selObj.transform, false);

            switch (chipPos)
            {
                case 0:
                    {
                        vChipPos = new Vector3(0f, 0f, 0f);
                        break;
                    }
                case 1:
                    {
                        vChipPos = new Vector3(-rectWidth / 2f, 0f, 0f);
                        break;
                    }
                case 2:
                    {
                        vChipPos = new Vector3(0f, rectHeight / 2f, 0f);
                        break;
                    }
                case 3:
                    {
                        vChipPos = new Vector3(rectWidth / 2f, 0f, 0f);
                        break;
                    }
                case 4:
                    {
                        vChipPos = new Vector3(0f, -rectHeight / 2f, 0f);
                        break;
                    }
                case 5:
                    {
                        vChipPos = new Vector3(-rectWidth / 2f, rectHeight / 2f, 0f);
                        break;
                    }
                case 6:
                    {
                        vChipPos = new Vector3(rectWidth / 2f, rectHeight / 2f, 0f);
                        break;
                    }
                case 7:
                    {
                        vChipPos = new Vector3(rectWidth / 2f, -rectHeight / 2f, 0f);
                        break;
                    }
                case 8:
                    {
                        vChipPos = new Vector3(-rectWidth / 2f, -rectHeight / 2f, 0f);
                        break;
                    }
            }
            chipInstant.GetComponent<RectTransform>().anchoredPosition = vChipPos;
            float chipValue = betValues[curBetValuesIdx];
            ChangeChipSprite(chipInstant, chipValue);
            chipInstant.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();

            Chip chipObj = new Chip();
            chipObj.name = chipInstant.name;
            chipObj.buttonName = strSelObjIdx;// "RoundChip"        
            chipObj.positionInButton = chipPos;
            chipObj.chipValue = chipValue;
            //chipObj.historyIndex = historyIndex;
            dicChipObjects.Add(chipInstant.name, chipObj);

            AddHistory(chipInstant.name, chipValue);
            historyIndex++;
        }

        public void Undo()
        {
            //int maxValue = dicChipObjects.Values.Max(chip => chip.historyIndex );
            /*
            if (dicChipObjects.Values.Count > 0)
            {
                Chip recentChip = dicChipObjects.Values.OrderByDescending(chip => chip.historyIndex).First();
                dicChipObjects.Remove(dicChipObjects.First(chipObj => chipObj.Value == recentChip).Key);
                GameObject recentChipObj = GameObject.Find(recentChip.name);
                float newTotal = float.Parse(txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text) - recentChip.chipValue;
                txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text = newTotal.ToString();
                GameObject.Destroy(recentChipObj);
            }           
            */
            //Debug.Log("historyIndex =" + historyIndex + "histoyCount = " + chipHistorys.Count);
            if (chipHistorys.Count == 0) return;

            List<ChipHistory> lastHistoryList = chipHistorys.Where(chip => chip.historyIdx == historyIndex -1).ToList();
            foreach (ChipHistory item in lastHistoryList)
            {
                GameObject selChipObj = null;
                if ( item.chipName != "")
                {
                    selChipObj = GameObject.Find(item.chipName);                    
                }
                else
                {
                    selChipObj = GameObject.Find(item.roundChipName);
                }

                float chipValue = float.Parse(selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text) - item.chipValue;
                if (chipValue > 0)
                {
                    ChangeChipSprite(selChipObj, chipValue);
                    selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();
                    dicChipObjects[selChipObj.name].chipValue = chipValue;
                }
                else
                {
                    /*
                    foreach (var item in dicChipObjects.Where(kvp => kvp.Value.name == "A").ToList())
                    {
                        dicChipObjects.Remove(item.Key);
                    }*/
                    foreach (KeyValuePair<string, Chip> kvp in dicChipObjects)
                    {
                        if (kvp.Value.name == item.chipName || kvp.Value.name == item.roundChipName )
                        {
                            dicChipObjects.Remove(kvp.Key);
                            break;
                        }
                    }
                    
                    GameObject.Destroy(selChipObj);                    
                }
                chipHistorys.Remove(item);
            }

            historyIndex -= 1;
            if (historyIndex < 0)
                historyIndex = 0;

            TotalValue();
        }

        public void ClearAll()
        {
            foreach (Chip chip in dicChipObjects.Values)
            {
                GameObject chipObj = GameObject.Find(chip.name);
                GameObject.Destroy(chipObj);
            }
            dicChipObjects.Clear();
            txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text = string.Empty;

            historyIndex = 0;
            chipHistorys.Clear();

            btn_Rollet.SetActive(false);
            beginBet = false;
        }

        public void Double()
        {
            float newTotal = float.Parse(txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text) * 2f;
            if (newTotal > 1000 || newTotal > float.Parse(txt_Cach.GetComponent<TMP_Text>().text))
                return;
            txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text = newTotal.ToString();

            foreach (Chip chip in dicChipObjects.Values)
            {
                chip.chipValue *= 2f;
                GameObject chipObj = GameObject.Find(chip.name);
                chipObj.transform.GetChild(0).GetComponent<TMP_Text>().text = chip.chipValue.ToString();
            }           
            
        }

        private bool CheckTotalValue(float curBetValue)
        {
            float curTotalValue = 0f;
            if (txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text != "")
                curTotalValue = float.Parse ( txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text);
            float curFUN = 0f;
            if (txt_Cach.GetComponent<TMP_Text>().text != "")
                curFUN = float.Parse(txt_Cach.GetComponent<TMP_Text>().text);

            if (curTotalValue + curBetValue <= 1000f && curTotalValue + curBetValue <= curFUN)
            {
                //txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text = (curTotalValue + curBetValue ).ToString();
                return true;
            }
            else
            {
                return false;
            }
            
        }
        private void TotalValue()
        {
            float totalValue = 0f;
            Dictionary<string, Chip>.ValueCollection values = dicChipObjects.Values;
            foreach (Chip val in values)
            {
                //Debug.Log ("ChipObj: chipName = " + val.name + ", chipButtonName = " + val.buttonName + ", chipPos" + val.positionInButton );
                if (val.name.Substring (0, 4) == "Chip")
                    totalValue += val.chipValue;
            }

            txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text = totalValue.ToString();
        }

        private void GetChipPos(Vector2 canvasPos)
        {
            if (canvasPos.x >= -rectWidth / 2f && canvasPos.x <= -rectWidth * 2f / 6f)
            {
                if (canvasPos.y >= -rectHeight * 2f / 6f && canvasPos.y <= rectHeight * 2f / 6f)
                {
                    chipPos = 1;
                    //Debug.Log("This is 1 part!");
                }
                else if (canvasPos.y < -rectHeight * 2f / 6f && canvasPos.y >= -rectHeight / 2f)
                {
                    chipPos = 8;
                    //Debug.Log("This is 8 part!");
                }
                else if (canvasPos.y > rectHeight * 2f / 6f && canvasPos.y <= rectHeight / 2f)
                {
                    chipPos = 5;
                    //Debug.Log("This is 5 part!");
                }
            }
            else if (canvasPos.x > -rectWidth * 2f / 6f && canvasPos.x <= rectWidth * 2f / 6f)
            {
                if (canvasPos.y >= -rectHeight * 2f / 6f && canvasPos.y <= rectHeight * 2f / 6f)
                {
                    chipPos = 0;
                    //Debug.Log("This is 0 part!");
                }
                else if (canvasPos.y < -rectHeight * 2f / 6f && canvasPos.y >= -rectHeight / 2f)
                {
                    chipPos = 4;
                    //Debug.Log("This is 4 part!");
                }
                else if (canvasPos.y > rectHeight * 2f / 6f && canvasPos.y <= rectHeight / 2f)
                {
                    chipPos = 2;
                    //Debug.Log("This is 2 part!");
                }
            }
            else if (canvasPos.x > rectWidth * 2f / 6f && canvasPos.x <= rectWidth / 2f)
            {
                if (canvasPos.y >= -rectHeight * 2f / 6f && canvasPos.y <= rectHeight * 2f / 6f)
                {
                    chipPos = 3;
                    //Debug.Log("This is 3 part!");
                }
                else if (canvasPos.y < -rectHeight * 2f / 6f && canvasPos.y >= -rectHeight / 2f)
                {
                    chipPos = 7;
                    //Debug.Log("This is 7 part!");
                }
                else if (canvasPos.y > rectHeight * 2f / 6f && canvasPos.y <= rectHeight / 2f)
                {
                    chipPos = 6;
                    //Debug.Log("This is 6 part!");
                }
            }
        }

        public void OnObjectHover(GameObject hoverObj)
        {
            //Debug.Log("OnObjectHover = " + hoverObj.name + ", mousePos = " + canvasEventData.position + ", Input.mouse = " + Input.mousePosition );
            //if (eventData.pointerCurrentRaycast.gameObject != null)

            for (int i = 0; i <= 36; i++)
            {
                EventSystem.current.SetSelectedGameObject(null);
                GameObject.Find("Squar_" + i.ToString("D2")).GetComponent<Button>().OnPointerExit(canvasEventData);
            }

            if (hoverObj.name == "Squar_p12")
            {
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    //Debug.Log("Mouse Hovered GameObject = " + itemTransform.gameObject.name);
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                }
            }
            else if (hoverObj.name == "Squar_m12")
            {
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                }
            }
            else if (hoverObj.name == "Squar_d12")
            {
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                }
            }
            else if (hoverObj.name == "Squar_manque")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                }
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i < 6)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_passe")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                }
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i > 5)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_impair")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    if (i % 2 == 0)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i % 2 == 0)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    if (i % 2 == 0)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_pair")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    if (i % 2 == 1)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i % 2 == 1)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    if (i % 2 == 1)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_row1")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 2)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 2)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 2)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_row2")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 1)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 1)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 1)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_row3")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 0)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 0)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 0)
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_black")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    if (i == 1 || i == 3 || i == 5 || i == 7 || i == 9 || i == 10)
                    {
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i == 0 || i == 2 || i == 4 || i == 7 || i == 9 || i == 11)
                    {
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    if (i == 1 || i == 3 || i == 4 || i == 6 || i == 8 || i == 10)
                    {
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    }
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_red")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    if (i == 0 || i == 2 || i == 4 || i == 6 || i == 8 || i == 11)
                    {
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i == 1 || i == 3 || i == 5 || i == 6 || i == 8 || i == 10)
                    {
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    if (i == 0 || i == 2 || i == 5 || i == 7 || i == 9 || i == 11)
                    {
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    }
                    i++;
                }
            }
            else if (hoverObj.name == "Round_RoundGroup_1")
            {
                foreach (Transform itemTransform in src_InsideNumG2_0.transform)
                {
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    //lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
                }
                foreach (Transform itemTransform in scr_InsideNumG2_1.GetComponent<ScrollRect>().content)
                {
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    //lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
                }
                foreach (Transform itemTransform in src_InsideNumG2_7.GetComponent<ScrollRect>().content)
                {
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    //lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
                }
                isRoundPanel = true;
            }
            else if (hoverObj.name == "Round_RoundGroup_2")
            {
                foreach (Transform itemTransform in scr_InsideNumG2_2.GetComponent<ScrollRect>().content)
                {
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    //lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
                }
                foreach (Transform itemTransform in scr_InsideNumG2_6.GetComponent<ScrollRect>().content)
                {
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    //lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
                }
                isRoundPanel = true;
            }
            else if (hoverObj.name == "Round_RoundGroup_3")
            {
                foreach (Transform itemTransform in scr_InsideNumG2_4.transform)
                {
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    //lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
                }
                foreach (Transform itemTransform in scr_InsideNumG2_3.GetComponent<ScrollRect>().content)
                {
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    //lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
                }
                foreach (Transform itemTransform in scr_InsideNumG2_5.GetComponent<ScrollRect>().content)
                {
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    //lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
                }
                isRoundPanel = true;
            }
            else if (hoverObj.name == "Round_RoundGroup_4")
            {
                foreach (Transform itemTransform in scr_InsideNumG2_4.transform)
                {
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    //lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
                }
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG2_3.GetComponent<ScrollRect>().content)
                {
                    if (i == 5)
                    {
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                        //lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
                    }
                    i++;
                }
                foreach (Transform itemTransform in scr_InsideNumG2_5.GetComponent<ScrollRect>().content)
                {
                    if (i == 5)
                    {
                        itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                        //lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
                    }
                    i++;
                }
                isRoundPanel = true;
            }
            else if (hoverObj.name.Substring(0, 2) == "Ro") //RoundNum
            {
                string buttonIdx = hoverObj.name.Substring(hoverObj.name.Length - 2);
                int curIdx = Array.IndexOf(snakeOfIdx, buttonIdx);
                if (curIdx == -1)
                {
                    //Debug.Log(buttonIdx + " found at index " + curIdx);
                    return;
                }

                int startIdx = curIdx - neighbourCount; // snake start idx
                int endIdx = curIdx + neighbourCount; // snake end idx
                int j = 0;
                for (int i = startIdx; i <= endIdx; i++)
                {
                    // calculate snake idex for loop array.
                    if (i < 0)
                        j = snakeOfIdx.Length - (-i);
                    else if (i > snakeOfIdx.Length - 1)
                        j = i - snakeOfIdx.Length;
                    else
                        j = i;

                    string curSnakeObjName = snakeOfIdx[j];
                    curSnakeObjName = "Round_" + curSnakeObjName;
                    //Debug.Log("curSnakeObjName = " + curSnakeObjName);

                    GameObject curSnakeObj = GameObject.Find(curSnakeObjName);
                    curSnakeObj.GetComponent<Button>().OnPointerEnter(canvasEventData);
                }

                isRoundPanel = true;
            }
        }

        public void HoverSquarButtons()
        {
            //Debug.Log("OnHoverMoving! curMouse pos = " + realtimeMousePos);
            if (hoverSquarButtonObj.name.Substring(0, 5) == "Squar")
            {
                // Get Rect Part of clicked point
                Vector2 canvasPos = new Vector2(0f, 0f);
                Vector2 ScreenPoint = new Vector2(realtimeMousePos.x, realtimeMousePos.y);
                if (isScreenSpaceMenu)
                {
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(hoverSquarButtonObj.GetComponent<RectTransform>(),
                        ScreenPoint, null, out canvasPos))
                    {
                        //Debug.Log("Mouse Position in Canvas Space: " + canvasPos);
                    }
                }
                else
                {
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(hoverSquarButtonObj.GetComponent<RectTransform>(),
                        canvasEventData.position, canvasCam, out canvasPos))
                    {
                        //Debug.Log("Mouse Position in Canvas Space: " + canvasPos + ",  canvaseMousePositon = " + canvasEventData.position);
                    }
                }

                rectWidth = hoverSquarButtonObj.GetComponent<RectTransform>().rect.width;
                rectHeight = hoverSquarButtonObj.GetComponent<RectTransform>().rect.height;
                byte curChipPos = HoverChipPos(canvasPos);
                //Debug.Log("squarbutton: prevChipPos = " + prevChipPos + ", curChipPos = " + curChipPos);
                if (prevChipPos != curChipPos)
                {
                    //Debug.Log("PrevChipPos =" + prevChipPos + ", curChipPos = " + curChipPos);
                    HoverOnSelButton(hoverSquarButtonObj, prevChipPos, false);
                    prevChipPos = curChipPos;
                }
                HoverOnSelButton(hoverSquarButtonObj, curChipPos, true);
                //Debug.Log("curChipPos = " + curChipPos);
            }
        }

        private void HoverOnSelButton(GameObject selObj, byte chipPos, bool isEnter)
        {
            int selObjIdx = int.Parse(selObj.name.Substring(selObj.name.Length - 2));
            string strSelObjIdx = selObjIdx.ToString("D2");

            int sideObjIdx;
            string strSideObjIdx = "";

            int sideObjIdx_1, sideObjIdx_2, sideObjIdx_3;
            string strSideObjIdx_1 = "", strSideObjIdx_2 = "", strSideObjIdx_3 = "";

            //for Up row
            if (selObjIdx % 3 == 0)
            {
                if (chipPos == 5) chipPos = 1;
                else if (chipPos == 6) chipPos = 3;
            }

            switch (chipPos)
            {
                case 1:
                    {
                        sideObjIdx = selObjIdx - 3;
                        if (sideObjIdx > 0)
                        {
                            strSideObjIdx = sideObjIdx.ToString("D2");
                        }
                        break;
                    }
                case 3:
                    {
                        sideObjIdx = selObjIdx + 3;
                        if (sideObjIdx < 37)
                        {
                            strSideObjIdx = sideObjIdx.ToString("D2");
                        }

                        break;
                    }
                case 2:
                    {
                        sideObjIdx = selObjIdx + 1; // 2's side is 4 and objIdx is add 1
                        if (sideObjIdx < 37 && sideObjIdx % 3 != 1)
                        {
                            strSideObjIdx = sideObjIdx.ToString("D2");
                        }

                        break;
                    }
                case 4:
                    {// ex; if chip pos == 04-04, colunm of 04 button is selected
                        sideObjIdx = selObjIdx - 1;
                        if (selObjIdx % 3 == 1)
                        {
                            sideObjIdx_1 = selObjIdx + 1;
                            sideObjIdx_2 = selObjIdx + 2;
                            if (sideObjIdx_1 < 37)
                            {
                                strSideObjIdx_1 = sideObjIdx_1.ToString("D2");
                            }
                            if (sideObjIdx_2 < 37)
                            {
                                strSideObjIdx_2 = sideObjIdx_2.ToString("D2");
                            }
                        }
                        else if (sideObjIdx > 0)
                        {
                            strSideObjIdx = sideObjIdx.ToString("D2");
                        }

                        break;
                    }
                case 5:
                    {
                        sideObjIdx_1 = selObjIdx - 3;
                        sideObjIdx_2 = selObjIdx - 2;
                        sideObjIdx_3 = selObjIdx + 1;
                        if (sideObjIdx_1 > 0)
                        {
                            strSideObjIdx_1 = sideObjIdx_1.ToString("D2");
                        }
                        if (sideObjIdx_2 > 0)
                        {
                            strSideObjIdx_2 = sideObjIdx_2.ToString("D2");
                        }
                        if (sideObjIdx_3 < 37)
                        {
                            strSideObjIdx_3 = sideObjIdx_3.ToString("D2");
                        }

                        break;
                    }
                case 6:
                    {
                        sideObjIdx_1 = selObjIdx + 1;
                        sideObjIdx_2 = selObjIdx + 4;
                        sideObjIdx_3 = selObjIdx + 3;
                        if (sideObjIdx_1 < 37)
                        {
                            strSideObjIdx_1 = sideObjIdx_1.ToString("D2");
                        }
                        if (sideObjIdx_2 < 37)
                        {
                            strSideObjIdx_2 = sideObjIdx_2.ToString("D2");
                        }
                        if (sideObjIdx_3 < 37)
                        {
                            strSideObjIdx_3 = sideObjIdx_3.ToString("D2");
                        }
                        break;
                    }
                case 7:
                    {
                        if (selObjIdx % 3 == 1) break; // for processing two column of bottom row.

                        sideObjIdx_1 = selObjIdx + 3;
                        sideObjIdx_2 = selObjIdx + 2;
                        sideObjIdx_3 = selObjIdx - 1;
                        if (sideObjIdx_1 < 37)
                        {
                            strSideObjIdx_1 = sideObjIdx_1.ToString("D2");
                        }
                        if (sideObjIdx_2 < 37)
                        {
                            strSideObjIdx_2 = sideObjIdx_2.ToString("D2");
                        }
                        if (sideObjIdx_3 > 0)
                        {
                            strSideObjIdx_3 = sideObjIdx_3.ToString("D2");
                        }
                        break;
                    }
                case 8:
                    {
                        if (selObjIdx % 3 == 1) break; // for processing two column of bottom row. 

                        sideObjIdx_1 = selObjIdx - 1;
                        sideObjIdx_2 = selObjIdx - 4;
                        sideObjIdx_3 = selObjIdx - 3;
                        if (sideObjIdx_1 > 0)
                        {
                            strSideObjIdx_1 = sideObjIdx_1.ToString("D2");
                        }
                        if (sideObjIdx_2 > 0)
                        {
                            strSideObjIdx_2 = sideObjIdx_2.ToString("D2");
                        }
                        if (sideObjIdx_3 > 0)
                        {
                            strSideObjIdx_3 = sideObjIdx_3.ToString("D2");
                        }

                        break;
                    }
                case 0:
                    {
                        selObj.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                        break;
                    }
            }

            if (1 <= chipPos && chipPos <= 4) // chipPos 4 have 2 case - having sideObject or side_1, side_2;
            {
                GameObject sideObj = GameObject.Find("Squar_" + strSideObjIdx);
                if (sideObj)
                {
                    if (isEnter)
                        sideObj.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    else
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        sideObj.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                }
            }

            if (4 <= chipPos && chipPos <= 8)
            {
                GameObject sideChipObj_1 = GameObject.Find("Squar_" + strSideObjIdx_1);
                GameObject sideChipObj_2 = GameObject.Find("Squar_" + strSideObjIdx_2);
                GameObject sideChipObj_3 = GameObject.Find("Squar_" + strSideObjIdx_3);

                if (sideChipObj_1)
                {
                    if (isEnter)
                        sideChipObj_1.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    else
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        sideChipObj_1.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                }
                if (sideChipObj_1)
                {
                    if (isEnter)
                        sideChipObj_1.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    else
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        sideChipObj_1.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                }
                if (sideChipObj_2)
                {
                    if (isEnter)
                        sideChipObj_2.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    else
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        sideChipObj_2.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                }
                if (sideChipObj_3)
                {
                    if (isEnter)
                        sideChipObj_3.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    else
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        sideChipObj_3.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                }
            }


            if (isEnter)
                selObj.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
                selObj.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
            }

            //for 2 columns of selected pos 7, 8 of bottom row
            if (selObjIdx % 3 == 1)
            {
                int min, max;

                if (chipPos == 7)
                {
                    min = selObjIdx;
                    max = selObjIdx + 5;
                    if (max > 36) max = 36;
                    for (int i = min; i <= max; i++)
                    {
                        GameObject obj = GameObject.Find("Squar_" + i.ToString("D2"));
                        if (isEnter)
                            obj.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                        else
                        {
                            EventSystem.current.SetSelectedGameObject(null);
                            obj.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                        }
                    }
                }
                else if (chipPos == 8)
                {
                    min = selObjIdx - 3;
                    if (min < 0) min = 0;
                    max = selObjIdx + 2;
                    if (max > 36) max = 36;
                    for (int i = min; i <= max; i++)
                    {
                        GameObject obj = GameObject.Find("Squar_" + i.ToString("D2"));
                        if (isEnter)
                            obj.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                        else
                        {
                            EventSystem.current.SetSelectedGameObject(null);
                            obj.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                        }
                    }
                }
            }
        }

        private byte HoverChipPos(Vector2 canvasPos)
        {
            byte chipPos = 0;
            if (canvasPos.x >= -rectWidth / 2f && canvasPos.x <= -rectWidth * 2f / 6f)
            {
                if (canvasPos.y >= -rectHeight * 2f / 6f && canvasPos.y <= rectHeight * 2f / 6f)
                {
                    chipPos = 1;
                    //Debug.Log("This is 1 part!");
                }
                else if (canvasPos.y < -rectHeight * 2f / 6f && canvasPos.y >= -rectHeight / 2f)
                {
                    chipPos = 8;
                    //Debug.Log("This is 8 part!");
                }
                else if (canvasPos.y > rectHeight * 2f / 6f && canvasPos.y <= rectHeight / 2f)
                {
                    chipPos = 5;
                    //Debug.Log("This is 5 part!");
                }
            }
            else if (canvasPos.x > -rectWidth * 2f / 6f && canvasPos.x <= rectWidth * 2f / 6f)
            {
                if (canvasPos.y >= -rectHeight * 2f / 6f && canvasPos.y <= rectHeight * 2f / 6f)
                {
                    chipPos = 0;
                    //Debug.Log("This is 0 part!");
                }
                else if (canvasPos.y < -rectHeight * 2f / 6f && canvasPos.y >= -rectHeight / 2f)
                {
                    chipPos = 4;
                    //Debug.Log("This is 4 part!");
                }
                else if (canvasPos.y > rectHeight * 2f / 6f && canvasPos.y <= rectHeight / 2f)
                {
                    chipPos = 2;
                    //Debug.Log("This is 2 part!");
                }
            }
            else if (canvasPos.x > rectWidth * 2f / 6f && canvasPos.x <= rectWidth / 2f)
            {
                if (canvasPos.y >= -rectHeight * 2f / 6f && canvasPos.y <= rectHeight * 2f / 6f)
                {
                    chipPos = 3;
                    //Debug.Log("This is 3 part!");
                }
                else if (canvasPos.y < -rectHeight * 2f / 6f && canvasPos.y >= -rectHeight / 2f)
                {
                    chipPos = 7;
                    //Debug.Log("This is 7 part!");
                }
                else if (canvasPos.y > rectHeight * 2f / 6f && canvasPos.y <= rectHeight / 2f)
                {
                    chipPos = 6;
                    //Debug.Log("This is 6 part!");
                }
            }
            return chipPos;
        }

        public void OnObjectHoverExit(GameObject hoverObj)
        {
            //if (eventData.pointerCurrentRaycast.gameObject != null)
            if (hoverObj.name == "Squar_p12")
            {
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    //Debug.Log("Mouse Hovered  Exited GameObject = " + itemTransform.gameObject.name);
                    EventSystem.current.SetSelectedGameObject(null);
                    itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                }
            }
            else if (hoverObj.name == "Squar_m12")
            {
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                }
            }
            else if (hoverObj.name == "Squar_d12")
            {
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                }
            }
            else if (hoverObj.name == "Squar_manque")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                }
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i < 6)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_passe")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                }
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i > 5)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_impair")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    if (i % 2 == 0)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i % 2 == 0)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    if (i % 2 == 0)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_pair")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    if (i % 2 == 1)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i % 2 == 1)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    if (i % 2 == 1)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_row1")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 2)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 2)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 2)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_row2")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 1)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 1)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 1)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_row3")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 0)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 0)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    if (i % 3 == 0)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_black")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    if (i == 1 || i == 3 || i == 5 || i == 7 || i == 9 || i == 10)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i == 0 || i == 2 || i == 4 || i == 7 || i == 9 || i == 11)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    if (i == 1 || i == 3 || i == 4 || i == 6 || i == 8 || i == 10)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
            }
            else if (hoverObj.name == "Squar_red")
            {
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
                {
                    if (i == 0 || i == 2 || i == 4 || i == 6 || i == 8 || i == 11)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
                {
                    if (i == 1 || i == 3 || i == 5 || i == 6 || i == 8 || i == 10)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                i = 0;
                foreach (Transform itemTransform in scr_InsideNumG_03.GetComponent<ScrollRect>().content)
                {
                    if (i == 0 || i == 2 || i == 5 || i == 7 || i == 9 || i == 11)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
            }
            else if (hoverObj.name == "Round_RoundGroup_1")
            {
                foreach (Transform itemTransform in src_InsideNumG2_0.transform)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                }
                foreach (Transform itemTransform in scr_InsideNumG2_1.GetComponent<ScrollRect>().content)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                }
                foreach (Transform itemTransform in src_InsideNumG2_7.GetComponent<ScrollRect>().content)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                }
                //lstHoverObjectsInRound.Clear();
                isRoundPanel = false;
            }
            else if (hoverObj.name == "Round_SecondGroup")
            {
                foreach (Transform itemTransform in scr_InsideNumG2_2.GetComponent<ScrollRect>().content)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                }
                foreach (Transform itemTransform in scr_InsideNumG2_6.GetComponent<ScrollRect>().content)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                }
                //lstHoverObjectsInRound.Clear();
                isRoundPanel = false;
            }
            else if (hoverObj.name == "Round_RoundGroup_3")
            {
                foreach (Transform itemTransform in scr_InsideNumG2_4.transform)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                }
                foreach (Transform itemTransform in scr_InsideNumG2_3.GetComponent<ScrollRect>().content)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                }
                foreach (Transform itemTransform in scr_InsideNumG2_5.GetComponent<ScrollRect>().content)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                }
                //lstHoverObjectsInRound.Clear();
                isRoundPanel = false;
            }
            else if (hoverObj.name == "Round_RoundGroup_4")
            {
                foreach (Transform itemTransform in scr_InsideNumG2_4.transform)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                }
                int i = 0;
                foreach (Transform itemTransform in scr_InsideNumG2_3.GetComponent<ScrollRect>().content)
                {
                    if (i == 5)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                foreach (Transform itemTransform in scr_InsideNumG2_5.GetComponent<ScrollRect>().content)
                {
                    if (i == 5)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
                    }
                    i++;
                }
                //lstHoverObjectsInRound.Clear();
                isRoundPanel = false;
            }
            else if (hoverObj.name.Substring(0, 2) == "Ro")
            {
                for (int i = 0; i < snakeOfIdx.Length; i++)
                {
                    string curSnakeObjName = snakeOfIdx[i];
                    curSnakeObjName = "Round_" + curSnakeObjName;

                    GameObject curSnakeObj = GameObject.Find(curSnakeObjName);
                    EventSystem.current.SetSelectedGameObject(null);
                    curSnakeObj.GetComponent<Button>().OnPointerExit(canvasEventData);
                }
                isRoundPanel = false;
            }

        }

        private void ChangeChipSprite(GameObject selChipObj, float chipValue)
        {
            if (0.1f <= chipValue && chipValue < 0.5f)
            {
                selChipObj.GetComponent<Image>().sprite = spriteChip_Y;
            }
            else if (0.5f <= chipValue && chipValue < 1f)
            {
                selChipObj.GetComponent<Image>().sprite = spriteChip_B;
            }
            else if (1f <= chipValue && chipValue < 5f)
            {
                selChipObj.GetComponent<Image>().sprite = spriteChip_R;
            }
            else if (5f <= chipValue && chipValue < 10f)
            {
                selChipObj.GetComponent<Image>().sprite = spriteChip_G;
            }
            else if (10f <= chipValue && chipValue < 25f)
            {
                selChipObj.GetComponent<Image>().sprite = spriteChip_M;
            }
            else if (25f <= chipValue && chipValue <= 100f)
            {
                selChipObj.GetComponent<Image>().sprite = spriteChip_MB;
            }
        }

        public void IncreaseSnakelength()
        {
            neighbourCount++;
            if (neighbourCount > 9)
                neighbourCount = 9;
            txt_neighbourCount.GetComponent<TMP_Text>().text = neighbourCount.ToString();
        }

        public void DecreaseSnakelength()
        {
            neighbourCount--;
            if (neighbourCount < 1)
                neighbourCount = 1;
            txt_neighbourCount.GetComponent<TMP_Text>().text = neighbourCount.ToString();
        }

        GameObject instWinMark;
        IEnumerator SpinResult()
        {
            //Debug.Log("SpinResult! now =" + Time.time);
            yield return new WaitForSeconds(1.5f);
            //Debug.Log("SpinResult! after wait now =" + Time.time);

            zoomGoal.SetActive(false);

            if (isScreenSpaceMenu)
            {
                currentPanState = boardPanState.ViewSpine;
                //pan_WholeBoard.GetComponent<Animator>().SetInteger("NumBoardPos", numBoardPos);
                pan_WholeBoard.GetComponent<RectTransform>().DOAnchorPosX(0, 1f).SetEase(Ease.InOutQuad).Play();
                txtr_Render.SetActive(false);
            }

            instWinMark = GameObject.Instantiate(winMark, new Vector3(0f, 0f, 0f), Quaternion.identity);
            GameObject parentObj = GameObject.Find("Squar_" + serverWinPoint.ToString("D2"));
            instWinMark.transform.SetParent(parentObj.transform, false);
            instWinMark.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 0f, 0f);
            //instWinMark.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);     

            lstLatestNum.Add(serverWinPoint);
            if (lstLatestNum.Count > 8)
            {
                lstLatestNum.RemoveAt(0);
            }

            lstHotNum.Add(serverWinPoint);
            foreach (int hotNum in lstHotNum)
            {
                lstHotNum = lstHotNum.Distinct().ToList();
            }
            if (lstHotNum.Count > 5)
            {
                lstHotNum.RemoveAt(0);
            }
            int j = 0, k = 0, l = 0;
            for (int i = 0; i < SockIOManage.instance.gameResult.analyze.Length; i++)
            {
                if (i < 3)
                {
                    arrayRedBlack[i] = SockIOManage.instance.gameResult.analyze[i];
                }
                else if (3 <= i && i < 6)
                {
                    arrayOddEven[j] = SockIOManage.instance.gameResult.analyze[i];
                    j++;
                }
                else if (6 <= i && i < 9)
                {
                    arrayDozen[k] = SockIOManage.instance.gameResult.analyze[i];
                    k++;
                }
                else if (9 <= i && i < 12)
                {
                    arrayColumn[l] = SockIOManage.instance.gameResult.analyze[i];
                    l++;
                }
            }
            if (SockIOManage.instance.gameResult.score > 0)
            {
                txt_Win.GetComponent<TMP_Text>().text = SockIOManage.instance.gameResult.score.ToString();
                winPan.SetActive(true);
                txt_WinPan.GetComponent<TMP_Text>().text = SockIOManage.instance.gameResult.score.ToString();
            }

            UpdateInfos();

            StartCoroutine(ChipAnimationState());
        }

        IEnumerator ChipAnimationState()
        {
            yield return new WaitForSeconds(1);

            chipAniState = true;
            ChipAnimation();
        }

        Vector3 chipOutUpPos = new Vector3(888f, 1200f, 0f);
        Vector3 chipOutDownPos = new Vector3(888f, -800f, 0f);
        float upLimit = 500f;
        float downLimit = -10f;
        int j = 0;
        void ChipAnimation()
        {
            if (isScreenSpaceMenu)
            {
                chipOutUpPos = new Vector3(888f * expandRatioX, 1200f * expandRatioY, 0f);
                chipOutDownPos = new Vector3(888f * expandRatioX, -300f * expandRatioY, 0f);
                upLimit = 800f * expandRatioY;
                downLimit = -10f * expandRatioY;
            }
            else
            {
                chipOutUpPos = new Vector3(333f * expandRatioX, 600f * expandRatioY, 0f);
                chipOutDownPos = new Vector3(333f * expandRatioX, -900f * expandRatioY, 0f);
                upLimit = 300f * expandRatioY;
                downLimit = -10f * expandRatioY;
            }
            //Debug.Log("chipObjectCount =" + GameObject.FindGameObjectsWithTag("Chip").Length);
            foreach (Chip chip in dicChipObjects.Values)
            {
                GameObject chipObj = GameObject.Find(chip.name);
                if (chipObj != null)
                {
                    if (chip.buttonName == serverWinPoint.ToString("D2"))
                    {
                        chipObj.transform.DOMove(chipOutDownPos, 1).OnComplete(() => OnOneChipMoveComplete(chipObj));
                    }
                    else
                    {
                        chipObj.transform.DOMove(chipOutUpPos, 1).OnComplete(() => OnOneChipMoveComplete(chipObj));
                    }
                }
            }

        }

        private void OnOneChipMoveComplete(GameObject _chipObj)
        {
            GameObject.Destroy(_chipObj);
            j++;
            if (j == dicChipObjects.Count)
            {
                //Debug.Log("Deleted obj =" + j + ", chipObjectCount =" + GameObject.FindGameObjectsWithTag("Chip").Length);
                foreach (GameObject chipObj in GameObject.FindGameObjectsWithTag("Chip"))
                {
                    //Debug.Log("chipObj = " + chipObj.name);
                    GameObject.Destroy(chipObj);
                }
                // following setting variable should be here that before starting rebet of autospin
                j = 0;
                chipAniState = false; // should be here for following RebetAndSpin.
                isSpinBet = false; // should be here for following RebetAndSpin.
                serverWinPoint = -1;

                txt_Cach.GetComponent<TMP_Text>().text = SockIOManage.instance.fun.ToString();

                if (instWinMark)
                    GameObject.Destroy(instWinMark, 1.5f);
                winPan.SetActive(false);
                //Debug.Log("nAutoCount = " + nAutoCount + ", currentAutoCount =" + currentAutoCount);
                if (0 < nAutoCount && nAutoCount > currentAutoCount)
                {
                    //Debug.Log("Start Auto Spin_" + currentAutoCount + " will be run!!");
                    currentAutoCount++;
                    btn_AutoSpin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Stop \n " + currentAutoCount.ToString();
                    RebetAndSpin();
                    //if (currentAutoCount > nAutoCount) nAutoCount = 0;
                }
                else
                {
                    btn_AutoSpin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Auto\nSpin";
                    nAutoCount = 0;
                    currentAutoCount = 0;

                    grp_PresetBets.SetActive(true);
                    btn_PayTable.SetActive(true);
                    grp_ViewMode.SetActive(true);
                    if (isScreenSpaceMenu)
                        btn_Neighbour.SetActive(true);

                    btn_Rebet.SetActive(true);
                    btn_RebetAndSpin.SetActive(true);
                }

                //Debug.Log("ChipAnimation is ended!");
            }
        }

        public void ViewAutoSpinePan()
        {
            if (nAutoCount == 0)
            {
                if (isSpinBet) return;
                if (!beginBet) return;
                if (pan_AutoSpin.activeSelf)
                    pan_AutoSpin.SetActive(true);
                else 
                    pan_AutoSpin.SetActive(true);
            }
            else
            {
                nAutoCount = 0;
                currentAutoCount = 0;
                btn_AutoSpin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Auto\nSpin";
            }
        }

        public void AoutoSpin(int countAuto)
        {
            pan_AutoSpin.SetActive(false);
            currentAutoCount++;
            btn_AutoSpin.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Stop \n " + currentAutoCount.ToString();
            nAutoCount = countAuto;
            RebetAndSpin();

        }

        public void Rebet()
        {
            foreach (Chip chip in dicChipObjects.Values)
            {
                //Debug.Log("chip.name = " + chip.name);
                CreatChipFromDicData(chip);
            }
            beginBet = true;
            btn_Rebet.SetActive(false);
            btn_RebetAndSpin.SetActive(false);
            ViewRightDownButton(true);
        }

        public void RebetAndSpin()
        {
            //Debug.Log("RebetAndSpin!");
            foreach (Chip chip in dicChipObjects.Values)
            {
                //Debug.Log("chip.name = " + chip.name);
                CreatChipFromDicData(chip);
            }
            btn_Rebet.SetActive(false);
            btn_RebetAndSpin.SetActive(false);
            beginBet = true;
            SpineBet();
        }

        void Update()
        {
            //if (isScreenSpaceMenu && currentPanState != boardPanState.ViewSpine )
            //txtr_Render.SetActive(false);
            //txt_Ch
            if (Screen.width < Screen.height)
                img_Rotate.SetActive(true);
            else 
                img_Rotate.SetActive(false);

            if (beginBet)
            {
                ViewRightDownButton(true);
            }
            else
            {
                ViewRightDownButton(false);
            }

            if (tiggerWinStat) // history index should be initalized!
            {
                Debug.Log("Update: winStat!");
                tiggerWinStat = false;
                StartCoroutine(SpinResult());
            }

            if (trigerSpinBet && serverWinPoint >= 0)
            {
                Debug.Log("Update: SpineBet!");
                if (isScreenSpaceMenu)
                {
                    btn_Rollet.SetActive(false);
                    ViewSpinPanel();
                }
                Spine.instance.StartSpinBet(serverWinPoint);

                ViewRightDownButton(false);
                grp_PresetBets.SetActive(false);
                btn_PayTable.SetActive(false);
                grp_ViewMode.SetActive(false);
                btn_Back_2.SetActive(false);
                trigerSpinBet = false;
            }

            if (isScreenSpaceMenu)
                realtimeMousePos = Input.mousePosition;
            if (isSquarButtonHover)
            {
                HoverSquarButtons();
            }

            //Debug.Log("Update: beginBet = " + beginBet);
            //Debug.Log("Tecture Render = " + txtr_Render.activeSelf);
            /* // don't use this for delay action.
            if (dicChipObjects.Count > 0)
            {
                beginBet = true;
                if (isScreenSpaceMenu )
                    btn_Rollet.SetActive(true);
            }
            else
            {           
                beginBet = false;
                btn_Rollet.SetActive(false);
            }*/
        }

        public void OnSquarButtonHover(GameObject _hoverObj)
        {
            hoverSquarButtonObj = _hoverObj;
            isSquarButtonHover = true;
        }

        public void OnSquarButtonHoverExit(GameObject _hoverObj)
        {
            hoverSquarButtonObj = _hoverObj;
            isSquarButtonHover = false;
        }

        public void GotoRoundPanel()
        {
            currentPanState = boardPanState.ViewRound;
            //pan_WholeBoard.GetComponent<Animator>().SetInteger("NumBoardPos", numBoardPos);
            pan_WholeBoard.GetComponent<RectTransform>().DOAnchorPosX(-Screen.width, 1f).SetEase(Ease.InOutQuad).Play();
            btn_Neighbour.SetActive(false);
            btn_Rollet.SetActive(false);
            txtr_Render.SetActive(false);
            StartCoroutine(ViewDelayButtons(btn_Back));
            //Debug.Log("Screen Width = " + Screen.width);
        }

        public void GotoSquarePanel(GameObject thisButton)
        {
            currentPanState = boardPanState.ViewRound;
            //pan_WholeBoard.GetComponent<Animator>().SetInteger("NumBoardPos", numBoardPos);
            pan_WholeBoard.GetComponent<RectTransform>().DOAnchorPosX(0f, 1f).SetEase(Ease.InOutQuad).Play();
            thisButton.SetActive(false);
            txtr_Render.SetActive(false);
            StartCoroutine(ViewDelayButtons(btn_Neighbour));
            if (beginBet)
                StartCoroutine(ViewDelayButtons(btn_Rollet));
            Debug.Log("Screen Width = " + Screen.width);
        }

        IEnumerator ViewDelayButtons(GameObject button)
        {
            yield return new WaitForSeconds(1);
            if (!isSpinBet)
                button.SetActive(true);
        }

        public void SpineBet()
        {
            Debug.Log("SpinBet!");
            if (isSpinBet) return;
            if (!beginBet) return;
            if (float.Parse(txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text) <= 0)
                return;
            if (float.Parse(txt_Cach.GetComponent<TMP_Text>().text) < float.Parse(txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text))
                return;

            txt_Cach.GetComponent<TMP_Text>().text = (SockIOManage.instance.fun - float.Parse(txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text)).ToString();
            txt_Win.GetComponent<TMP_Text>().text = "";

            beginBet = false;
            isSpinBet = true;            

            IEnumerable<Chip> valuesforjson = dicChipObjects.Where(pair => pair.Value.name.Substring(0, 2) != "Ro" && pair.Value.buttonName.Substring(0, 2) != "Ro").Select(pair => pair.Value);
            string json = JsonConvert.SerializeObject(valuesforjson);
            Debug.Log("Spin: json = " + json);
            SockIOManage.instance.sioCom.Instance.Emit("bet", json, false);

            //for local test;
            //serverWinPoint = UnityEngine.Random.Range(1, 33);
            //trigerSpinBet = true;
        }

        public void ViewInfo ()
        {
            if (pan_Info.activeSelf)
                pan_Info.SetActive(false);
            else
                pan_Info.SetActive(true);
        }

        public void ViewInfo_False()
        {
            pan_Info.SetActive(false);
        }    

        private void LoadData(byte index)
        {
            ClearAll();

            string filePath = Application.persistentDataPath + "/CED_data_" + index + ".dat";
            byte[] data;

            if (File.Exists(filePath))
            {
                FileStream file = File.Open(filePath, FileMode.Open);
                data = new byte[file.Length];
                file.Read(data, 0, data.Length);
                file.Close();

                if (data != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream stream = new MemoryStream(data);
                    dicChipObjects = (Dictionary<string, Chip>)bf.Deserialize(stream);
                    stream.Close();
                }
            }

            historyIndex = 0;
            foreach (Chip chip in dicChipObjects.Values)
            {
                //Debug.Log("Get data from saved data: Chip.name = " + chip.name);
                CreatChipFromDicData(chip);
            }
        }

        public void OnLoadAClick(GameObject buttonObjIdx)
        {
            if (!BetButtonState()) return;

            byte index = byte.Parse(buttonObjIdx.name.Substring(buttonObjIdx.name.Length - 1));
            //Debug.Log("index = " + index);
            LoadData(index); // beginBet is false at LoadData.
            TotalValue();
            beginBet = true;

        }

        private void CreatChipFromDicData(Chip curChip)
        {
            //Debug.Log("CreatChipsFromData!");
            Vector3 vChipPos = new Vector3(0f, 0f, 0f);
            GameObject chipInstant = GameObject.Instantiate(prefabChip, vChipPos, Quaternion.identity);
            chipInstant.name = curChip.name;
            string parentName = "";
            if (curChip.buttonName.Substring(0, 2) == "Ro") // Round. if length 5, have erro because of length problem such as 02;
            {
                parentName = "Round_" + curChip.buttonName;
                //AddHistory(curChip.name, curChip.chipValue, true);
            }
            else
            {
                parentName = "Squar_" + curChip.buttonName;
                //AddHistory(curChip.name, curChip.chipValue);
            }
            //Debug.Log("parentName = " + parentName);

            GameObject buttonInSquare = GameObject.Find(parentName);
            chipInstant.transform.SetParent(buttonInSquare.transform, false);
            vChipPos = GetRecPos(buttonInSquare, curChip.positionInButton);
            chipInstant.GetComponent<RectTransform>().anchoredPosition = vChipPos;
            ChangeChipSprite(chipInstant, curChip.chipValue);
            chipInstant.transform.GetChild(0).GetComponent<TMP_Text>().text = curChip.chipValue.ToString();           
            
        }

        private Vector2 GetRecPos(GameObject selObj, int chipPos)
        {
            Vector2 vChipPos = new Vector2(0f, 0f);
            rectWidth = selObj.GetComponent<RectTransform>().rect.width;
            rectHeight = selObj.GetComponent<RectTransform>().rect.height;

            switch (chipPos)
            {
                case 0:
                    {
                        vChipPos = new Vector3(0f, 0f, 0f);
                        break;
                    }
                case 1:
                    {
                        vChipPos = new Vector3(-rectWidth / 2f, 0f, 0f);
                        break;
                    }
                case 2:
                    {
                        vChipPos = new Vector3(0f, rectHeight / 2f, 0f);
                        break;
                    }
                case 3:
                    {
                        vChipPos = new Vector3(rectWidth / 2f, 0f, 0f);
                        break;
                    }
                case 4:
                    {
                        vChipPos = new Vector3(0f, -rectHeight / 2f, 0f);
                        break;
                    }
                case 5:
                    {
                        vChipPos = new Vector3(-rectWidth / 2f, rectHeight / 2f, 0f);
                        break;
                    }
                case 6:
                    {
                        vChipPos = new Vector3(rectWidth / 2f, rectHeight / 2f, 0f);
                        break;
                    }
                case 7:
                    {
                        vChipPos = new Vector3(rectWidth / 2f, -rectHeight / 2f, 0f);
                        break;
                    }
                case 8:
                    {
                        vChipPos = new Vector3(-rectWidth / 2f, -rectHeight / 2f, 0f);
                        break;
                    }
            }
            return vChipPos;
        }
        private void UpdateInfos()
        {
            // LastesNum
            int i = 0;
            foreach (Transform item in grp_LastedNum.transform)
            {
                if (grp_LastedNum.transform.GetChild(i).gameObject.name.Substring(0, 4) == "txt_")
                    grp_LastedNum.transform.GetChild(i).gameObject.GetComponent<TMP_Text>().text = "";
                i++;
            }
            i = 0;
            foreach (int item in lstLatestNum)
            {
                i++;
                GameObject itemObj = GameObject.Find("txt_Lastest_" + i.ToString());
                itemObj.GetComponent<TMP_Text>().text = item.ToString();
            }

            // Hot Num
            i = 0;
            string strHotNum = "";
            foreach (int item in lstHotNum)
            {
                strHotNum += item;
                strHotNum += "   ";
            }
            grp_HotColdNum.transform.GetChild(3).gameObject.GetComponent<TMP_Text>().text = strHotNum;

            // Red and Black: child 1, 2, 3 is color bars and 4, 5, 6 is text according to color bars.
            grp_RedBlack.transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(arrayRedBlack[0] / 100f, 1f, 1f);
            grp_RedBlack.transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition = grp_RedBlack.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_RedBlack.transform.GetChild(1).GetComponent<RectTransform>().rect.width / 2f * arrayRedBlack[0] / 100f, 0f);
            if (arrayRedBlack[0] != 0)
                grp_RedBlack.transform.GetChild(4).GetComponent<TMP_Text>().text = arrayRedBlack[0].ToString() + "%";

            grp_RedBlack.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = grp_RedBlack.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_RedBlack.transform.GetChild(1).GetComponent<RectTransform>().rect.width * arrayRedBlack[0] / 100f, 0f);
            grp_RedBlack.transform.GetChild(2).GetComponent<RectTransform>().localScale = new Vector3(arrayRedBlack[1] / 100f, 1f, 1f);
            grp_RedBlack.transform.GetChild(5).GetComponent<RectTransform>().anchoredPosition = grp_RedBlack.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_RedBlack.transform.GetChild(2).GetComponent<RectTransform>().rect.width / 2f * arrayRedBlack[1] / 100f, 0f); ;
            if (arrayRedBlack[1] != 0)
                grp_RedBlack.transform.GetChild(5).GetComponent<TMP_Text>().text = arrayRedBlack[1].ToString() + "%";

            grp_RedBlack.transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = grp_RedBlack.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_RedBlack.transform.GetChild(1).GetComponent<RectTransform>().rect.width * arrayRedBlack[1] / 100f, 0f);
            grp_RedBlack.transform.GetChild(3).GetComponent<RectTransform>().localScale = new Vector3(arrayRedBlack[2] / 100f, 1f, 1f);
            grp_RedBlack.transform.GetChild(6).GetComponent<RectTransform>().anchoredPosition = grp_RedBlack.transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_RedBlack.transform.GetChild(3).GetComponent<RectTransform>().rect.width / 2f * arrayRedBlack[2] / 100f, 0f); ;
            if (arrayRedBlack[2] != 0)
                grp_RedBlack.transform.GetChild(6).GetComponent<TMP_Text>().text = arrayRedBlack[2].ToString() + "%";

            // Odd and Enen: child 1, 2, 3 is color bars and 4, 5, 6 is text according to color bars.
            grp_OddEven.transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(arrayOddEven[0] / 100f, 1f, 1f);
            grp_OddEven.transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition = grp_OddEven.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_OddEven.transform.GetChild(1).GetComponent<RectTransform>().rect.width / 2f * arrayOddEven[0] / 100f, 0f);
            if (arrayOddEven[0] != 0)
                grp_OddEven.transform.GetChild(4).GetComponent<TMP_Text>().text = arrayOddEven[0].ToString() + "%";

            grp_OddEven.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = grp_OddEven.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_OddEven.transform.GetChild(1).GetComponent<RectTransform>().rect.width * arrayOddEven[0] / 100f, 0f);
            grp_OddEven.transform.GetChild(2).GetComponent<RectTransform>().localScale = new Vector3(arrayOddEven[1] / 100f, 1f, 1f);
            grp_OddEven.transform.GetChild(5).GetComponent<RectTransform>().anchoredPosition = grp_OddEven.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_OddEven.transform.GetChild(2).GetComponent<RectTransform>().rect.width / 2f * arrayOddEven[1] / 100f, 0f); ;
            if (arrayOddEven[1] != 0)
                grp_OddEven.transform.GetChild(5).GetComponent<TMP_Text>().text = arrayOddEven[1].ToString() + "%";

            grp_OddEven.transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = grp_OddEven.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_OddEven.transform.GetChild(1).GetComponent<RectTransform>().rect.width * arrayOddEven[1] / 100f, 0f);
            grp_OddEven.transform.GetChild(3).GetComponent<RectTransform>().localScale = new Vector3(arrayOddEven[2] / 100f, 1f, 1f);
            grp_OddEven.transform.GetChild(6).GetComponent<RectTransform>().anchoredPosition = grp_OddEven.transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_OddEven.transform.GetChild(3).GetComponent<RectTransform>().rect.width / 2f * arrayOddEven[2] / 100f, 0f); ;
            if (arrayOddEven[2] != 0)
                grp_OddEven.transform.GetChild(6).GetComponent<TMP_Text>().text = arrayOddEven[2].ToString() + "%";

            // Dozens: child 1, 2, 3 is color bars and 4, 5, 6 is text according to color bars.
            grp_Dozen.transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(arrayDozen[0] / 100f, 1f, 1f);
            grp_Dozen.transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition = grp_Dozen.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_Dozen.transform.GetChild(1).GetComponent<RectTransform>().rect.width / 2f * arrayDozen[0] / 100f, 0f);
            if (arrayDozen[0] != 0)
                grp_Dozen.transform.GetChild(4).GetComponent<TMP_Text>().text = arrayDozen[0].ToString() + "%";

            grp_Dozen.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = grp_Dozen.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_Dozen.transform.GetChild(1).GetComponent<RectTransform>().rect.width * arrayDozen[0] / 100f, 0f);
            grp_Dozen.transform.GetChild(2).GetComponent<RectTransform>().localScale = new Vector3(arrayDozen[1] / 100f, 1f, 1f);
            grp_Dozen.transform.GetChild(5).GetComponent<RectTransform>().anchoredPosition = grp_Dozen.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_Dozen.transform.GetChild(2).GetComponent<RectTransform>().rect.width / 2f * arrayDozen[1] / 100f, 0f); ;
            if (arrayDozen[1] != 0)
                grp_Dozen.transform.GetChild(5).GetComponent<TMP_Text>().text = arrayDozen[1].ToString() + "%";

            grp_Dozen.transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = grp_Dozen.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_Dozen.transform.GetChild(1).GetComponent<RectTransform>().rect.width * arrayDozen[1] / 100f, 0f);
            grp_Dozen.transform.GetChild(3).GetComponent<RectTransform>().localScale = new Vector3(arrayDozen[2] / 100f, 1f, 1f);
            grp_Dozen.transform.GetChild(6).GetComponent<RectTransform>().anchoredPosition = grp_Dozen.transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_Dozen.transform.GetChild(3).GetComponent<RectTransform>().rect.width / 2f * arrayDozen[2] / 100f, 0f); ;
            if (arrayDozen[2] != 0)
                grp_Dozen.transform.GetChild(6).GetComponent<TMP_Text>().text = arrayDozen[2].ToString() + "%";

            // Column child 1, 2, 3 is color bars and 4, 5, 6 is text according to color bars.
            grp_Column.transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(arrayColumn[0] / 100f, 1f, 1f);
            grp_Column.transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition = grp_Column.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_Column.transform.GetChild(1).GetComponent<RectTransform>().rect.width / 2f * arrayColumn[0] / 100f, 0f);
            if (arrayColumn[0] != 0)
                grp_Column.transform.GetChild(4).GetComponent<TMP_Text>().text = arrayColumn[0].ToString() + "%";

            grp_Column.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = grp_Column.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_Column.transform.GetChild(1).GetComponent<RectTransform>().rect.width * arrayColumn[0] / 100f, 0f);
            grp_Column.transform.GetChild(2).GetComponent<RectTransform>().localScale = new Vector3(arrayColumn[1] / 100f, 1f, 1f);
            grp_Column.transform.GetChild(5).GetComponent<RectTransform>().anchoredPosition = grp_Column.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_Column.transform.GetChild(2).GetComponent<RectTransform>().rect.width / 2f * arrayColumn[1] / 100f, 0f); ;
            if (arrayColumn[1] != 0)
                grp_Column.transform.GetChild(5).GetComponent<TMP_Text>().text = arrayColumn[1].ToString() + "%";

            grp_Column.transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = grp_Column.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_Column.transform.GetChild(1).GetComponent<RectTransform>().rect.width * arrayColumn[1] / 100f, 0f);
            grp_Column.transform.GetChild(3).GetComponent<RectTransform>().localScale = new Vector3(arrayColumn[2] / 100f, 1f, 1f);
            grp_Column.transform.GetChild(6).GetComponent<RectTransform>().anchoredPosition = grp_Column.transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition +
                new Vector2(grp_Column.transform.GetChild(3).GetComponent<RectTransform>().rect.width / 2f * arrayColumn[2] / 100f, 0f); ;
            if (arrayColumn[2] != 0)
                grp_Column.transform.GetChild(6).GetComponent<TMP_Text>().text = arrayColumn[2].ToString() + "%";
        }

        public void SwitchScreenAndWorldMenu(int buttonType)
        {
            switch (buttonType)
            {
                case 0:
                    {
                        isScreenSpaceMenu = false;
                        break;
                    }
                case 1:
                    {
                        isScreenSpaceMenu = true;
                        break;
                    }
                case 2:
                    {
                        isScreenSpaceMenu = !isScreenSpaceMenu;
                        break;
                    }
            }

            if (!isScreenSpaceMenu)
            {
                obj_CanvasWorld.SetActive(true);
                pan_WholeBoard.SetActive(false);
                img_ScreenBG.SetActive(false);
                txtr_Render.SetActive(true);
                opt_SwitchBtton.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(-22f, -2.6f);

                btn_Rollet.SetActive(false);
                btn_Neighbour.SetActive(false);
                btn_Back.SetActive(false);

                pan_SquareControlGroup.transform.localScale = new Vector3(0.72f, 0.64f, 1f);
                pan_SquareControlGroup.transform.SetParent(obj_CanvasWorld.transform, false);
                pan_SquareControlGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -50f);
                pan_SquareControlGroup.GetComponent<RectTransform>().anchorMin = new Vector2(0.79f, 0.5f);

                pan_RoundControlGroup.transform.localScale = new Vector3(0.7f, 0.45f, 1f);
                pan_RoundControlGroup.transform.SetParent(obj_CanvasWorld.transform, false);
                pan_RoundControlGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(300f, 240f);

                pan_NeighbourCounterGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(120f, 380f);
                pan_NeighbourCounterGroup.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.5f, 1.8f);

                txtr_Render.GetComponent<RectTransform>().anchoredPosition = new Vector2(-626f, 0f);
                txtr_Render.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                obj_CanvasWorld.SetActive(false);
                pan_WholeBoard.SetActive(true);
                img_ScreenBG.SetActive(true);
                txtr_Render.SetActive(false);
                opt_SwitchBtton.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(16.6f, -2.6f);

                btn_Neighbour.SetActive(true);
                if (beginBet)
                    btn_Rollet.SetActive(true);

                pan_SquareControlGroup.transform.SetParent(pan_WholeBoard.transform, false);
                pan_SquareControlGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0f);
                pan_SquareControlGroup.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                pan_SquareControlGroup.transform.localScale = new Vector3(1f, 1f, 1f); //new Vector3(expandRatioX, expandRatioY, 1f);

                pan_RoundControlGroup.transform.localScale = new Vector3(1f, 1f, 1f); //new Vector3(expandRatioX, expandRatioY, 1f);
                pan_RoundControlGroup.transform.SetParent(pan_WholeBoard.transform, false);
                pan_RoundControlGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(ConstVars.designeWidth, 0f);

                pan_NeighbourCounterGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(50f, -280f);
                pan_NeighbourCounterGroup.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f); //new Vector3(expandRatioX, expandRatioY, 1f);
                currentPanState = boardPanState.ViewSquare;

                pan_WholeBoard.GetComponent<RectTransform>().DOAnchorPosX(0f, 1f).SetEase(Ease.InOutQuad).Play();
                //Debug.Log("SwitchScreenAndWorld: LocalScal = " + new Vector3(expandRatioX, expandRatioY, 1f));
            }
            Debug.Log("Screen Width = " + Screen.width);
        }

        private void ViewRightDownButton(bool beginBet = false)
        {
            btn_Undo.SetActive(beginBet);
            btn_Clear.SetActive(beginBet);
            btn_Double.SetActive(beginBet);
        }

        public void ViewSpinPanel()
        {
            img_ScreenBG.SetActive(true);
            txtr_Render.SetActive(true);
            btn_Rollet.SetActive(false);
            btn_Neighbour.SetActive(false);

            StartCoroutine(ViewDelayButtons(btn_Back_2));

            txtr_Render.GetComponent<RectTransform>().anchoredPosition = new Vector2(-250f, 10f);
            txtr_Render.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1.2f);
            currentPanState = boardPanState.ViewSpine;
            //pan_WholeBoard.GetComponent<Animator>().SetInteger("NumBoardPos", numBoardPos);
            pan_WholeBoard.GetComponent<RectTransform>().DOAnchorPosX(Screen.width, 1f).SetEase(Ease.InOutQuad).Play();

        }
        public void IncreasBetValue()
        {
            curBetValuesIdx++;
            if (curBetValuesIdx > 5)
                curBetValuesIdx = 0;
            txt_Value.GetComponent<TMP_Text>().text = betValues[curBetValuesIdx].ToString();
        }

        public void DecreasBetValue()
        {
            curBetValuesIdx--;
            if (curBetValuesIdx < 0)
                curBetValuesIdx = 5;
            txt_Value.GetComponent<TMP_Text>().text = betValues[curBetValuesIdx].ToString();
        }

        public void OpenAnalysitc()
        {
            if (!pan_Analystic.activeSelf)
                pan_Analystic.SetActive(true);
            else
                pan_Analystic.SetActive(false);
        }

        public void CloseAnalysitc()
        {
            pan_Analystic.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                canvasEventData = eventData;
                //Debug.Log("mouse entered in " + eventData.pointerCurrentRaycast.gameObject.name);
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                canvasEventData = eventData;
                //Debug.Log("mouse Clicked in " + eventData.pointerCurrentRaycast.gameObject.name);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                canvasEventData = eventData;
                //Debug.Log("mouse exited from " + eventData.pointerCurrentRaycast.gameObject.name);
            }
        }
    }

}
