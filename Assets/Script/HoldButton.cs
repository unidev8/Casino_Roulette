namespace CED_Roulette
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using UnityEngine.UI;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public byte idx = 0;
        public Sprite spriteSaved;
        public Sprite spriteNoSaved;
        private float holdTime = 1.2f; // The time in seconds the button must be held down for
        private bool isHeldDown = false; // Whether the button is currently being held down


        // Start is called before the first frame update
        void Start()
        {
            if (PlayerPrefs.GetString("btnSave_" + idx.ToString()) == "Saved")
            {
                this.gameObject.GetComponent<Image>().sprite = spriteSaved;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                StartCoroutine(HoldCoroutine());
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                StopAllCoroutines();
                isHeldDown = false;
            }
        }

        private IEnumerator HoldCoroutine()
        {
            isHeldDown = true;
            yield return new WaitForSeconds(holdTime);

            if (isHeldDown) // If the button is still being held down after the wait time
            {
                Debug.Log("Button held down for " + holdTime + " seconds");
                SaveData(idx);
            }
        }

        private void SaveData(byte index)
        {   /*   
        foreach (string key in OthoMenu.instance.dicChipObjects.Keys)
        {
            Chip chipObject = OthoMenu.instance.dicChipObjects[key];
            //string chipJson = JsonUtility.ToJson(chipObject);
            PlayerPrefs.SetString(key, chipObject.chipValue.ToString());
            Debug.Log("Key = " + key + ", ChipValue = " + chipObject.chipValue.ToString());
        }
        PlayerPrefs.Save();
*/
            Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/CED_data_" + index + ".dat");
            if (OthoMenu.instance.dicChipObjects.Count > 0)
            {
                this.gameObject.GetComponent<Image>().sprite = spriteSaved;
                PlayerPrefs.SetString("btnSave_" + index.ToString(), "Saved");
            }
            else
            {
                this.gameObject.GetComponent<Image>().sprite = spriteNoSaved;
                PlayerPrefs.SetString("btnSave_" + index.ToString(), "UnSaved");
            }

            bf.Serialize(file, OthoMenu.instance.dicChipObjects);
            file.Close();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
