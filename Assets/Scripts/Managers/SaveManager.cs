using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string KEY_WIN = "win";
    private const string KEY_LOSE = "lose";
    private const string KEY_GEMS = "gems";
    private const string KEY_KILLS = "kills";
    
    private int _win;
    private int _lose;
    private int _gems;
    private int _kills;
    
    public int GetWins() => _win;
    public int GetLoses() => _lose;
    public int GetGems() => _gems;

    public static SaveManager Instance;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        _win = PlayerPrefs.GetInt(KEY_WIN);
        _lose = PlayerPrefs.GetInt(KEY_LOSE);
        _gems = PlayerPrefs.GetInt(KEY_GEMS);
        _kills = PlayerPrefs.GetInt(KEY_KILLS);
    }

    public void AddWin()
    {
        _win++;
        PlayerPrefs.SetInt(KEY_WIN, _win);
    }
    
    public void AddLose()
    {
        _lose++;
        PlayerPrefs.SetInt(KEY_LOSE, _lose);
    }

    public void AddGems(int value)
    {
        _gems += value;
        PlayerPrefs.SetInt(KEY_GEMS, _gems);
    }

    public void RemoveGems(int value)
    {
        _gems -= value;
        PlayerPrefs.SetInt(KEY_GEMS, _gems);
    }

    public void AddKill()
    {
        _kills++;
        PlayerPrefs.SetInt(KEY_KILLS, _kills);
    }
    
    public void AddUpgrade(string upgradeName)
    {
        PlayerPrefs.SetInt(upgradeName, 1);
    }

    public bool HasUpgrade(string upgradeName)
    {
        return PlayerPrefs.GetInt(upgradeName) != 0;
    }
}
