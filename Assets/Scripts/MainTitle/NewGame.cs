using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewGame : MonoBehaviour, IPointerDownHandler {

    public void OnPointerDown(PointerEventData eventData) {
        ProgressSaveAndLoad.SaveGame();
        SceneManager.LoadScene("Intro");
    }
}
