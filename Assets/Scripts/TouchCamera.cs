using UnityEngine;
using System.Collections;

// This al
public class TouchCamera : MonoBehaviour
{
	private float orthoZoomSpeed = 0.15f;
	private float cameraMovementSpeed = 0.1f;
	const float MAX_ZOOM_IN = 1.8f;
	const float MAX_ZOOM_OUT = 0.7f;

	float MAX_PAN_X, MAX_PAN_Y;

	float initialOrthographicSize;

	Vector3 initialCameraPosition;

	TileEngine tileEngine;

	void Start ()
	{
		initialOrthographicSize = GetComponent<Camera>().orthographicSize;
		initialCameraPosition = GetComponent<Camera>().transform.position;
		tileEngine = (TileEngine)FindObjectOfType(typeof(TileEngine));

		MAX_PAN_X = tileEngine != null ? tileEngine.Width / 2 : 10;
		MAX_PAN_Y = tileEngine != null ? tileEngine.Height / 2 : 10;
	}

	void Update ()
	{
		if (Input.touchCount == 2) {
			Touch touchZero = Input.GetTouch (0);
			Touch touchOne = Input.GetTouch (1);

			if (touchZero.phase != TouchPhase.Moved && touchOne.phase != TouchPhase.Moved) {
				return;
			}

			Vector2 touchZeroCurrentPos = touchZero.position;
			Vector2 touchOneCurrentPos = touchOne.position;

			Vector2 touchZeroPrevPos = touchZeroCurrentPos - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOneCurrentPos - touchOne.deltaPosition;

			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			if (GetComponent<Camera>().orthographic) {
					GetComponent<Camera>().orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
			}

			Vector2 avgDelta = (touchZero.deltaPosition + touchOne.deltaPosition) / 2;
			float deltaX = -avgDelta.x * cameraMovementSpeed;
			float deltaY = -avgDelta.y * cameraMovementSpeed;
			GetComponent<Camera>().transform.Translate(new Vector3(deltaX, deltaY));
		} else if(Input.touchCount == 3){
			OnClickResetCamera();
		}

		float cameraX = GetComponent<Camera>().transform.position.x;
		
		if (Mathf.Abs(cameraX) > MAX_PAN_X) {
			float direction = cameraX / Mathf.Abs(cameraX);
			GetComponent<Camera>().transform.Translate((MAX_PAN_X * direction) - cameraX, 0, 0);
		}
		
		float cameraY = GetComponent<Camera>().transform.position.y;
		
		if (Mathf.Abs(cameraY) > MAX_PAN_Y) {
			float direction = cameraY / Mathf.Abs(cameraY);
			GetComponent<Camera>().transform.Translate(0,(MAX_PAN_Y * direction) - cameraY, 0);
		}

		if (GetComponent<Camera>().orthographicSize > initialOrthographicSize * MAX_ZOOM_IN) {
				GetComponent<Camera>().orthographicSize = initialOrthographicSize * MAX_ZOOM_IN;
		} else if (GetComponent<Camera>().orthographicSize < initialOrthographicSize * MAX_ZOOM_OUT) {
				GetComponent<Camera>().orthographicSize = initialOrthographicSize * MAX_ZOOM_OUT;
		}
	}

	public void OnClickResetCamera()
	{
		iTween.MoveTo (GetComponent<Camera>().gameObject, initialCameraPosition, 0.5f);
		iTween.ValueTo(gameObject, iTween.Hash("from", GetComponent<Camera>().orthographicSize,"to", initialOrthographicSize,"onupdate", "UpdateCameraSize", "time", 0.5f));
	}

	public void UpdateCameraSize(float newValue)
	{
		GetComponent<Camera>().orthographicSize = newValue;
	}
}
