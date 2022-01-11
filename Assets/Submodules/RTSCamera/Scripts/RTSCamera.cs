using UnityEngine;

public enum MouseInputs
{
	none, left, right, middle
}

[System.Serializable]
public class StartingOffset
{
	public Vector3 startingPos = new Vector3(0f, 5f, -5f);
	public Vector3 startingRot = new Vector3(45f, 0f, 0f);
}

public class RTSCamera : MonoBehaviour
{
	#region Variables
	[Space(5)]
	[Header("Camera Field")]
	public GameObject orbitalCam;

	[Space(5)]
	[Header("Initialization")]
	public StartingOffset startingOffset;

	[Space(10)]
	[Header("Mouse Inputs")]
	public MouseInputs mouseButtonMove = MouseInputs.left;
	private MouseInputs mouseMoveBackup = MouseInputs.left;
	private int _mouseButtonMove = 0;
	public MouseInputs mouseButtonRotate = MouseInputs.right;
	private MouseInputs mouseRotateBackup = MouseInputs.right;
	private int _mouseButtonRotate = 1;

	[Space(10)]
	[Header("Camera Movement")]
	public float cameraBaseHeight;
	public float baseCameraSpeed;
	public float fastCameraSpeed;
	float cameraSpeed;
	public float movementSmoothness;	
	public float dragSpeed;
	public float maxDragSpeed;

	public bool borderPan = true;
	public float borderThickness;

	[Space(10)]
	[Header("Camera rotation")]
	public bool hideCursor = true;
	public float orbitalSpeed;
	public float orbitalSmoothness;
	public float keysRotationFactor;

	[Space(10)]
	[Header("Camera Zoom")]
	public Vector3 zoomAmt;
	public float zoomSpeed;
	public float zoomSmoothness;
	public float maxInZoom;
	public float maxOutZoom;

	[Space(10)]
	[Header("Focus")]
	public LayerMask focusMask;
	Transform focusTransform;

	Vector3 newPos;
	Quaternion newRot;
	Vector3 newZoom;
    #endregion


    void OnValidate()
	{
		CheckInputs();

		if(orbitalCam != null && !Application.isPlaying)
        {
			orbitalCam.transform.localPosition = startingOffset.startingPos;
			orbitalCam.transform.localEulerAngles = startingOffset.startingRot;
		}
	}

