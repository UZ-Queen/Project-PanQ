using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//get inputs from user
public class Player : LivingEntity
{
    PlayerController playerController;
    GunController gunController;
    Camera mainCamera;

    [SerializeField]    float moveSpeed = 10;
    [SerializeField] Crosshair crossHair;

    
    protected override void Start()
    {
        base.Start();

        // Player dupes = FindObjectOfType<Player>();
        // dupes?.

        GameObject.DontDestroyOnLoad(gameObject);
        playerController = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        SetCamera();
        Debug.Log("로드 성공!");
    }
    void Update()
    {
        
        Vector3 dir = Vector3.Normalize(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));
        playerController.Move(dir*moveSpeed);

        if(Input.GetKeyDown(KeyCode.R)){
            gunController.Reload();
        }
        if(Input.GetMouseButton(0)){
            gunController.OnTriggerHold();
        }
        if(Input.GetMouseButtonUp(0)){
            gunController.OnTriggerRelease();
        }

        if(mainCamera == null){
            SetCamera();
        }
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.up* gunController.GunPosition.y);
        float distance;
        if(plane.Raycast(ray, out distance )){
            Vector3 point = ray.GetPoint(distance);
            crossHair.transform.position = point;
            playerController.LookAt(point);
            crossHair.DetectTarget(ray);
            gunController.LookCursor(point);

            Debug.DrawLine(ray.origin, point, Color.red);
        }

    }

    void SetCamera(){
        mainCamera = Camera.main;
    }

}
