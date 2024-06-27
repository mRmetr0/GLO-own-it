using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class CorrectionNPCR : MonoBehaviour
{
    //Enums:
    public enum State
    {
        Null,
        Walking,
        Working,
        FixingClothing,
        Dead
    }
    private State state = State.Null;
    private bool breakingRule;
    public bool TalkedTo = false;
    public bool beingHelped = false;
    
    public enum Action
    {
        Null,
        HardHat,
        NoSmoking,
        EarProtector,
        Goggles,
        Gloves
    }
    [SerializeField] private Action action;
    private Action actionToFix = Action.Null;

    //Remaining variables:
    private CorrectionManagerR manager;
    private System.Random rand = new();
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private float helpSpeed = 2;
    [SerializeField] private CorrectionRegionsR[] tasks;
    private int taskProgress = 0;
    private Vector2 taskPos = Vector2.zero;
    
    [SerializeField] private Slider WorkSlider;
    [SerializeField] private Image WorkSliderFill;
    private float elpasedToWork = 0;
    
    [Header("SPRITES")]
    [SerializeField] private Sprite NeutralSprite;
    [SerializeField] private Sprite SmokeSprite;
    [SerializeField] private Sprite HelmetSprite;
    [SerializeField] private Sprite EarProtectionSprite;
    [SerializeField] private Sprite GogglesSprite;
    [SerializeField] private Sprite GlovesSprite;
    [SerializeField] private Sprite DeathSprite;
    private SpriteRenderer _renderer;
    
    public static System.Action OnDone;
    public static System.Action OnDeath;
    public State NPCState => state;

    public void SetUp(CorrectionManagerR pManager, CorrectionRegionsR[] pTasks)
    {
        List<CorrectionRegionsR> regions = pTasks.ToList();
        regions = regions.OrderBy(x => rand.Next()).ToList();
        if (rand.Next(0, 100) < 30)
            regions.RemoveAt(regions.Count-1);
        if (rand.Next(0, 100) < 10)
            regions.RemoveAt(regions.Count-1);

        tasks = regions.ToArray();
        
        
        manager = pManager;
        taskProgress = -1;
        NextTask();
    }

    private void Start()
    {
        _renderer = GetComponent <SpriteRenderer>();
    }

    private void Update()
    {
        if (TalkedTo) return;
        switch (state)
        {
            case(State.Walking):
                if (Vector2.Distance(transform.position, taskPos) > 0.2f)
                {
                    Vector2 position = transform.position;
                    Vector2 moveDir = new Vector2(taskPos.x - position.x, taskPos.y - position.y).normalized;
                    transform.position = position + moveDir * (moveSpeed * Time.deltaTime);
                }
                else
                {
                    if (taskProgress < 0)
                    {
                        manager.NPCAmountActive--;
                        OnDone?.Invoke();
                        Destroy(gameObject);
                        return;
                        //GIVE POINTS OR SOMETHING
                    }

                    elpasedToWork = tasks[taskProgress].WorkDuration;
                    ChangeState(State.Working);
                }
                break;
            case(State.Working):
                if (elpasedToWork <= 0) {
                    Help(false);
                    if (action != tasks[taskProgress].RuleToAbide)
                    {
                        Die();
                        return;
                    }
                    NextTask();
                } else
                {
                    elpasedToWork -= Time.deltaTime * (beingHelped ? helpSpeed : 1f);
                    WorkSlider.value = (tasks[taskProgress].WorkDuration - elpasedToWork) / tasks[taskProgress].WorkDuration;
                }
                break;
            case(State.FixingClothing):
                Vector2 cabinetPos = manager.cabinet.transform.position;
                if (Vector2.Distance(transform.position, cabinetPos) > 0.2f)
                {
                    Vector2 position = transform.position;
                    Vector2 moveDir = new Vector2(cabinetPos.x - position.x, cabinetPos.y - position.y).normalized;
                    transform.position = position + moveDir * (moveSpeed * Time.deltaTime);
                    break;
                }
                SetEquipment(actionToFix);
                ChangeState(State.Walking);
                break;
        }
    }

    private void NextTask()
    {
        actionToFix = Action.Null;
        ChangeState(State.Walking);
        if (taskProgress >= tasks.Length - 1)
        {
            taskProgress = -2;
            taskPos = manager.transform.position;
            return;
        }

        taskProgress++;
        taskPos = tasks[taskProgress].GetRandomWorkPos();

        if (rand.Next(0, 100) < 50)
        {
            AskEquipmentFix(tasks[taskProgress].RuleToAbide);
        }
    }

    public void Help(bool helping = true)
    {
        beingHelped = helping;
        WorkSliderFill.color = helping ? Color.green : Color.white;
    }

    public void AskEquipmentFix(Action pActionToFix)
    {
        if (pActionToFix == Action.Null) return;
        actionToFix = pActionToFix;
        ChangeState(State.FixingClothing);
    }

    private void SetEquipment(Action toFix)
    {
        switch (toFix)
        {
            case (Action.HardHat):
                _renderer.sprite = HelmetSprite;
                break;
            case(Action.NoSmoking):
                _renderer.sprite = SmokeSprite;
                break;            
            case(Action.EarProtector):
                _renderer.sprite = EarProtectionSprite;
                break;            
            case(Action.Goggles):
                _renderer.sprite = GogglesSprite;
                break;
            case(Action.Gloves):
                _renderer.sprite = GlovesSprite;
                break;
            case (Action.Null):
            default:
                _renderer.sprite = NeutralSprite;
                break;
        }

        action = toFix;
        actionToFix = Action.Null;
    }

    private void ChangeState(State changeTo)
    {
        WorkSlider.gameObject.SetActive(changeTo == State.Working);
        state = changeTo;
    }

    private void Die()
    {
        ChangeState(State.Null);
        manager.NPCAmountActive--;
        _renderer.sprite = DeathSprite;
        OnDeath?.Invoke();
        GetComponent<BoxCollider2D>().enabled = false;
        enabled = false;
    }
}
