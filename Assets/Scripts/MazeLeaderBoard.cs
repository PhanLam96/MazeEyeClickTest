using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MazeLeaderBoard : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> listText;
    public List<int> listScore;

    private void Awake()
    {
        listScore = new List<int>();
    }
    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            var highScore = PlayerPrefs.GetInt("HighScore" +  i);
            if (highScore != 0)
            {
                listScore.Add(highScore);
            }
        }
        for (int i = 0; i < listScore.Count; i++)
        {
            listText[i].text = (i + 1).ToString() + ". " + listScore[i].ToString();
        }
    }

    public void SubmitScore(int step)
    {            
        listScore.Add(step);
        listScore.Sort((a, b) => a.CompareTo(b));
        if (listScore.Count > 5)
        {
            listScore.RemoveAt(5);
        }
        for (int i = 0; i < listScore.Count; i++)
        {
            PlayerPrefs.SetInt("HighScore" + i, listScore[i]);
            listText[i].text = (i + 1).ToString() + ". " + listScore[i].ToString();
            listText[i].gameObject.SetActive(true);
        }
    }
}

