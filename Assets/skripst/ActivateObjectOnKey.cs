using UnityEngine;

public class ActivateObjectOnKey : MonoBehaviour
{
    public GameObject targetObject; // объект который нужно включить
    public KeyCode key = KeyCode.E; // кнопка

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            targetObject.SetActive(true);
        }
    }
}