using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] GameObject evolutionScreen;
    [SerializeField] Image monsterImage;

    public static EvolutionManager i;

    public event Action OnEvolutionStart;
    public event Action OnEvolutionEnd;

    private void Awake() 
    {
          i = this;  
    }

    public IEnumerator Evolve(Monster monster, Evolution evolution)
    {
        OnEvolutionStart?.Invoke();
        evolutionScreen.SetActive(true);
        monsterImage.sprite = monster.Base.FrontSprite;

        
        yield return DialogManager.Instance.ShowDialogText($"{monster.Base.MonsterName} is evolving!", true);
        var oldMonster = monster.Base;
        monster.Evolve(evolution);

        monsterImage.sprite = monster.Base.FrontSprite;
        yield return DialogManager.Instance.ShowDialogText($"{oldMonster.MonsterName} evolved into {monster.Base.MonsterName}!", true);

        evolutionScreen.SetActive(false);

        OnEvolutionEnd?.Invoke();
    }
}
