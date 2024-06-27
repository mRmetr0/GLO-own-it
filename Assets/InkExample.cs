using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;


public class InkExample : MonoBehaviour
{

    public TextAsset inkJSONAsset;
    private Story story;

    // Start is called before the first frame update
    void Start()
    {
        story = new Story(inkJSONAsset.text);   // Links this C# script to INK file
        Debug.Log(story.Continue());            
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
