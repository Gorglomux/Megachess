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
    public GameObject unitImage;
    public Transform gridTransform;
    public List<GameObject> unitImages;
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
}
