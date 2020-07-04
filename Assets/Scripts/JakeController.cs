using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JakeController : MonoBehaviour
{
    Animator animator;
    float horizontal;
    Vector2 lookDirection = new Vector2(0,-1);
    Rigidbody2D rigidbody2d;
    public float speed = 3.0f;
    float vertical;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // GetAxisRaw allows us to get 'digital' input without smoothing the results, this still doesn't fix actual analog input... >:|
        horizontal = Mathf.Round(Input.GetAxisRaw("Horizontal"));
        vertical = Mathf.Round(Input.GetAxisRaw("Vertical"));
        Debug.Log($"{horizontal}, {vertical}");

        Vector2 move = new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
                
        animator.SetFloat("LookX", lookDirection.x);
        animator.SetFloat("LookY", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (horizontal != 0 || vertical != 0) {
            animator.Play("Run");
        } else {
            animator.Play("Idle");
        }
    }

    void FixedUpdate()
    {
        // Debug.Log(horizontal + "," + vertical);

        Vector2 position = rigidbody2d.position;
        Vector3 heading = new Vector3(horizontal, vertical, 0);

        if (horizontal == 0 || vertical == 0) {
            heading = SnapTo(heading, 90);
            position.x = position.x + speed * 1.5f * heading.x * Time.deltaTime;
        } else {
            heading = SnapTo(heading, 57);
            position.x = position.x + speed * heading.x * Time.deltaTime;
        }

            position.y = position.y + speed * heading.y * Time.deltaTime;

        
        rigidbody2d.MovePosition(position);
    }

    Vector3 SnapTo(Vector3 v3, float snapAngle) {
        float angle = Vector3.Angle (v3, Vector3.up);

        if (angle < snapAngle / 2.0f)          // Cannot do cross product 
            return Vector3.up * v3.magnitude;  //   with angles 0 & 180
        
        if (angle > 180.0f - snapAngle / 2.0f)
            return Vector3.down * v3.magnitude;
        
        float t = Mathf.Round(angle / snapAngle);
        float deltaAngle = (t * snapAngle) - angle;
        
        Vector3 axis = Vector3.Cross(Vector3.up, v3);
        Quaternion q = Quaternion.AngleAxis (deltaAngle, axis);
        
        return q * v3;
    }
}