    void Start()
	{
		newPos = transform.position;
		newZoom = orbitalCam.transform.localPosition;
		Cursor.lockState = borderPan ? CursorLockMode.Confined : CursorLockMode.None;
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			cameraSpeed = fastCameraSpeed;
		}
		else
		{
			cameraSpeed = baseCameraSpeed;
		}
	}

	void LateUpdate()
	{
		if (focusTransform == null)
		{
			HandleKeyBoardMovementInput();
			HandleMouseDragMovement();
			newPos.y = cameraBaseHeight;
		}
		else
		{
			newPos = focusTransform.transform.position;
		}

		HandleOrbitalInput();
		HandleZoom();
		Focus();
		transform.position = Vector3.Lerp(transform.position, newPos, Time.unscaledDeltaTime * movementSmoothness);
	}

	void HandleKeyBoardMovementInput()
	{
		float horizontalInput = Input.GetAxis("Horizontal");
		float verticalInput = Input.GetAxis("Vertical");

		if(borderPan)
			HandleScreenEdgesMovement(ref horizontalInput, ref verticalInput);

		newPos += (transform.right * horizontalInput + (new Vector3(transform.forward.x, 0f, transform.forward.z) * verticalInput)) * Time.unscaledDeltaTime * cameraSpeed;
	}

	bool IsMouseOverGameWindow { get { return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y ||
				Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y); } }

	void HandleScreenEdgesMovement(ref float xInput, ref float yInput)
    {
		if(IsMouseOverGameWindow && Application.isPlaying)
        {
			if (Input.mousePosition.x < borderThickness)
			{
				xInput = -1f;
			}
			if (Input.mousePosition.x >= Screen.width - borderThickness)
			{
				xInput = 1f;
			}
			if (Input.mousePosition.y < borderThickness)
			{
				yInput = -1f;
			}
			if (Input.mousePosition.y >= Screen.height - borderThickness)
			{
				yInput = 1f;
			}
		}
		
    }

	float mouseX;
	float mouseY;
	bool mouseDragging = false;
	void HandleMouseDragMovement()
	{
		if (_mouseButtonMove == 10)
			return;

		if (Input.GetMouseButton(_mouseButtonMove) && !FocusRayChecker())
		{
			mouseDragging = true;
			SetCursorVisible(false);
		}
		if (Input.GetMouseButtonUp(_mouseButtonMove))
		{
			SetCursorVisible(true);
			mouseDragging = false;
			mouseX = 0f;
			mouseY = 0f;
		}

		if(mouseDragging)
        {
			mouseX += Input.GetAxis("Mouse X") * 0.1f * dragSpeed;
			mouseY += Input.GetAxis("Mouse Y") * 0.1f * dragSpeed;
			Vector3 clampedNewPos = Vector3.ClampMagnitude(new Vector3(transform.forward.x, 0f, transform.forward.z) * -mouseY + transform.right * -mouseX, maxDragSpeed);
			newPos = transform.position + clampedNewPos;
		}
	}

	float orbitalX;
	float orbitalY;

	void HandleOrbitalInput()
	{
		if (_mouseButtonRotate == 10)
			return;

		if (Input.GetMouseButton(_mouseButtonRotate))
		{
			SetCursorVisible(false);
		
			orbitalX += Input.GetAxis("Mouse X") * orbitalSpeed;
			orbitalY += Input.GetAxis("Mouse Y") * orbitalSpeed;

			orbitalY = Mathf.Clamp(orbitalY, -45f, 30f);
		}

		if(Input.GetMouseButtonUp(_mouseButtonRotate))
        {
			SetCursorVisible(true);
		}

		if(Input.GetKey(KeyCode.Q))
		{
			orbitalX += orbitalSpeed * Time.unscaledDeltaTime * keysRotationFactor;
		}

		if (Input.GetKey(KeyCode.E))
		{
			orbitalX -= orbitalSpeed * Time.unscaledDeltaTime * keysRotationFactor;
		}

		newRot = Quaternion.Euler(-orbitalY, orbitalX, 0f);
		transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.unscaledDeltaTime * orbitalSmoothness);
	}

	float scrollValue = -5f;

	void HandleZoom()
	{
		scrollValue += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
		scrollValue = Mathf.Clamp(scrollValue, -maxOutZoom, -maxInZoom);

		newZoom = scrollValue * zoomAmt;
		orbitalCam.transform.localPosition = Vector3.Lerp(orbitalCam.transform.localPosition, newZoom, Time.unscaledDeltaTime * zoomSmoothness);
	}

	void Focus()
	{
		if (mouseDragging)
			return;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
		{
			if (focusTransform != null)
			{
				focusTransform = null;
			}

		}

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, focusMask))
		{
			//MOUSE IS ON FOCUSABLE OBJECT
			if (Input.GetMouseButtonDown(0))
			{
				focusTransform = hit.collider.gameObject.transform;
			}
		}
	}

	void CheckInputs()
	{
		if (mouseButtonMove == mouseButtonRotate)
		{
			if (mouseButtonMove != mouseMoveBackup)
			{
				mouseButtonRotate = MouseInputs.none;
				mouseRotateBackup = mouseButtonRotate;
				_mouseButtonRotate = 10;
			}
			if (mouseButtonRotate != mouseRotateBackup)
			{
				mouseButtonMove = MouseInputs.none;
				mouseMoveBackup = mouseButtonMove;
				_mouseButtonMove = 10;
			}
		}

		if (mouseButtonMove != mouseMoveBackup)
		{
			switch (mouseButtonMove)
			{
				case MouseInputs.none:
					_mouseButtonMove = 10;
					break;
				case MouseInputs.left:
					_mouseButtonMove = 0;
					break;
				case MouseInputs.right:
					_mouseButtonMove = 1;
					break;
				case MouseInputs.middle:
					_mouseButtonMove = 2;
					break;
			}
			mouseMoveBackup = mouseButtonMove;
		}

		if (mouseButtonRotate != mouseRotateBackup)
		{
			switch (mouseButtonRotate)
			{
				case MouseInputs.none:
					_mouseButtonRotate = 10;
					break;
				case MouseInputs.left:
					_mouseButtonRotate = 0;
					break;
				case MouseInputs.right:
					_mouseButtonRotate = 1;
					break;
				case MouseInputs.middle:
					_mouseButtonRotate = 2;
					break;
			}
			mouseRotateBackup = mouseButtonRotate;
		}

		Cursor.lockState = borderPan && Application.isPlaying ? CursorLockMode.Confined : CursorLockMode.None;

	}

	void SetCursorVisible(bool _value)
    {
		if(!_value)
        {
			if (hideCursor && Cursor.visible)
			{
				Cursor.visible = _value;
			}
		}
        else
        {
			if (hideCursor && !Cursor.visible)
			{
				Cursor.visible = _value;
			}
		}	
	}

	public void SetDefault()
	{
		baseCameraSpeed = 8f;
		movementSmoothness = 10f;
		fastCameraSpeed = 15f;
		dragSpeed = 3f;
		maxDragSpeed = 10f;
		borderPan = true;
		borderThickness = 20f;
		hideCursor = true;
		orbitalSpeed = 5f;
		orbitalSmoothness = 5f;
		keysRotationFactor = 20f;
		zoomAmt = new Vector3(0, -1, 1);
		zoomSpeed = 10f;
		zoomSmoothness = 5f;
		maxInZoom = 5f;
		maxOutZoom = 30f;
	}

	bool FocusRayChecker()
    {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, focusMask))
		{
			return true;
		}
		else
			return false;
    }
}
