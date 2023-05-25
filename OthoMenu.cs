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
    public GameObject RawImage;
    [SerializeField]
    private GameObject txt_TotalBet;
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
    private GameObject img_SpinBG;
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

    [HideInInspector]
    public bool winStat = false;
    private bool beginBet = false; // false : there is no chip, true: there are one more chips.
    private sbyte numBoardPos = 0;
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
    private byte historyIndex = 0;
    private float rectWidth;// chip postion button rect width
    private float rectHeight;// chip postion button rect height
    private bool isRoundPanel = false;
    private List<string> lstHoverObjectsInRound = new List<string>();

    private List<int> lstLastestNum = new List<int>();
    private List<int> lstHotNum = new List<int>();
    private int[] arrayRedBlack = new int[3];
    private int[] arrayOddEven = new int[3];
    private int[] arrayDozen = new int[3];
    private int[] arrayColumn = new int[3];
    private bool isSquarButtonHover = false;
    private GameObject hoverSquarButtonObj;
    private byte prevChipPos = 0;

    //private int[] arrayColdNum = new int[] { 12, 13, 26, 6, 14 };
    [HideInInspector]
    public string tokenValue = "";

    public static OthoMenu instance;



    private void Awake()
    {
        if (!instance)
            instance = this;

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

        string url = UseJSLib.GetSearchParams();
        if (url != "")
            tokenValue = url.Substring(url.IndexOf("=") + 1); //HttpUtility.ParseQueryString(new Uri(url).Query).Get("cert");
        Debug.Log("_URL = " + url + ", tokenValue = " + tokenValue);
    }

    public void OnBetRoundInside(GameObject selObj)
    {
        beginBet = true;
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

            string strObjectNameInSquare = "Squar_" + curSnakeObjIdx;
            GameObject ChipInSquareObj = GameObject.Find(strObjectNameInSquare);
            OnBetAction(ChipInSquareObj);
        }
    }

    public void OnBetOutSide(GameObject selObj)
    {
        beginBet = true;
        if (isScreenSpaceMenu)
            btn_Rollet.SetActive(true);

        float curBetValue = betValues[curBetValuesIdx];
        GameObject selChipObj = GameObject.Find("Chip_" + selObj.name.Substring(6));

        if (selChipObj)
        {
            float chipValue = float.Parse(selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text) + curBetValue;
            ChangeChipSprite(selChipObj, chipValue);

            selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();

            dicChipObjects[selChipObj.name].chipValue = chipValue;

            //------------ Round panelound panel is refected to Square Panel-----------
            if (!isRoundPanel) return; //numBoardPos != -1

            foreach (string itemObjName in lstHoverObjectsInRound)
            {
                Debug.Log("itemObjName = " + itemObjName);
                string buttonIdx = itemObjName.Substring(itemObjName.Length - 2);
                GameObject chipObj = GameObject.Find("Chip_" + buttonIdx + "-0");
                if (!chipObj) continue;

                ChangeChipSprite(chipObj, chipValue);

                chipObj.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();
                dicChipObjects[chipObj.name].chipValue = chipValue;
            }
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
            chipObj.chipValue = curBetValue;
            chipObj.buttonName = selObj.name.Substring(6);
            chipObj.historyIndex = historyIndex;
            chipObj.name = chipInstant.name;
            dicChipObjects.Add(chipInstant.name, chipObj);
            //Debug.Log("current history value: " + historyIndex);
            historyIndex++;
            ChangeChipSprite(chipInstant, curBetValue);

            // ------------ Round panel is refected to Square Panel-----------
            if (!isRoundPanel) return;
            foreach (string itemObjName in lstHoverObjectsInRound)
            {
                Debug.Log("itemObjName = " + itemObjName);
                string buttonIdx = itemObjName.Substring(itemObjName.Length - 2);
                GameObject chipNumObj = GameObject.Find("Chip_" + buttonIdx + "-0");
                if (chipNumObj) continue;

                string parentName = "Squar_" + buttonIdx;
                GameObject parentObj = GameObject.Find(parentName);
                if (!parentObj) continue;

                vChipPos = new Vector3(0f, 0f, 0f);
                chipInstant = GameObject.Instantiate(prefabChip, vChipPos, Quaternion.identity);
                chipInstant.name = "Chip_" + buttonIdx + "-0";
                chipInstant.transform.SetParent(parentObj.transform, false);
                chipInstant.GetComponent<RectTransform>().anchoredPosition = vChipPos;
                chipInstant.transform.GetChild(0).GetComponent<TMP_Text>().text = curBetValue.ToString();

                chipObj = new Chip();
                chipObj.chipValue = curBetValue;
                chipObj.buttonName = selObj.name.Substring(6);
                chipObj.historyIndex = historyIndex;
                chipObj.name = chipInstant.name;
                dicChipObjects.Add(chipInstant.name, chipObj);
                //Debug.Log("current history value: " + historyIndex);
                Debug.Log("NewChipObject Name = " + chipInstant.name);

                historyIndex++;
                ChangeChipSprite(chipInstant, curBetValue);
            }
        }

        TotalValue();
    }

    public void OnBetAction(GameObject selObj) // for number buttons
    {
        beginBet = true;
        ViewRightDownButtons(true);
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
        }
        else if (selChipObj)
        {
            float chipValue = float.Parse(selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text) + curBetValue;
            ChangeChipSprite(selChipObj, chipValue);
            selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();
            dicChipObjects[selChipObj.name].chipValue = chipValue;
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
        }
        else if (sideChipObj_2)
        {
            float chipValue = float.Parse(sideChipObj_2.transform.GetChild(0).GetComponent<TMP_Text>().text) + curBetValue;
            ChangeChipSprite(sideChipObj_2, chipValue);
            sideChipObj_2.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();
            dicChipObjects[sideChipObj_2.name].chipValue = chipValue;
        }
        else if (sideChipObj_3)
        {
            float chipValue = float.Parse(sideChipObj_3.transform.GetChild(0).GetComponent<TMP_Text>().text) + curBetValue;
            ChangeChipSprite(sideChipObj_3, chipValue);
            sideChipObj_3.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();
            dicChipObjects[sideChipObj_3.name].chipValue = chipValue;
        }
        else if (selChipObj)
        {
            float chipValue = float.Parse(selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text) + curBetValue;
            ChangeChipSprite(selChipObj, chipValue);
            selChipObj.transform.GetChild(0).GetComponent<TMP_Text>().text = chipValue.ToString();
            dicChipObjects[selChipObj.name].chipValue = chipValue;
        }
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
        chipObj.chipValue = chipValue;
        if (selObj.name.Substring(0, 5) == "Squar")
            chipObj.buttonName = strSelObjIdx;// "RoundChip"
        else
            chipObj.buttonName = "RoundChip";
        chipObj.positionInButton = chipPos;
        chipObj.name = chipInstant.name;
        chipObj.historyIndex = historyIndex;
        dicChipObjects.Add(chipInstant.name, chipObj);
        //Debug.Log("current history value: " + historyIndex);
        historyIndex++;
    }

    public void Undo()
    {
        //int maxValue = dicChipObjects.Values.Max(chip => chip.historyIndex );

        if (dicChipObjects.Values.Count > 0)
        {
            Chip recentChip = dicChipObjects.Values.OrderByDescending(chip => chip.historyIndex).First();
            dicChipObjects.Remove(dicChipObjects.First(chipObj => chipObj.Value == recentChip).Key);
            GameObject recentChipObj = GameObject.Find(recentChip.name);
            GameObject.Destroy(recentChipObj);
            float newTotal = float.Parse(txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text) - recentChip.chipValue;
            txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text = newTotal.ToString();
            //Debug.Log($"Deleted chip object: name - {recentChip.name }, Value - {recentChip.chipValue }"); 
        }
        else
        {
            Debug.Log("Dictionary is empty ");
        }
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

        btn_Rollet.SetActive(false);
        beginBet = false;
    }

    public void Double()
    {
        foreach (Chip chip in dicChipObjects.Values)
        {
            chip.chipValue *= 2f;
            GameObject chipObj = GameObject.Find(chip.name);
            chipObj.transform.GetChild(0).GetComponent<TMP_Text>().text = chip.chipValue.ToString();
        }
        float newTotal = float.Parse(txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text) * 2f;
        txt_TotalBet.transform.GetChild(0).GetComponent<TMP_Text>().text = newTotal.ToString();
    }
    private void TotalValue()
    {
        float totalValue = 0f;
        Dictionary<string, Chip>.ValueCollection values = dicChipObjects.Values;
        foreach (Chip val in values)
        {
            //Debug.Log ("ChipObj: chipNum = " + val.chipNum + ", chipPos = " + val.chipPos + ", chipValue" + val.chipValue );
            if (val.buttonName != "RoundChip")
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
        if (hoverObj.name == "Squar_p12" )
        {
            foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
            {
                //Debug.Log("Mouse Hovered GameObject = " + itemTransform.gameObject.name);
                itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
            }
        }
        else if (hoverObj.name == "Squar_m12" )
        {
            foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
            {
                itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
            }
        }
        else if (hoverObj.name == "Squar_d12" )
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
        else if (hoverObj.name == "Round_FirstGroup")
        {
            foreach (Transform itemTransform in src_InsideNumG2_0.transform)
            {
                itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
            }
            foreach (Transform itemTransform in scr_InsideNumG2_1.GetComponent<ScrollRect>().content)
            {
                itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
            }
            foreach (Transform itemTransform in src_InsideNumG2_7.GetComponent<ScrollRect>().content)
            {
                itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
            }
            isRoundPanel = true;
        }
        else if (hoverObj.name == "Round_SecondGroup")
        {
            foreach (Transform itemTransform in scr_InsideNumG2_2.GetComponent<ScrollRect>().content)
            {
                itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
            }
            foreach (Transform itemTransform in scr_InsideNumG2_6.GetComponent<ScrollRect>().content)
            {
                itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
            }
            isRoundPanel = true;
        }
        else if (hoverObj.name == "Round_ThirdGroup")
        {
            foreach (Transform itemTransform in scr_InsideNumG2_4.transform)
            {
                itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
            }
            foreach (Transform itemTransform in scr_InsideNumG2_3.GetComponent<ScrollRect>().content)
            {
                itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
            }
            foreach (Transform itemTransform in scr_InsideNumG2_5.GetComponent<ScrollRect>().content)
            {
                itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
            }
            isRoundPanel = true;
        }
        else if (hoverObj.name == "Round_ZeroGroup")
        {
            foreach (Transform itemTransform in scr_InsideNumG2_4.transform)
            {
                itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
            }
            int i = 0;
            foreach (Transform itemTransform in scr_InsideNumG2_3.GetComponent<ScrollRect>().content)
            {
                if (i == 5)
                {
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
                }
                i++;
            }
            foreach (Transform itemTransform in scr_InsideNumG2_5.GetComponent<ScrollRect>().content)
            {
                if (i == 5)
                {
                    itemTransform.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
                    lstHoverObjectsInRound.Add(itemTransform.gameObject.name);
                }
                i++;
            }
            isRoundPanel = true;
        }
        else if (hoverObj.name.Substring(0, 5) == "Round") //RoundNum
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
        else if (hoverObj.name.Substring(0, 5) == "Squar")
        {
            Vector2 canvasPos;
            if (isScreenSpaceMenu)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(hoverObj.GetComponent<RectTransform>(),
                    canvasEventData.position, null, out canvasPos))
                {
                    //Debug.Log("Mouse Position in Canvas Space: " + canvasPos);
                }
            }
            else
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(hoverObj.GetComponent<RectTransform>(),
                    canvasEventData.position, canvasCam, out canvasPos))
                {
                    //Debug.Log("Mouse Position in Canvas Space: " + canvasPos);
                }
            }
            rectWidth = hoverObj.GetComponent<RectTransform>().rect.width;
            rectHeight = hoverObj.GetComponent<RectTransform>().rect.height;
            prevChipPos = HoverChangeChipPos(canvasPos);
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
            byte curChipPos = HoverChangeChipPos(canvasPos);
            //Debug.Log("squarbutton: curChipPos = " + curChipPos);
            if (prevChipPos != curChipPos)
            {
                Debug.Log("PrevChipPos =" + prevChipPos + ", curChipPos = " + curChipPos);
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
                    if (selObjIdx % 3 == 1 )
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
                    else if (sideObjIdx > 0 )
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

        if (isEnter)
            selObj.gameObject.GetComponent<Button>().OnPointerEnter(canvasEventData);
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
            selObj.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
        }
    }

    private byte HoverChangeChipPos(Vector2 canvasPos)
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
        if (hoverObj.name == "Squar_p12" )
        {
            foreach (Transform itemTransform in scr_InsideNumG_01.GetComponent<ScrollRect>().content)
            {
                //Debug.Log("Mouse Hovered  Exited GameObject = " + itemTransform.gameObject.name);
                EventSystem.current.SetSelectedGameObject(null);
                itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
            }
        }
        else if (hoverObj.name == "Squar_m12" )
        {
            foreach (Transform itemTransform in scr_InsideNumG_02.GetComponent<ScrollRect>().content)
            {
                EventSystem.current.SetSelectedGameObject(null);
                itemTransform.gameObject.GetComponent<Button>().OnPointerExit(canvasEventData);
            }
        }
        else if (hoverObj.name == "Squar_d12" )
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
        else if (hoverObj.name == "Round_FirstGroup")
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
            lstHoverObjectsInRound.Clear();
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
            lstHoverObjectsInRound.Clear();
            isRoundPanel = false;
        }
        else if (hoverObj.name == "Round_ThirdGroup")
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
            lstHoverObjectsInRound.Clear();
            isRoundPanel = false;
        }
        else if (hoverObj.name == "Round_ZeroGroup")
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
            lstHoverObjectsInRound.Clear();
            isRoundPanel = false;
        }
        else if (hoverObj.name.Substring(0, 5) == "Round")
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

    void Update()
    {
        if (isScreenSpaceMenu && numBoardPos != 1)//numBoardPos -1: RoundPanel state, 0: Square Panel state, 1: Spine View State
            RawImage.SetActive(false);

        if (beginBet)
            ViewRightDownButtons(true);
        else
            ViewRightDownButtons(false);
        if (winStat) // history index is initalized!
        {
            //beginBet = false;
            foreach (Chip chip in dicChipObjects.Values)
            {
                GameObject chipObj = GameObject.Find(chip.name);
                GameObject.Destroy(chipObj);
            }
            dicChipObjects.Clear();
            //historyIndex = 0;
            winStat = false;
        }

        if (isScreenSpaceMenu)
            realtimeMousePos = Input.mousePosition;
        if (isSquarButtonHover)
        {
            HoverSquarButtons();
        }

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
        numBoardPos = -1;
        pan_WholeBoard.GetComponent<Animator>().SetInteger("NumBoardPos", numBoardPos);
        btn_Neighbour.SetActive(false);
        btn_Rollet.SetActive(false);
        StartCoroutine(ViewDelayButtons(btn_Back));
    }

    public void GotoSquarePanel(GameObject thisButton)
    {
        numBoardPos = 0;
        pan_WholeBoard.GetComponent<Animator>().SetInteger("NumBoardPos", numBoardPos);
        thisButton.SetActive(false);
        StartCoroutine(ViewDelayButtons(btn_Neighbour));
        if (beginBet)
            StartCoroutine(ViewDelayButtons(btn_Rollet));
    }

    IEnumerator ViewDelayButtons(GameObject button)
    {
        yield return new WaitForSeconds(1);
        button.SetActive(true);
    }

    public void SpineAction()
    {
        if (!beginBet) return;
        if (isScreenSpaceMenu)
        {
            btn_Rollet.SetActive(false);
            ViewSpinPanel();
        }
        winStat = false;
        string json = JsonConvert.SerializeObject(dicChipObjects.Values);
        Debug.Log("Spin: json = " + json);
        SockIOManage.instance.sioCom.Instance.Emit("spine-start", json, false);

        beginBet = false;

        Spine.instance.SetMark(UnityEngine.Random.RandomRange(0, 36));//winNum

        lstLastestNum = new List<int> { 15, 22, 9, 32 };
        lstHotNum = new List<int> { 32, 31, 6 };
        arrayRedBlack = new int[] { 60, 30, 10 };
        arrayOddEven = new int[] { 30, 50, 20 };
        arrayDozen = new int[] { 60, 30, 10 };
        arrayColumn = new int[] { 50, 30, 20 };
        UpdateInfos();

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

        foreach (Chip chip in dicChipObjects.Values)
        {
            Debug.Log("Get data from saved data: Chip.name = " + chip.name);
            CreatChipsFromData(chip);
        }

    }

    public void OnLoadAClick(GameObject buttonObjIdx)
    {
        byte index = byte.Parse(buttonObjIdx.name.Substring(buttonObjIdx.name.Length - 1));
        //Debug.Log("index = " + index);
        LoadData(index); // beginBet is false at LoadData.
        beginBet = true;

    }

    private void CreatChipsFromData(Chip curChip)
    {
        Vector3 vChipPos = new Vector3(0f, 0f, 0f);
        GameObject chipInstant = GameObject.Instantiate(prefabChip, vChipPos, Quaternion.identity);
        chipInstant.name = curChip.name;
        string parentName = "Squar_" + curChip.buttonName;
        Debug.Log("parentName = " + parentName);
        GameObject buttonInSquare = GameObject.Find(parentName);
        chipInstant.transform.SetParent(buttonInSquare.transform, false);

        rectWidth = buttonInSquare.GetComponent<RectTransform>().rect.width;
        rectHeight = buttonInSquare.GetComponent<RectTransform>().rect.height;

        switch (curChip.positionInButton)
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
        ChangeChipSprite(chipInstant, curChip.chipValue);
        chipInstant.transform.GetChild(0).GetComponent<TMP_Text>().text = curChip.chipValue.ToString();

        //Debug.Log("current history value: " + historyIndex);
        historyIndex++;
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
        foreach (int item in lstLastestNum)
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
            img_SpinBG.SetActive(false);
            txtr_Render.SetActive(true);
            opt_SwitchBtton.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(-22f, -2.6f);

            btn_Rollet.SetActive(false);
            btn_Neighbour.SetActive(false);
            btn_Back.SetActive(false);
            btn_Back_2.SetActive(false);

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
            img_SpinBG.SetActive(true);
            txtr_Render.SetActive(false);
            opt_SwitchBtton.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(16.6f, -2.6f);

            btn_Neighbour.SetActive(true);
            if (beginBet)
                btn_Rollet.SetActive(true);

            pan_SquareControlGroup.transform.SetParent(pan_WholeBoard.transform, false);
            pan_SquareControlGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(-25f, 2.15f);
            pan_SquareControlGroup.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            pan_SquareControlGroup.transform.localScale = new Vector3(1f, 1f, 1f);

            pan_RoundControlGroup.transform.localScale = new Vector3(1f, 1f, 1f);
            pan_RoundControlGroup.transform.SetParent(pan_WholeBoard.transform, false);
            pan_RoundControlGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920f, 0f);

            pan_NeighbourCounterGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(50f, -280f);
            pan_NeighbourCounterGroup.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            numBoardPos = 0;
        }
    }

    private void ViewRightDownButtons(bool beginBet = false)
    {
        btn_Undo.SetActive(beginBet);
        btn_Clear.SetActive(beginBet);
        btn_Double.SetActive(beginBet);
    }

    public void ViewSpinPanel()
    {
        img_SpinBG.SetActive(true);
        txtr_Render.SetActive(true);
        btn_Rollet.SetActive(false);
        btn_Neighbour.SetActive(false);

        StartCoroutine(ViewDelayButtons(btn_Back_2));

        txtr_Render.GetComponent<RectTransform>().anchoredPosition = new Vector2(-250f, 10f);
        txtr_Render.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1.2f);
        numBoardPos = 1;
        pan_WholeBoard.GetComponent<Animator>().SetInteger("NumBoardPos", numBoardPos);

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
