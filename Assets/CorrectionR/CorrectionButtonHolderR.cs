using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class CorrectionButtonHolderR : MonoBehaviour
{
    private CorrectionPlayerR player;
    private CorrectionButtonR helpButton = null;
    
    private void Awake()
    {
        player = FindObjectOfType<CorrectionPlayerR>();

        CorrectionButtonR[] buttons = GetComponentsInChildren<CorrectionButtonR>();
    }

    private void Start()
    {
        gameObject.SetActive(false);

        foreach (CorrectionButtonR b in GetComponentsInChildren<CorrectionButtonR>())
        {
            if (b.helpButton)
                helpButton = b;
        }
    }
    
    private void OnEnable()
    {

        Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var position = player.transform.position;
        Vector2 newPos = new Vector2(position.x, position.y + 1.5f);

        transform.position = Camera.main.WorldToScreenPoint(newPos);
        
        if (helpButton is not null) 
            helpButton.gameObject.SetActive(player.TalkingToNPC.NPCState == CorrectionNPCR.State.Working);
    }
}
