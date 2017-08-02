using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockEditorUI : MonoBehaviour {

    Text titleText;
    RectTransform editContainer;

    public PatternBlockUI currentBlock;

    //Inputs
    InputField timeInput;
    InputField cyclesInput;
    InputField[][] freqInputs;
    

    void Awake()
    {
        titleText = transform.Find("Title").GetComponent<Text>();

        editContainer = transform.Find("EditContainer").GetComponent<RectTransform>();
        
        timeInput = editContainer.Find("TimeInput").GetComponent<InputField>();
        cyclesInput = editContainer.Find("CyclesInput").GetComponent<InputField>();

        freqInputs = new InputField[3][];
        for (int i = 0; i < 3; i++)
        {
            freqInputs[i] = new InputField[3];
            freqInputs[i][0] = editContainer.Find("F" + i + "X").GetComponent<InputField>();
            freqInputs[i][1] = editContainer.Find("F" + i + "Y").GetComponent<InputField>();
            freqInputs[i][2] = editContainer.Find("F" + i + "Z").GetComponent<InputField>();
        }

        
    }
    
	// Use this for initialization
	void Start () {
        updateData();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void editBlock(PatternBlockUI block)
    {
        if (currentBlock == block) return;
        if (currentBlock != null) currentBlock.setEditing(false);
        currentBlock = block;
        if (currentBlock != null) currentBlock.setEditing(true);
        updateData();
    }

    public void updateData()
    {

        titleText.text = currentBlock == null ? "Clic to edit a block":"Editing Block " + currentBlock.blockIndex;
        editContainer.gameObject.SetActive(currentBlock != null);

        if (currentBlock == null) return;

        PatternData p = currentBlock.pattern;
        timeInput.text = p.time.ToString();
        cyclesInput.text = p.cycles.ToString();
        for (int i=0;i<3;i++)
        {
            freqInputs[i][0].text = p.frequencies[i].x.ToString();
            freqInputs[i][1].text = p.frequencies[i].y.ToString();
            freqInputs[i][2].text = p.frequencies[i].z.ToString();
        }
    }

    public void updatePatternData()
    {
        if (currentBlock == null) return;

        PatternData p = currentBlock.pattern;
        float.TryParse(timeInput.text, out p.time);
        float.TryParse(cyclesInput.text, out p.cycles);
        for (int i = 0; i < 3; i++)
        {
            float.TryParse(freqInputs[i][0].text, out p.frequencies[i].x);
            float.TryParse(freqInputs[i][1].text, out p.frequencies[i].y);
            float.TryParse(freqInputs[i][2].text, out p.frequencies[i].z);
        }

        currentBlock.updateData();
    }

  
}
