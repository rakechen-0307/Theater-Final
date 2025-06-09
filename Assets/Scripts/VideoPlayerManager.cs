using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoEndSceneSwitcher : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string nextSceneName;

    void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "StartVideo" && OSCSender.Instance != null)
        {
            OSCSender.Instance?.PlaySound("start", 1);
        }
        else if (sceneName == "EndVideo" && OSCSender.Instance != null)
        {
            OSCSender.Instance?.PlaySound("game2", 0);
            OSCSender.Instance?.PlaySound("end", 1);
        }
        else
        {
            Debug.LogWarning("Unrecognized scene: " + sceneName);
        }
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}