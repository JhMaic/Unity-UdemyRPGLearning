using UnityEngine;

public class Parallex : MonoBehaviour
{
    [SerializeField] private float cameraFollowSpeed;

    private GameObject _cam;
    private Vector3 _camOldPos;
    private float _spriteWidth;

    private void Start()
    {
        _cam = GameObject.FindWithTag("MainCamera");
        _camOldPos = _cam.transform.position;
        _spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        FollowCamera();
    }

    private void FollowCamera()
    {
        var camNewPos = _cam.transform.position;
        var camMovement = camNewPos - _camOldPos;

        //无视Y
        camMovement.y = 0;
        var targetPos = transform.position + camMovement * cameraFollowSpeed;
        transform.position = targetPos;
        _camOldPos = camNewPos;


        // endless background
        if (camNewPos.x - transform.position.x > _spriteWidth)
            transform.position = new Vector3(transform.position.x + 2 * _spriteWidth, transform.position.y);
        else if (transform.position.x - camNewPos.x > _spriteWidth)
            transform.position = new Vector3(transform.position.x - 2 * _spriteWidth, transform.position.y);
    }
}