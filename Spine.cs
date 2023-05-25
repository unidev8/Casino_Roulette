using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class Spine : MonoBehaviour
{
    public GameObject Ball;
    public GameObject CenterObj;
    public GameObject MagnetObj;    
    public PhysicMaterial ballPhyicMat;
    public PhysicMaterial wheelPhyicMat;
    [HideInInspector]
    public int MarkNum;
    [HideInInspector]
    public  GameObject TargetObj;
    private Vector3 centerPoint;// = new Vector3(-475f, 110.22f, -0.90f);
    private Vector3 initBallPosition;
    private Rigidbody rigidBall;
    private bool isMagnetCollided = false;
    private float radius; // roulette wheel radius
    private float angle = 0.0f; // current angle on the roulette wheel

    private float speed = 3.2f; // ball speed
    private float endSpeed = 1f;
    private float decreaseSpeed = 0.64f;    

    private float magnetForce = 24f; // the magnet force to apply    
    private float Gravity = -36f;
    private bool cwSpine = true;
    private bool startBallSpine = false;  // flage of user starting spine
    private bool wheelStart = false;

    [HideInInspector]
    public int winPoint;
    public Sprite[] sprites;
    public Texture2D spriteSheet;
    public GameObject zoomGoal;

    public static Spine instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        centerPoint = CenterObj.transform.position;
        initBallPosition = Ball.transform.position;

        rigidBall = Ball.GetComponent<Rigidbody>();

        Reset();

        /*
        int spriteSize = 241; // set the size of each sprite in pixels
        int numSprites = 40; // set the number of sprites in the sprite sheet
        sprites = new Sprite[numSprites];

        for (int i = 0; i < numSprites; i++)
        {
            int x = i % (spriteSheet.width / spriteSize);
            int y = i / (spriteSheet.width / spriteSize);
            Rect rect = new Rect(x * spriteSize, y * spriteSize, spriteSize, spriteSize);
            sprites[i] = Sprite.Create(spriteSheet, rect, Vector2.zero);
        }*/

    }

    public void SetMark(int mark)
    {
        startBallSpine = true; //after setMark it is true, after correct trigger it is false
        wheelStart = true; // for onload. it is always true after first spine

        for (int i = 0; i < transform.childCount; i ++)
        {
            if (transform.GetChild(i).name.Substring(0, 3) != "tri") continue ;
            string markObjName = "";
            int markIdx = int.Parse(transform.GetChild(i).name.Substring(transform.GetChild(i).name.Length - 2));
            //Debug.Log("all markObjIdx = " + markIdx);
            markObjName = "trigger_" + markIdx.ToString("D2");
            GameObject TargetObj_Temp = GameObject.Find(markObjName);

            if (markIdx == mark)
            {                 
                TargetObj = TargetObj_Temp;
                winPoint = mark;
                MagnetObj.transform.position = TargetObj.transform.position;
                Reset();
                Debug.Log("mark = " + mark + ", markObjName = " + markObjName);               
            }
            else
            {
                //TargetObj_Temp.SetActive(false);
            }
        }        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!wheelStart) return;

        // Spine revolve;
        if (cwSpine)
        {
            gameObject.transform.Rotate(0, 0, Time.deltaTime * 40f);            
        }
        else
        {
            gameObject.transform.Rotate(0, 0, -Time.deltaTime * 40f);
        }
    }

    private void FixedUpdate ()
    {
        if (!wheelStart) return;
        // Ball moving
        if (!startBallSpine) return;

        isMagnetCollided = MagnetObj.GetComponent<Magnet>().isCollider;

        if (!isMagnetCollided)
        {
            if (Mathf.Abs(angle) > Mathf.PI * 2f)
            {
                if (Mathf.Abs(speed - endSpeed) > 0.1f)
                {
                    speed = Mathf.Lerp(speed, endSpeed, Time.deltaTime * decreaseSpeed);
                }
                else
                {
                    MagnetObj.SetActive(true);
                }
            }
            // Ball revolve;
            if (cwSpine)
            {
                angle += speed * Time.deltaTime;
            }
            else
            {
                angle -= speed * Time.deltaTime;
            }
            // calculate ball position based on the angle
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            Ball.transform.position = centerPoint + new Vector3(x, 0f, z);

        }
        else if (isMagnetCollided)
        {
            float distance = 0f;
            Vector3 direction = new Vector3(0f, 0f, 0f);
            //Debug.Log("isCollided withd Magenet!");
            rigidBall.isKinematic = false;
            rigidBall.useGravity = true;
            //Debug.Log("curGravity = " + curGravity);
            Physics.gravity = new Vector3(0.0f, Gravity, 0.0f);

            direction = MagnetObj.transform.position - rigidBall.position;
            //distance = direction.magnitude;
            float forceMagnitude = magnetForce;// * (distance / MagnetObj.GetComponent<Magnet>().initDist);
            rigidBall.AddForce(direction.normalized * forceMagnitude);
            //Debug.Log("distance = " + distance + ",  direction = " + direction.normalized + ", forcedMagnitude " + forceMagnitude);

            if (TargetObj.GetComponent<Target>().isCollider)
            {
                Debug.Log("Target Here!");
                if ((rigidBall.transform.position - TargetObj.transform.position).magnitude < 1)
                {
                    Debug.Log("Ball in Target Correctly!");
                    Ball.transform.SetParent(TargetObj.transform);
                    rigidBall.AddForce(new Vector3(0f, 0f, 0f));
                    rigidBall.velocity = Vector3.zero;
                    startBallSpine = false;
                    //Physics.gravity = new Vector3(0.0f, -80f, 0.0f);
                    //rigidBall.isKinematic = true;
                    OthoMenu.instance.winStat = true;
                    zoomGoal.SetActive(true);
                    zoomGoal.GetComponent <Image>().sprite = sprites[winPoint];
                    StartCoroutine ( ZoomGoalDeactive());

                    return;
                }
            }
        }
    }

    IEnumerator ZoomGoalDeactive()
    {
        yield return new WaitForSeconds(5f);
        zoomGoal.SetActive(false);
    }

    public void Reset()
    {        
        rigidBall.isKinematic = true;
        rigidBall.useGravity = false;
        Ball.transform.SetParent(null);
        Ball.transform.position = initBallPosition;        
        radius = (Ball.transform.position - centerPoint).magnitude;
        MagnetObj.SetActive(false);

        if (TargetObj) //for  Start()
            TargetObj.GetComponent<Target>().isCollider = false;
        if (MagnetObj)
            MagnetObj.GetComponent<Magnet>().isCollider = false;

        angle = 0f;
        speed = 2.0f;

        cwSpine = !cwSpine;
        
        //ballPhyicMat.bounciness = 0.1f;
        //ballPhyicMat.dynamicFriction = 0.1f;
        //wheelPhyicMat.staticFriction = 0.1f;
        //wheelPhyicMat.bounciness = 0.1f;
        //wheelPhyicMat.dynamicFriction = 0.1f;
        //wheelPhyicMat.staticFriction = 0.1f;
    }

}
