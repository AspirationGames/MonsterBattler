using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerController : MonoBehaviour, Interactable
{
    [SerializeField] string summonerName;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;

    [SerializeField] Dialog postBattleDialog;
    [SerializeField] GameObject exclamation;

    [SerializeField] GameObject fov;

    Character character;

    //State
    bool battleLost = false;

    private void Awake() 
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.CharacterAnimator.DefaultDirection);
    }

    public void Interact(Transform initiator)
    {
        character.LookTowards(initiator.position);

        if(!battleLost)
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => 
            {
            GameController.Instance.StartSummonerBattle(this);
            }));
        }
        else
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(postBattleDialog));
        }

        
    }

    public IEnumerator TriggerMageBattle(PlayerController player)
    {
        //Exclamation Mark
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        //Moves to player
        var diff = player.transform.position - transform.position; //difference vector
        var moveVector = diff - diff.normalized;

        yield return character.Move(moveVector);

        //Dialog and Start Battle
        StartCoroutine( DialogManager.Instance.ShowDialog(dialog, () => 
        { 
            //Start Battle
            GameController.Instance.StartSummonerBattle(this);
             
        } ) );

        
        
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;

        // we don't need to doe a facing direction down because that is the default

        if(dir == FacingDirection.Up)
        {
            angle = 180f;
        }
        else if(dir == FacingDirection.Right)
        {
            angle = 90;
        }
        else if(dir == FacingDirection.Left)
        {
            angle = 270;
        }

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle); //euler angles can rotate taking a vector 3
    }

    public void BattleLost()
    {
        fov.SetActive(false);
        battleLost = true;
    }

    public string Name
    {
        get => summonerName;
    }

    public Sprite Sprite
    {
        get => sprite;
    }
}