#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private TextMeshProUGUI bestScoreText;

    private void Start()
    {
        //DataManager.Instance.ClearData();
        Load();
    }
    
    void SaveUserData()
    {
        var data = DataManager.Instance.GetUserHasExist(userNameInput.text);
        if(DataManager.Instance.isUserHasExist)
        {
            DataManager.Instance.SaveUserData(data.userName, data.bestScore);
            return;
        }
        
        if(userNameInput.text != DataManager.Instance.userName)
            DataManager.Instance.SaveUserData(userNameInput.text, 0);
        else
            DataManager.Instance.SaveUserData(userNameInput.text, DataManager.Instance.bestScore);
    }
    
    #region Button Method

    public void Load()
    {
        DataManager.Instance.LoadData();
        DataManager.Instance.GetBestScore();
        bestScoreText.text = $"Best Score: {DataManager.Instance.bestScore}";
        userNameInput.text = DataManager.Instance.userName;
    }

    // public void SaveButton()
    // {
    //     DataManager.Instance.SaveUserName();
    // }
    
    public void GetUserNameInput(string input)
    {
        userNameInput.text = input;
    }
    
    public void StartButton()
    {
        SaveUserData();
        SceneManager.LoadScene("main");
    }

    public void ExitButton()
    {
        SaveUserData();
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();       
#endif
    }

    #endregion
}
