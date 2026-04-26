using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Video;

public class IdleVideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject videoUI; // Drag your Raw Image object here
    public float idleThreshold = 10f;
    private float idleTimer;

    void OnEnable()
    {
        InputSystem.onEvent += ResetTimer;
    }

    void OnDisable()
    {
        InputSystem.onEvent -= ResetTimer;
    }

    void ResetTimer(InputEventPtr eventPtr, InputDevice device)
    {
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>()) return;

        idleTimer = 0;

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            videoUI.SetActive(false); // <--- HIDES the video UI when you move
        }
    }

    void Update()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleThreshold && !videoPlayer.isPlaying)
        {
            videoUI.SetActive(true); // <--- SHOWS the video UI when idle
            videoPlayer.Play();
        }
    }
}