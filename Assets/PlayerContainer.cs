using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerContainer : MonoBehaviour
{
    public PlayerData playerData;
    public TextMeshProUGUI text;
    public Image playerDataImage;
    public Transform lockedTransform;
    public TextMeshProUGUI winText;
    public GameObject unitImage;
    public Transform gridTransform;
    public List<GameObject> unitImages;


    public Sprite winSprite;
    public Image winImage1;
    public Image winImage2;



    // Start is called before the first frame update
    void Start()
    {

    }

    public void FillPlayer(PlayerData playerData)
    {
        this.playerData = playerData;
        playerDataImage.sprite = playerData.sprite;
        text.text = playerData.playerName;


    }

    public void AddUnits(List<UnitData> unitDatas)
    {
        foreach(UnitData unitData in unitDatas)
        {
            GameObject go = GameObject.Instantiate(unitImage,gridTransform);
            go.GetComponent<Image>().sprite = unitData.sprite;
            unitImages.Add(go);
        }
    }
    public void setLocked()
    {
        lockedTransform.gameObject.SetActive(true);
    }
    public void setWinCount(int count)
    {
        winText.text = string.Format("{0} {1}", count,GlobalHelper.PluralOrSingular("Win","Wins",count) );

        winImage1.sprite = winSprite;
        winImage2.sprite = winSprite;

    }
}
