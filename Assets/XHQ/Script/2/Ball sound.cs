using UnityEngine;

public class BallRollingSound : MonoBehaviour
{
    public AudioSource rollingSource;
    public Rigidbody rb;
    
    public float maxSpeed = 10f; 
    public float minPitch = 0.5f;
    public float maxPitch = 1.5f;

    void Start()
    {
        if (rollingSource != null)
        {
            rollingSource.loop = true;
            rollingSource.Stop(); // 确保开始时是停止的
            rollingSource.volume = 0;
        }
    }

    void Update()
    {
        if (rollingSource == null || rb == null) return;

        // 获取水平速度（忽略掉微小的垂直抖动）
        Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float currentSpeed = horizontalVel.magnitude;

        // 判定条件：速度必须大于 0.2f 且 必须在地面
        if (currentSpeed > 0.2f && IsGrounded())
        {
            if (!rollingSource.isPlaying) 
            {
                rollingSource.Play();
            }

            // 平滑调整音量和音调
            float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);
            rollingSource.volume = Mathf.Lerp(rollingSource.volume, speedRatio, Time.deltaTime * 5f);
            rollingSource.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);
        }
        else
        {
            // 没达到移动条件时，快速淡出音量
            rollingSource.volume = Mathf.MoveTowards(rollingSource.volume, 0, Time.deltaTime * 2f);
            
            if (rollingSource.volume <= 0 && rollingSource.isPlaying)
            {
                rollingSource.Stop();
            }
        }
    }

    bool IsGrounded()
    {
        // 增加射线的偏置，确保是从球体中心向下发出的
        return Physics.Raycast(transform.position, Vector3.down, (transform.localScale.y / 2) + 0.1f);
    }
}