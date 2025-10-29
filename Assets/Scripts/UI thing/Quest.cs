using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

[Serializable]
public class Step
{
    public bool isCompleted;
    public string description;
}

public class Quest : MonoBehaviour
{
    [SerializeField] private List<Step> steps;
    [SerializeField] private TMP_Text textField;

    private void CheckProgress()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("It has been decreed that thou shalt do the following:");
        sb.AppendLine();

        for (int i = 0; i < steps.Count; i++)
        {
            if (steps[i].isCompleted)
            {
                sb.Append("<style=\"checked\">");
                sb.Append(steps[i].description);
                sb.Append("</style>\n");
            }
            else
            {
                sb.Append("<style=\"unchecked\">");
                sb.Append(steps[i].description);
                sb.Append("</style>\n");
            }
        }

        textField.SetText(sourceText:sb.ToString());
    }

    public void CompleteStep(int index)
    {
        if (index < 0 || index >= steps.Count) return;
        steps[index].isCompleted = true;
        CheckProgress();
    }

    private void OnValidate()
    {
        CheckProgress();
    }

}
