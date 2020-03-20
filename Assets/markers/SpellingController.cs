using UnityEngine;
using System.Collections.Generic;
using System.Linq; 
using Wikitude;
using UnityEngine.UI;

[System.Serializable]  
public class wordList{
    public string word;
    public Texture prompt; 
}

public class SpellingController : MonoBehaviour
{
    public GameObject trackable;
    public GameObject tick;
    public GameObject cross;

    bool gotReward = false;
    public string word = "car";

    public List<wordList> words; 
    public Texture gameOver;
    public RawImage screenPrompt;
    public Text score;

    int currentWord = -1;

    void Start()
    {
        PickRandomWord();
    }

    public void PickRandomWord()
    {
        if (currentWord >= 0)
        {
            words.RemoveAt(currentWord);
        }
        currentWord = Random.Range(0, words.Count);
        word = words[currentWord].word;
        screenPrompt.texture = words[currentWord].prompt;  
    }

    public void OnImgRec(ImageTarget target)
    {
        Invoke("CheckSpelling", 0.1f);
    }

    void CheckSpelling()
    {
        Transform[] allTransforms = trackable.GetComponentsInChildren<Transform>();
              
        List<Transform> markers = new List<Transform>();

        foreach(Transform t in allTransforms)   
        {
            if (t.parent.gameObject == trackable)
                markers.Add(t);
        }

        if (markers.Count != word.Length)
        {
            gotReward = false;
            return;
        }

        markers = markers.OrderByDescending(marker => marker.position.x).ToList();
        int countCorrect = 0;
        
        for(int i=0; i<markers.Count; i++)
        {
            if(markers[i].gameObject.name.StartsWith(word[i] + "_"))
            {
                countCorrect++;
                GameObject tickObj = Instantiate(tick, markers[i].position, markers[i].rotation);
                tickObj.transform.parent = markers[i];
            } 
            else
            {
                GameObject crossObj = Instantiate(cross, markers[i].position, markers[i].rotation);
                crossObj.transform.parent = markers[i];
            }
        }

        if(countCorrect == word.Length && !gotReward)
        {
            gotReward = true;
            score.text = (int.Parse(score.text)+1) + "";
            if (gotReward)
            {
                if (words.Count == 1)
                {
                    screenPrompt.texture = gameOver;
                    trackable.transform.parent.gameObject.SetActive(false);
                }else
                {
                    PickRandomWord();
                    gotReward = false;
                }
            }
        }
        else
        {
            Debug.Log("wrong");
        }
    }
}