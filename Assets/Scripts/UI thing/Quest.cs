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

        sb.AppendLine("Job List:");
        sb.AppendLine();

        for (int i = 0; i < steps.Count; i++)
        {
            // 체크/언체크 아이콘 정하기
            string icon = steps[i].isCompleted ? "[X] " : "[ ] ";


            // 색도 주고 싶으면 이렇게:
            // string icon = steps[i].isCompleted ? "<color=#00FF00>☑ </color>" : "<color=#FFFFFF>☐ </color>";

            sb.Append(icon);
            sb.Append(steps[i].description);

            if (i < steps.Count - 1)
                sb.AppendLine();
        }

        // Rich Text 사용하는 버전이면 SetText 말고 text 써도 됨
        textField.SetText(sb.ToString());
        // textField.text = sb.ToString();
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
