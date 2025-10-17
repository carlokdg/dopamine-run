using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RestartPage : MonoBehaviour
{
    [SerializeField] private AudioSource buttonSelect;

    public void Restart()
    {
        StartCoroutine(RestartButton());
    }

    private IEnumerator RestartButton()
    {
        buttonSelect.Play();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }
}
