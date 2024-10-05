using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VisualHealth : MonoBehaviour
{

    public Sprite filledHeart, unfilledHeart;
    Image heartImage;

    private void Awake() 
    {

        heartImage = GetComponent<Image>();
        
    }

    public void SetImage(HeartStats stats)
    {

        switch (stats)
        {

            case HeartStats.Empty:
                heartImage.sprite = unfilledHeart;
                break;
            case HeartStats.Full:
                heartImage.sprite = filledHeart;
                break;

        }
        
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum HeartStats
{

    Empty = 0,
    Full = 1,

}
