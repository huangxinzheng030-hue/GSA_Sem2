using UnityEngine;

public class BallMovementArrows : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 20f;
    
    [Header("调试开关（如果方向反了就勾选）")]
    public bool invertHorizontal = false;
    public bool invertVertical = true; // 针对你 Z=180 的情况，默认开启反转

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // 物理阻尼，防止球滑得停不下来
        rb.linearDamping = 2f;
        rb.angularDamping = 2f;
    }

    void FixedUpdate()
    {
        MoveBall();
    }

    void MoveBall()
    {
        // 1. 获取输入
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h == 0 && v == 0) return;

        // 2. 补偿反转逻辑
        if (invertHorizontal) h *= -1f;
        if (invertVertical) v *= -1f;

        // 3. 获取相机在水平面上的旋转角度
        // 我们只关心相机绕 Y 轴转了多少度，彻底无视倒立的 Z 轴
        float camYRotation = Camera.main.transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, camYRotation, 0);

        // 4. 将输入方向映射到相机的水平视角方向
        Vector3 inputDir = new Vector3(h, 0, v);
        Vector3 moveDir = rotation * inputDir;

        // 5. 应用物理力
        rb.AddForce(moveDir.normalized * moveSpeed, ForceMode.Force);
    }
}