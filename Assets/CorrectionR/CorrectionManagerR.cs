using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CorrectionManagerR : MonoBehaviour
{
    [SerializeField] private int PeopleToHelp;
    private int peopleHelped = 0;
    
    [SerializeField] private float timer;
    private float elapsed = 0;
    private int strikes = 0;
    
    [Header("NPC SETUP")]
    [SerializeField] private CorrectionRegionsR[] tasks;
    public GameObject cabinet;
    [SerializeField] private CorrectionNPCR NPCPrefab;
    [SerializeField] private int maxNPCAmount = 3;
    [SerializeField] private float NPCSpawnRate = 5f;
    private float elapsedSpawnRate = 0;
    [NonSerialized] public int NPCAmountActive = 0;

    [Header("SETUP")] 
    [SerializeField] private TMP_Text goalText;
    [SerializeField] private Sprite redStrike;
    [SerializeField] private Image[] strikeRenderer;
    [SerializeField] private GameObject WinView, LoseView;

    private void Awake()
    {
        elapsed = timer;
        goalText.text = $"JOBS DONE: {peopleHelped}/{PeopleToHelp}";
    }

    private void OnEnable()
    {
        CorrectionNPCR.OnDone += AddDone;
        CorrectionNPCR.OnDeath += AddStrike;
    }

    private void OnDisable()
    {
        CorrectionNPCR.OnDone -= AddDone;
        CorrectionNPCR.OnDeath -= AddStrike;
    }
    
    private void Update()
    {
        // CountDown();
        HandleNPCSPawning();
    }

    private void HandleNPCSPawning()
    {
        if (NPCAmountActive >= maxNPCAmount) return;
        if (elapsedSpawnRate <= 0)
        {
            elapsedSpawnRate = NPCSpawnRate;
            NPCAmountActive++;
            
            CorrectionNPCR newNPC = Instantiate(NPCPrefab, transform.position, Quaternion.identity);
            newNPC.SetUp(this, tasks);
        }
        else
        {
            elapsedSpawnRate -= Time.deltaTime;
        }
    }

    private void CountDown()
    {
        if (elapsed <= 0)
        {
            goalText.text = "GAME OVER";
            enabled = false;
            WinView.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            elapsed -= Time.deltaTime;
            int elapsedInt = (int)elapsed;
            goalText.text = $"TIME: {elapsedInt.ToString()}" ;
        }
    }

    private void AddDone()
    {
        peopleHelped++;
        goalText.text = $"JOBS DONE: {peopleHelped}/{PeopleToHelp}";
        if (peopleHelped < PeopleToHelp) return;
        WinView.SetActive(true);
        Time.timeScale = 0;
    }

    private void AddStrike()
    {
        strikes++;
        SetStrikes(strikes);
        if (strikes >= 3)
        {
            LoseView.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void SetStrikes(int strikes)
    {
        for (int i = 0; i < strikeRenderer.Length; i++)
        {
            if (i >= strikes) return;
            strikeRenderer[i].sprite = redStrike;
        }
    }
}
