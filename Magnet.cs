using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{   

    [HideInInspector]
    public bool isCollider = false;
    [HideInInspector]
    public float initDist;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        isCollider = true;
        initDist = (transform.position - other.transform.position).magnitude;
        Debug.Log("Magnet Collided!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
