using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : Singleton<UI> {

    public GameObject ExitButton;
    public Text scoreText;
    public GameObject WonPanel;
    public GameObject LostPanel;
    public GameObject GameCompletePanel;
    public Image VolumeButtonToggleImage;

    public Sprite VolumeOn;
    public Sprite VolumeMuted;

    void Start () {
        ExitButton.SetActive(false);
    }
	
	void Update () {
        //Toggle exit button
        if (Input.GetKeyDown(KeyCode.Escape)) ExitButton.SetActive(!ExitButton.activeSelf);
    }

    public void OnVolumeButtonPressed()
    {
        GameManager.Instance.SoundsMuted = !GameManager.Instance.SoundsMuted;
        if (GameManager.Instance.SoundsMuted)
        {
            VolumeButtonToggleImage.sprite = VolumeMuted;
        } else
        {
            VolumeButtonToggleImage.sprite = VolumeOn;
        }
    }

    public static void UpdateScore(int score, int outOf)
    {
        Instance.scoreText.text = score + "/" + outOf;
    }

    public static void WonPanelVisiblility(bool visible)
    {
        SetGameObjectActive(Instance.WonPanel, visible);
    }

    public static void LostPanelVisiblility(bool visible)
    {
        SetGameObjectActive(Instance.LostPanel, visible);
    }

    public static void ShowGameComplete()
    {
        SetGameObjectActive(Instance.GameCompletePanel, true);
    }

    static void SetGameObjectActive(GameObject go, bool active)
    {
        go.SetActive(active);
    }
}
