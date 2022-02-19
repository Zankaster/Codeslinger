using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class Continue : MonoBehaviour, IPointerDownHandler {

    public void OnPointerDown(PointerEventData eventData) {
        ProgressSaveAndLoad.LoadGame();
        SceneManager.LoadScene("Hub");
    }
}
