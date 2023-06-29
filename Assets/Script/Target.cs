namespace CED_Roulette
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Target : MonoBehaviour
    {
        [HideInInspector]
        public bool isCollider = false;
        public int index;
        // Start is called before the first frame update
        void Start()
        {
            isCollider = false;
        }

        private void OnEnable()
        {
            isCollider = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (index == Spine.instance.winPoint)
            {
                isCollider = true;
                //Debug.Log("Target Collided! Target Name = " + this.gameObject.name);
            }
            else
            {
                Vector3 forceDirection = (Spine.instance.TargetObj.transform.position - other.transform.position).normalized * 6.4f;
                other.GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);
                //Debug.Log("wrong Target Collided! Target Name = " + this.gameObject.name);
            }
        }

    }

}
