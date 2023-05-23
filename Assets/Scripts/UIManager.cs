using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Accessibility;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtScore;
    [SerializeField] private TextMeshProUGUI txtTime;
    [SerializeField] private TextMeshProUGUI txtNotice;
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelInit;

    public int step;
    public float elapseTime;

    private void Start()
    {
        SetNotice("Welcome to Maze Runner <br> Press WASD to move", 0, 5);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            panelMenu.SetActive(!panelMenu.activeSelf);
        }

        if (GameManager.Instance.isGameReady)
        {
            elapseTime += Time.deltaTime;
            txtTime.text = "Time: " + elapseTime.ToString("0.0");
        }

    }

    public void AddStep()
    {
        step++;
        txtScore.text = $"Step: {step}";
    }
    public void OnStartGame()
    {
        panelInit.SetActive(false);
    }
    public void OnEndGame()
    {
        SetNotice($"Congratulation!<br>You solved all Maze with {step} step and {elapseTime.ToString("0.0")} second", 0, 5);
        GameManager.Instance.mazeLeaderBoard.SubmitScore(step);
        StartCoroutine(Utils.IEDelayCall(() => panelMenu.gameObject.SetActive(true), 5));
    }
    public void SetNotice(string content, float delayShow = 0, float delayHide = 0)
    {
        StartCoroutine(ShowNotice(content, delayShow, delayHide));
    }

    IEnumerator ShowNotice(string content, float delayShow = 0, float delayHide = 0)
    {
        yield return new WaitForSeconds(delayShow);
        txtNotice.text = content;
        txtNotice.gameObject.SetActive(true);
        yield return new WaitForSeconds(delayHide);
        txtNotice.gameObject.SetActive(false);
    }
    public void HideNotice() => txtNotice.gameObject.SetActive(false);
}
