using UnityEngine;

public class HoopTrigger : MonoBehaviour
{
    private bool hasScored = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasScored) return;

        if (other.CompareTag("MainCamera")) 
        {
            hasScored = true;
            ScoreManager.instance.AddPoint();

            //reset hoop after 5 seconds
            Invoke(nameof(ResetHoop), 5f);
        }
    }

    void ResetHoop()
    {
        hasScored = false;
    }
}