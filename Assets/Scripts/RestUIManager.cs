using System;
using TMPro;
using UnityEngine;

public class RestUIManager : MonoBehaviour
{
    public GameObject Menu;

    public GameObject LevelsMenu;

    Player Player => ServiceLocator.GetFirst<Player>();

    void Awake()
    {
        ServiceLocator.Register(this);    

        gameObject.SetActive(false);
    }

    [Serializable]
    private struct LevelUpData
    {
        public string Name;
        public int LevelUpCount;
        public TMP_Text Text;

        public void SetValue(int value, LevelScaledStat stat)
        {
            if(value != 0 && value + stat.CurrentLevel() == stat.MaxLevel)
            {
                return;
            }

            LevelUpCount = value;

            Text.text = $"{Name} {LevelUpCount + stat.CurrentLevel()}";
        }

        public void LevelUp(LevelScaledStat stat)
        {
            SetValue(LevelUpCount + 1, stat);
        }

        public void LevelDown(LevelScaledStat stat)
        {
            SetValue(LevelUpCount - 1, stat);
        }
    }

    [SerializeField] LevelUpData Vigor, Strength;

    private int LevelUps => Vigor.LevelUpCount + Strength.LevelUpCount;

    public TMP_Text Cost, Left;

    private void UpdateCostLeft()
    {
        Cost.text = $"Cost {LevelUps}";
        Left.text = $"Left  {Player.UnspentSkillPoints - LevelUps}";
    }

    public void OnLevelsPressed()
    {
        Menu.SetActive(false);
        LevelsMenu.SetActive(true);

        ResetLevelsTab();
    }

    public void OnLevelsCancelPressed()
    {
        Menu.SetActive(true);
        LevelsMenu.SetActive(false);
    }

    private void ApplyLevels(int count, LevelScaledStat stat)
    {
        for(int i = 0; i < count; ++i)
        {
            stat.LevelUp();
        }
    }

    public void OnVigorLevelUp()
    {
        if(Player.UnspentSkillPoints == LevelUps)
        {
            return;
        }

        Vigor.LevelUp(Player.Vigor);
        UpdateCostLeft();
    }
    public void OnStrengthLevelUp()
    {
        if(Player.UnspentSkillPoints == LevelUps)
        {
            return;
        }

        Strength.LevelUp(Player.Strength);
        UpdateCostLeft();
    }

    public void OnVigorLevelDown()
    {
        if(Vigor.LevelUpCount == 0)
        {
            return;
        }

        Vigor.LevelDown(Player.Vigor);
        UpdateCostLeft();
    }
    public void OnStrengthLevelDown()
    {
        if(Strength.LevelUpCount == 0)
        {
            return;
        }

        Strength.LevelDown(Player.Strength);
        UpdateCostLeft();
    }

    public void OnLevelUpConfirm()
    {
        ApplyLevels(Vigor.LevelUpCount, Player.Vigor);
        ApplyLevels(Strength.LevelUpCount, Player.Strength);

        Player.UnspentSkillPoints -= LevelUps;

        ResetLevelsTab();
    }

    public void OnRestPressed()
    {

        ServiceLocator.GetFirst<RoomTransitionManager>().ReloadCurrent();        
        Player.Rest();
    }

    private void ResetLevelsTab()
    {
        Vigor.SetValue(0, Player.Vigor);

        Strength.SetValue(0, Player.Strength);

        UpdateCostLeft();
    }
}
