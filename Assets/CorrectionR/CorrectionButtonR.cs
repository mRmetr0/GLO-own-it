using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CorrectionButtonR : MonoBehaviour
{
    [SerializeField] private CorrectionNPCR.Action action;
    public bool helpButton = false;
    private CorrectionPlayerR player;

    private void Awake()
    {
        player = FindObjectOfType<CorrectionPlayerR>();
        GetComponent<Button>().onClick.AddListener(OnButtonPress);
    }

    private void OnButtonPress()
    {
        transform.parent.gameObject.SetActive(false);
        if (helpButton)
        {
            player.state = CorrectionPlayerR.PlayerState.Helping;
            player.TalkingToNPC.Help(true);
            player.TalkingToNPC.TalkedTo = false;
            return;
        }

        player.ActivatePlayer(true);
        player.TalkingToNPC.TalkedTo = false;
        player.TalkingToNPC.AskEquipmentFix(action);
        player.TalkingToNPC = null;
    }
}
