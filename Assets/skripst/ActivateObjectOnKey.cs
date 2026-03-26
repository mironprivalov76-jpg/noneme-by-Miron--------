using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    public GameObject targetObject; // объект
    public KeyCode key = KeyCode.E; // кнопка

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}