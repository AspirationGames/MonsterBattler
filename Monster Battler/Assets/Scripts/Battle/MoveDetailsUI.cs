using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoveDetailsUI : MonoBehaviour
{
    [SerializeField] Image moveTypeImage;
    [SerializeField] Image attackCategoryImage;
    [SerializeField] TextMeshProUGUI powerText;
    [SerializeField] TextMeshProUGUI accuracyText;
    [SerializeField] TextMeshProUGUI apText;

    [SerializeField] List<Sprite> typeImages;
    [SerializeField] List<Sprite> attackCategoryImages;
    
    public void Awake()
    {

    }
    public void ShowMoveDetails(Move move)
    {
        SetTypeImage(move);
        SetCategoryImage(move);
        powerText.text = $"Power: {move.Base.Power}";
        accuracyText.text = $"Accuracy: {move.Base.Accuracy}";
        apText.text = $"AP: {move.AP}/{move.Base.AP}";
    }

    void SetTypeImage(Move move)
    {
        var moveType = move.Base.Type;
        int typeIndex = (int)moveType; //this converts the move type to a index based on the Monster Type Enum

        moveTypeImage.sprite = typeImages[typeIndex];
        
    }

    void SetCategoryImage(Move move)
    {
        var attackType = move.Base.Category;
        int categoryIndex = (int)attackType;

        attackCategoryImage.sprite = attackCategoryImages[categoryIndex];
    }
}
