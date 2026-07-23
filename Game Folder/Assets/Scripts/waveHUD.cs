using UnityEngine;
using TMPro;

public class waveHUD : MonoBehaviour
{
    [SerializeField] TMP_Text waveText;
    [SerializeField] TMP_Text waveMessageText;

    void Awake()
    {
        hideWaveMessage();
    }

    public void startWave(int currentWave, int totalWaves)
    {
        if (waveText == null)
        {
            return;
        }

        if (currentWave == totalWaves)
        {
            waveText.text =
                "Final Wave (" +
                currentWave + " / " +
                totalWaves + ")";
        }
        else
        {
            waveText.text =
                "Wave " +
                currentWave + " / " +
                totalWaves;
        }

        hideWaveMessage();
    }

    public void showWaveComplete(int currentWave)
    {
        if (waveMessageText == null)
        {
            return;
        }

        waveMessageText.gameObject.SetActive(true);
        waveMessageText.text =
            "Wave " + currentWave + " Complete!";
    }

    public void showNextWave(int seconds)
    {
        if (waveMessageText == null)
        {
            return;
        }

        waveMessageText.gameObject.SetActive(true);
        waveMessageText.text =
            "Next Wave In: " + seconds;
    }

    public void hideWaveMessage()
    {
        if (waveMessageText != null)
        {
            waveMessageText.gameObject.SetActive(false);
        }
    }
}