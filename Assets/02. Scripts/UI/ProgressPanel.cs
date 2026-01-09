using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressPanel : MonoBehaviour
{
    [Header("패널")]
    public GameObject progressPanel;     // 진행도 패널


    // 닫기 버튼을 누르면 진행도 패널을 닫아라.
    public void OffProgress()
    {
        progressPanel.SetActive(false);
    }
}

