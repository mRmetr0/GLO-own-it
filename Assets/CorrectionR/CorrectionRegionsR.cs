using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectionRegionsR : MonoBehaviour
{

    [SerializeField] private float workRadius;
    [SerializeField] private float workDuration;
    [SerializeField] private CorrectionNPCR.Action ruleToAbide;
    public CorrectionNPCR.Action RuleToAbide => ruleToAbide;

    private System.Random rand = new ();

    public float WorkDuration => workDuration;

    public Vector2 GetRandomWorkPos()
    {
        int angle = rand.Next(360);

        float xPos = transform.position.x + workRadius * Mathf.Cos(angle);
        float yPos = transform.position.y + workRadius * Mathf.Sin(angle);

        return new Vector2(xPos, yPos);
    }
}
