﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Some of movement and camera adapted from https://www.youtube.com/watch?v=n-KX8AeGK7E&t=913s
public class PlayerCam : MonoBehaviour
{
    private bool seenItem = false;
    public CanvasControl canControl;

    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private float mouseSensitivity;

    [SerializeField] private Transform playerBody;

    private float xAxisClamp;
    bool crouch;
    public bool allowPlayerControl;
    public GameObject lastSeen;
    public LightUpObject objectShader;

    public Light directionalLight;
    public GameObject[] blackLightObjects;
    public bool blacklight_have;
    public bool blackLightOn;

    private bool ventMoved;

    public GameObject PuzzleCam;
    public GameObject slidingDoor;
    public WallRaise slidingWall;

    public hack1 firstPuzzleCam;
    public hack2 secondPuzzleCam;
    public PlayerMove playerMove;

    private float startTime;
    public int countDownTime;

    public bool keycard;

    public GameObject hack1;    
    public GameObject hack2;

    private void Awake()
    {
        LockCursor();
        xAxisClamp = 0.0f;
        //allowPlayerControl = true;
        mouseXInputName = "Mouse X";
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        crouch = false;
        ventMoved = false;
        keycard = false;
        directionalLight.intensity = 0;
        blackLightObjects = GameObject.FindGameObjectsWithTag("blacklight");
        blacklight_have = false;
        blackLightOn = false;
        countDownTime = 100;
        //Canvas mainCanvas = GameObject.FindObjectOfType<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            if(blacklight_have && blackLightOn == false && countDownTime > 0)
            {
                BlackLight();
                blackLightOn = true;
            }
            else if (blacklight_have && blackLightOn == true && countDownTime == 0)
            {
                BlackLightOff();
                blackLightOn = false;
            }

        }
        
        
        Canvas mainCanvas = GameObject.FindObjectOfType<Canvas>();
        if (blackLightOn == true)
        {
            if (countDownTime != 0)
            {
                countDownTime -= 1;
            }
            else if (countDownTime == 0)
            {
                blackLightOn = false;
                BlackLightOff();
            }
            mainCanvas.SendMessage("CountDown", countDownTime);
        }
        if (blackLightOn == false && blacklight_have == true)
        {
            if (countDownTime != 100)
            {
                countDownTime += 1;
            }
            mainCanvas.SendMessage("CountDown", countDownTime);
        }
        RaycastHit hit;
        if (allowPlayerControl == true)
        {
            CameraRotation();
            //Debug.DrawRay(transform.position,transform.forward, Color.green,.01f);
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.P))
            {
                Application.Quit();
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    if(crouch == false)
                    {
                        //print("crouch");
                        transform.localPosition = new Vector3(0,-.127f,0);
                        crouch = true;
                        transform.parent.GetComponent<CharacterController>().height = .75f;
                    }
                }
                if (Input.GetKeyUp(KeyCode.LeftControl))
                {
                    if(crouch == true)
                    {
                        transform.localPosition = new Vector3(0,.311f,0);
                        crouch = false;
                        transform.parent.GetComponent<CharacterController>().height = 1.8f;
                    }
                }
            if (Physics.Raycast(transform.position, transform.forward, out hit, 10.0f))
            {
                if (seenItem == false && hit.transform.tag != "Untagged")
                {

                    /*if (hit.transform.tag != "monitor" && hit.transform.tag != "hack1" || (
                        ventMoved == true && hit.transform.tag == "Vent"))
                    {
                        mainCanvas.SendMessage("PressE");
                        seenItem = true;

                        lastSeen = hit.transform.gameObject;
                        objectShader = lastSeen.GetComponent<LightUpObject>();

                        //objectShader.isLit = true;
                    }*/
                    if (hit.transform.tag == "hack1")
                    {
                        mainCanvas.SendMessage("PressE");
                        seenItem = true;

                        lastSeen = hit.transform.gameObject;

                    }
                    if (hit.transform.tag == "monitor")
                    {
                        if (slidingWall.move == false)
                        {
                            mainCanvas.SendMessage("PressE");
                            seenItem = true;

                            lastSeen = hit.transform.gameObject;
                            objectShader = lastSeen.GetComponent<LightUpObject>();

                            objectShader.isLit = true;
                        }
                    }
                    if (hit.transform.tag == "keycard" || hit.transform.tag == "vent2" || hit.transform.tag == "hack2")
                    {
                        mainCanvas.SendMessage("PressE");
                        seenItem = true;

                        lastSeen = hit.transform.gameObject;
                    }

                    if (hit.transform.tag == "keycardreader")
                    {
                        if(keycard)
                        {
                            mainCanvas.SendMessage("PressE");
                            seenItem = true;
                            lastSeen = hit.transform.gameObject;
                        }
                    }

                    if(hit.transform.tag == "document")
                    {
                        mainCanvas.SendMessage("PressE");
                        seenItem = true;
                        lastSeen = hit.transform.gameObject;
                    }
                    if (hit.transform.tag == "safe" || hit.transform.tag == "actualblacklight" || hit.transform.tag == "lightswitch" ||
                        hit.transform.tag == "Vent" || hit.transform.tag == "vent2")
                    {
                        //Debug.Log("safety");
                        mainCanvas.SendMessage("PressE");
                        seenItem = true;
                        lastSeen = hit.transform.gameObject;
                    }
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.transform.tag == "actualblacklight")
                    {
                        blacklight_have = true;
                        GameObject[] bls = GameObject.FindGameObjectsWithTag("actualblacklight");
                        foreach (GameObject bl in bls)
                        {
                            Destroy(bl);
                        }
                        lastSeen = null;
                        seenItem = false;
                        mainCanvas.SendMessage("ClearText");
                    }
                    if (hit.transform.tag == "document")
                    {
                        //canControl.docCount += 1;
                        //canControl.collectedDocuments.Add(hit.transform.gameObject.name);
                        if (hit.transform.gameObject.name == "Document(Harper)")
                        {
                            canControl.firstFound = true;
                        }
                        if (hit.transform.gameObject.name == "Document(Dr. Perez) ")
                        {
                            canControl.secondFound = true;
                        }
                        if (hit.transform.gameObject.name == "Document(Resignation)")
                        {
                            canControl.thirdFound = true;
                        }
                        if (hit.transform.gameObject.name == "Document(Autopsy Report)")
                        {
                            canControl.fourthFound = true;
                        }
                        if (hit.transform.gameObject.name == "Document(25 Jun Report)")
                        {
                            canControl.fifthFound = true;
                        }
                        if (hit.transform.gameObject.name == "Document(Crystal)")
                        {
                            canControl.sixthFound = true;
                        }
                        Destroy(hit.transform.gameObject);
                        mainCanvas.SendMessage("ClearText");
                        lastSeen = null;
                        seenItem = false;
                        //objectShader = null;
                    }

                    if (hit.transform.tag == "hack1")
                    {
                        //print("dufuq");
                        mainCanvas.SendMessage("ClearText");
                        hack1.GetComponent<HackCube>().enabled = true;
                        allowPlayerControl = false;
                        playerMove.allowPlayerMovement = false;
                        firstPuzzleCam.transform.GetComponent<Camera>().enabled = true;
                        transform.GetComponent<Camera>().enabled = false;
                    }

                    if(hit.transform.tag == "hack2")
                    {
                        mainCanvas.SendMessage("ClearText");
                        hack2.GetComponent<HackCube>().enabled = true;
                        allowPlayerControl = false;
                        playerMove.allowPlayerMovement = false;
                        secondPuzzleCam.transform.GetComponent<Camera>().enabled = true;
                        transform.GetComponent<Camera>().enabled = false;
                    }
                    if (hit.transform.tag == "Vent")
                    {
                        hit.transform.SendMessage("MoveTheVent");
                        ventMoved = true;
                        mainCanvas.SendMessage("ClearText");
                    }

                    if(hit.transform.tag == "vent2")
                    {
                        hit.transform.SendMessage("MoveTheVent");
                        mainCanvas.SendMessage("ClearText");
                    }

                    if (hit.transform.tag == "keycard")
                    {
                        hit.transform.GetComponent<keycard>().SendMessage("Pickup");
                        GameObject[] kks = GameObject.FindGameObjectsWithTag("keycard");
                        foreach (GameObject kk in kks)
                        {
                            Destroy(kk);
                        }
                        keycard = true;
                        lastSeen = null;
                        seenItem = false;
                        mainCanvas.SendMessage("ClearText");
                    }

                    if (hit.transform.tag == "keycardreader")
                    {
                        if(keycard)
                        {
                            hit.transform.GetComponent<keycardreader>().SendMessage("Open");
                            mainCanvas.SendMessage("ClearText");
                            seenItem = false;
                            lastSeen = null;
                        }
                        
                    }

                    /*if (hit.transform.GetComponent<Camera>() != null)
                    {
                        print("yep");
                        allowPlayerControl = false;
                        GetComponent<Camera>().enabled = false;
                        PuzzleCam.GetComponent<Camera>().enabled = true;
                        
                    }*/
                    if (hit.transform.tag == "monitor")
                    {
                        
                            
                         slidingWall.move = true;

                         objectShader.isLit = false;
                         seenItem = false;
                         mainCanvas.SendMessage("ClearText");
                        
                        
                    }

                    if(hit.transform.tag == "Crystal")
                    {
                        GameObject.FindGameObjectWithTag("FinalPuzzleCam").SendMessage("SwitchHere");
                        transform.GetComponent<Camera>().enabled = false;
                        playerMove.allowPlayerMovement = false;
                        allowPlayerControl = false;
                        mainCanvas.SendMessage("ClearText");
                    }
                    if (hit.transform.tag == "safe")
                    {
                        mainCanvas.SendMessage("NumPadOn");
                        mainCanvas.SendMessage("ClearText");
                        seenItem = false;
                    }

                    if(hit.transform.tag == "lightswitch")
                    {
                        hit.transform.SendMessage("Toggle");
                    }
                }
                
                if (seenItem == true)
                {
                    if (Physics.Raycast(transform.position, transform.forward, out hit, 10.0f))
                    {
                        if (lastSeen != null)
                        {
                            Vector3 targetDir = lastSeen.transform.position - transform.position;
                            float angle = Vector3.Angle(targetDir, transform.forward);
                            if (hit.transform.tag == "Untagged" && angle > 15)
                            {
                                mainCanvas.SendMessage("ClearText");
                                seenItem = false;
                                //objectShader.isLit = false;
                            }
                        }
                        
                    }
                }
            }
            if (!Physics.Raycast(transform.position, transform.forward, out hit, 10.0f))
            {
                Vector3 targetDir = lastSeen.transform.position - transform.position;
                float angle = Vector3.Angle(targetDir, transform.forward);
                if (angle > 15)
                {
                    seenItem = false;
                    mainCanvas.SendMessage("ClearText");
                    if (lastSeen.transform.tag == "monitor")
                    {
                        objectShader.isLit = false;
                    }
                    
                }
                
            }

            if (allowPlayerControl == false)
            {
                Vector3 targetDir = lastSeen.transform.position - transform.position;
                float angle = Vector3.Angle(targetDir, transform.forward);
                if (angle > 15)
                {
                    seenItem = false;
                    mainCanvas.SendMessage("ClearText");
                    if (lastSeen.transform.tag == "monitor")
                    {
                        objectShader.isLit = false;
                    }
                }
            }
        }
    }
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    private void CameraRotation()
    {
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        xAxisClamp += mouseY;

        if (xAxisClamp > 90.0f)
        {
            xAxisClamp = 90.0f;
            mouseY = 0.0f;
            ClampXAxisRot(270.0f);
        }
        else if (xAxisClamp < -90.0f)
        {
            xAxisClamp = -90.0f;
            mouseY = 0.0f;
            ClampXAxisRot(90.0f);
        }

        transform.Rotate(Vector3.left * mouseY);
        playerBody.Rotate(Vector3.up * mouseX);
    }
    private void ClampXAxisRot(float val)
    {
        Vector3 eulerRot = transform.eulerAngles;
        eulerRot.x = val;
        transform.eulerAngles = eulerRot;
    }

    public void Solved()
    {
        allowPlayerControl = true;
        playerMove.allowPlayerMovement = true;
        GetComponent<Camera>().enabled = true;
        //PuzzleCam.GetComponent<Camera>().enabled = false;
    }

    void OpenSmallDoor()
    {
        Vector3 slide = new Vector3(0, 6.88f, 0);
        Vector3 start = slidingDoor.transform.position;
        //slidingDoor.transform.position += slide;
        slide += slidingDoor.transform.position;
        slidingDoor.transform.position = Vector3.Lerp(start, slide, 15.0f);
    }
    void BlackLight()
    {
        directionalLight.intensity = 1;
        foreach (GameObject enableObject in blackLightObjects)
        {
            enableObject.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    void BlackLightOff()
    {
        directionalLight.intensity = 0;
        foreach (GameObject enableObject in blackLightObjects)
        {
            enableObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

}
