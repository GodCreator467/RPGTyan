using UnityEngine;
using UnityEngine.UI;

public class ModeHelper : MonoBehaviour {

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }
    void TaskOnClick()
    {
        StatInfo.Mode = GetComponent<Button>().name;
    }
}
