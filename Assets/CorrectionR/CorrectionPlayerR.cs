using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CorrectionPlayerR : MonoBehaviour
{
    
    public enum PlayerState
    {
        Talking,
        Walking,
        Helping
    }

    [NonSerialized] public PlayerState state = PlayerState.Walking;
    
    private PlayerMovement movement;
    private CorrectionNPCR talkingToNPC = null;
    private CorrectionButtonHolderR buttonHolder;

    public CorrectionNPCR TalkingToNPC
    {
        get { return talkingToNPC; }
        set { talkingToNPC = value; }
    }

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        buttonHolder = FindObjectOfType<CorrectionButtonHolderR>();
    }

    private void Update()
    {
        switch (state)
        {
            case(PlayerState.Helping):
                if (Input.GetMouseButtonDown(0))
                {
                    state = PlayerState.Walking;
                    TalkingToNPC.Help(false);
                }


                if (talkingToNPC is not null && talkingToNPC.beingHelped) return;
                ActivatePlayer(true);
                TalkingToNPC = null;
                break;
            case (PlayerState.Talking):
                break;
            case(PlayerState.Walking):
                ClickedOnNPC();
                CheckTalkingToNPC();
                break; 
        }
    }
    
    private void ClickedOnNPC()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        talkingToNPC = null;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D collider = Physics2D.OverlapPoint(mousePos);
        if (collider is null) return;
        if (collider.gameObject.CompareTag("NPC"))
        {
            talkingToNPC = collider.GetComponent<CorrectionNPCR>();
        }
    }

    private void CheckTalkingToNPC()
    {
        if (talkingToNPC is null) return;
        
        if (Vector2.Distance(talkingToNPC.transform.position, transform.position) < 1.3f)
        {
            buttonHolder.gameObject.SetActive(true);
            ActivatePlayer(false, true);
            talkingToNPC.TalkedTo = true;
        }
    }
    
    public void ActivatePlayer(bool pCanMove, bool resetWalk = false)
    {
        state = pCanMove ? PlayerState.Walking : PlayerState.Talking;
        movement.canMove = pCanMove;
        if (resetWalk)
            movement.moveToPos = transform.position;
    }
}
