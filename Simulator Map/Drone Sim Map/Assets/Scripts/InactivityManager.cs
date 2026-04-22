using UnityEngine;
using UnityEngine.Video;

public class InGameVideoPause : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public float inactivityTimeout = 10f;
    private float inactivityTimer = 0f;
    private bool isVideoPlaying = false;

    void Update()
    {
        if (DetectInput())
        {
            inactivityTimer = 0f;
            if (isVideoPlaying)
            {
                StopVideo();
            }
        }
        else
        {
            inactivityTimer += Time.deltaTime;
            if (inactivityTimer >= inactivityTimeout && !isVideoPlaying)
            {
                PlayVideo();
            }
        }
    }

    bool DetectInput()
    {
        return Input.anyKey || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || Input.touchCount > 0;
    }

    void PlayVideo()
    {
        // Pause game
        Time.timeScale = 0;
        // Show video player UI
        videoPlayer.gameObject.SetActive(true);
        // Play the video
        videoPlayer.Play();
        isVideoPlaying = true;
    }

    public void StopVideo()
    {
        // Resume game
        Time.timeScale = 1;
        // Stop the video
        videoPlayer.Stop();
        // Hide video player UI
        videoPlayer.gameObject.SetActive(false);
        isVideoPlaying = false;
    }
}