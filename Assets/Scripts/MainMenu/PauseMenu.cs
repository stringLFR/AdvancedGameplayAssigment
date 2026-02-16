using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    private FlowAction_PauseMenu pause;

    public void ActivatePause(FlowAction_PauseMenu menu)
    {
        pause = menu;
    }

    public void UnPause()
    {
        pause.SetDone();
        pause = null;
    }

    public void GoToMenuScene()
    {
        if (pause == null) return;

        pause.GoToMainMenu();

        gameObject.SetActive(false);
    }
}
