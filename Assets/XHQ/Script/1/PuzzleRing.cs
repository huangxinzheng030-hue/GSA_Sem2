using UnityEngine;

[RequireComponent(typeof(AudioSource))] 
public class PuzzleRing : MonoBehaviour
{
    [Header("设置")]
    public float correctAngle = 0f;    
    public float rotateSpeed = 200f;   
    public float tolerance = 5f;       

    [Header("音效")]
    public AudioClip clickSound;      
    
    private float targetAngle;
    private bool isRotating = false;
    private bool isLocked = false;     
    private AudioSource audioSource;

    void Start()
    {

        targetAngle = transform.eulerAngles.z;
        audioSource = GetComponent<AudioSource>();
        
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        
        if (transform.eulerAngles.z != targetAngle)
        {
            Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            
            if (Quaternion.Angle(transform.rotation, targetRot) < 0.1f)
            {
                transform.rotation = targetRot; 
                isRotating = false;
            }
        }
    }

    void OnMouseDown()
    {
        if (isRotating || isLocked) return;

        RotateRing();
    }

    void RotateRing()
    {
        isRotating = true;
        targetAngle += 90f; 
        
        if (clickSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clickSound);
        }
    }

    public bool IsCorrect()
    {
        float currentZ = transform.eulerAngles.z;
        float difference = Mathf.DeltaAngle(currentZ, correctAngle);
        return Mathf.Abs(difference) < tolerance;
    }

    public void LockRing()
    {
        isLocked = true;
    }
}