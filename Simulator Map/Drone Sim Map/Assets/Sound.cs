using UnityEngine;
using UnityEngine.InputSystem;

public class DroneSound : MonoBehaviour
{
    private GameInput input;
    private AudioSource audioSource;

    void Awake()
    {
        input = new GameInput();
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        input.Player.Enable();
    }

    void OnDisable()
    {
        input.Player.Disable();
    }

    void Update()
    {
        // Get movement input (WASD / stick)
        Vector2 moveInput = input.Player.Move.ReadValue<Vector2>();

        // Check if player is actually moving
        bool isMoving = moveInput.magnitude > 0.1f;

        if (isMoving)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class droneSound : MonoBehaviour
//{
//    public AudioSource droneSoundAudio;

//    private GameInput input;

//    void Awake()
//    {
//        input = new GameInput();
//    }

//    void OnEnable()
//    {
//        input.Player.Enable();
//    }

//    void OnDisable()
//    {
//        input.Player.Disable();
//    }

//    void Update()
//    {
//        // Read the Dash input (works for keyboard + controller)
//        float dashInput = input.Player.Dash.ReadValue<float>();

//        if (dashInput > 0)
//        {
//            // Start playing if not already playing
//            if (!droneSoundAudio.isPlaying)
//            {
//                droneSoundAudio.Play();
//            }
//        }
//        else
//        {
//            // Stop immediately when button is released
//            if (droneSoundAudio.isPlaying)
//            {
//                droneSoundAudio.Stop();
//            }
//        }
//    }
//}
//add audio file to where all other assets/script are
//add audio clip to scene (anywhere, in this case drag/drop onto player)
//turn on loop and play on awake , disable whole audio source section
//drag script to scene (player) then add player to drone sound in script drop down (in player)