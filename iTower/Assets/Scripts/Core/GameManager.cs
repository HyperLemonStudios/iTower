﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
//using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour{   public static GameManager instance;
    public static bool GlobalTimeIsPaused;
    public static bool GlobalTimeIsPausedNotSlowed;
    [Header("Global")]
    public bool smthOn=true;
    [Header("Current Player Values")]
    public int score = 0;
    [Header("Settings")]
    [Range(0.0f, 10.0f)] public float gameSpeed=1f;
    public float defaultGameSpeed=1f;
    public bool speedChanged;
    [Header("Other")]
    public string gameVersion;
	public float buildVersion;
    public bool cheatmode;
    public bool dmgPopups=true;
    public bool analyticsOn=true;
    [SerializeField]float restartTimer=-4;
    
    Player player;
    PostProcessVolume postProcessVolume;
    bool setValues;
    public float gameSessionTime=0;
    //[SerializeField] InputMaster inputMaster;
    [Range(0,2)]public static int maskMode=1;
    //public string gameVersion;

    void Awake(){
        SetUpSingleton();
        instance=this;
        #if UNITY_EDITOR
        cheatmode=true;
        #else
        cheatmode=false;
        #endif
        gameObject.AddComponent<gitignoreScript>();
    }
    void SetUpSingleton(){int numberOfObj=FindObjectsOfType<GameManager>().Length;if(numberOfObj>1){Destroy(gameObject);}else{DontDestroyOnLoad(gameObject);}}
    void Start(){}
    void Update(){
        if(gameSpeed>=0){Time.timeScale=gameSpeed;}if(gameSpeed<0){gameSpeed=0;}

        if(SceneManager.GetActiveScene().name=="Game"&&FindObjectOfType<Player>()!=null&&gameSpeed>0.0001f){gameSessionTime+=Time.unscaledDeltaTime;}
        //Set speed to normal
        if(PauseMenu.GameIsPaused==false&&
        (FindObjectOfType<Player>()!=null)&&speedChanged!=true){gameSpeed=defaultGameSpeed;}
        if(SceneManager.GetActiveScene().name!="Game"){gameSpeed=1;}
        if(FindObjectOfType<Player>()==null){gameSpeed=defaultGameSpeed;}
        
        //Restart with R or Space/Resume with Space
        if(SceneManager.GetActiveScene().name=="Game"){
        if(PauseMenu.GameIsPaused==true){if(restartTimer==-4)restartTimer=0.5f;}
        if(restartTimer>0)restartTimer-=Time.unscaledDeltaTime;
        }

        if(PauseMenu.GameIsPaused==true){
            foreach(AudioSource sound in FindObjectsOfType<AudioSource>()){
                if(sound!=null){
                    GameObject snd=sound.gameObject;
                    //if(sound!=musicPlayer){
                    if(snd.GetComponent<Jukebox>()==null){
                        //sound.pitch=1;
                        sound.Stop();
                    }
                }
            }
        }

        //Postprocessing
        postProcessVolume=FindObjectOfType<PostProcessVolume>();
        if(SaveSerial.instance!=null){
        if(SaveSerial.instance.settingsData.pprocessing==true && postProcessVolume!=null){postProcessVolume.GetComponent<PostProcessVolume>().enabled=true;}
        if(SaveSerial.instance.settingsData.pprocessing==false && FindObjectOfType<PostProcessVolume>()!=null){postProcessVolume=FindObjectOfType<PostProcessVolume>();postProcessVolume.GetComponent<PostProcessVolume>().enabled=false;}
        }


        CheckCodes("0","0");
    }
    public void CheckCodes(string fkey, string nkey){gitignoreScript.instance.CheckCodes(fkey,nkey);}
    public void SaveSettings(){SaveSerial.instance.SaveSettings();}
    public void Save(){ SaveSerial.instance.Save(); SaveSerial.instance.SaveSettings(); }
    public void Load(){ SaveSerial.instance.Load(); SaveSerial.instance.LoadSettings(); }
    public void DeleteAll(){ SaveSerial.instance.Delete(); ResetSettings(); GSceneManager.instance.LoadStartMenu();}
    public void ResetSettings(){
        SaveSerial.instance.ResetSettings();
        GSceneManager.instance.ReloadScene();
        SaveSerial.instance.SaveSettings();
    }
    public void ResetMusicPitch(){if(Jukebox.instance!=null)Jukebox.instance.GetComponent<AudioSource>().pitch=1;}
    float settingsOpenTimer;
    public void CloseSettings(bool goToPause){
    if(GameManager.instance!=null){
        if(SceneManager.GetActiveScene().name=="Options"){GSceneManager.instance.LoadStartMenu();}
        else if(SceneManager.GetActiveScene().name=="Game"&&PauseMenu.GameIsPaused){if(FindObjectOfType<SettingsMenu>()!=null)FindObjectOfType<SettingsMenu>().Close();if(FindObjectOfType<PauseMenu>()!=null&&goToPause)FindObjectOfType<PauseMenu>().Pause();}
    }}
    public string FormatTime(float time){
        int minutes = (int) time / 60 ;
        int seconds = (int) time - 60 * minutes;
        //int milliseconds = (int) (1000 * (time - minutes * 60 - seconds));
    return string.Format("{0:00}:{1:00}"/*:{2:000}"*/, minutes, seconds/*, milliseconds*/ );
    }
    public string GetGameManagerTimeFormat(){
        return FormatTime(gameSessionTime);
    }public int GetGameManagerTime(){
        return Mathf.RoundToInt(gameSessionTime);
    }
    public void SetCheatmode(){if(!cheatmode){cheatmode=true;return;}else{cheatmode=false;return;}}
}
public enum dir{up,down,left,right}