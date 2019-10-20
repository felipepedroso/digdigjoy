using UnityEngine;
using System.Collections;

public class TutorialCamera : MonoBehaviour {

	public SpriteRenderer tutorial = null;  // Atribuir na IDE da Unity

	private const int INVALID_TOUCH_ID = -1;
	private const float FIX_SCROLL_RATE = 1.2f;
	private const int MAX_IGNORE_TOUCHES = 10;
	private const float SCROLL_SPEED_KEEP_RATE = 0.95f;
	private const float ENTRY_RATE = 0.25f;

	private float minTutY;
	private float maxTutY;
	private float scrollRateY;

	private Vector3 tutStartPos;
	private bool sceneStarting;
	private bool sceneEnding;

	private int scrollTouchId;
	private int touchY;
	private int touchPreviousY;
	private float scrollSpeed;

	private int ignoreTouchCount;
	private int[] ignoreTouchId;

	// Use this for initialization
	void Start () {
		Camera.main.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
		Camera.main.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

		// Retangulo da tela onde visao da camera eh exibida
		Rect screenRect = Camera.main.pixelRect;
		// print("Camera displays from (" + screenRect.xMin + ", " + screenRect.yMin + ") to (" + screenRect.xMax + ", " + screenRect.yMax + ") pixel");
	
		// Calcula tamanho da imagem de tutorial em unidades do mundo
		Rect r = tutorial.sprite.rect;
		float tutWidth = (r.xMax - r.xMin) / tutorial.sprite.pixelsPerUnit;
		float tutHeight = (r.yMax - r.yMin) / tutorial.sprite.pixelsPerUnit;
		// print("Tutorial width = " + tutWidth + ", height = " + tutHeight);

		// A origin do Ray abaixo estah no plano z = Clipping planes Near (vide IDE da Unity)
		Ray rayTopRight = Camera.main.ScreenPointToRay(new Vector3 (screenRect.xMax, screenRect.yMax, 0));
		// print ("Ray TR origin = (" + rayTopRight.origin.x + ", " + rayTopRight.origin.y + ", " + rayTopRight.origin.z + ")");
		// print ("Ray TR direction = (" + rayTopRight.direction.x + ", " + rayTopRight.direction.y + ", " + rayTopRight.direction.z + ")");

		// Calcula a coordenada Z do tutorial para que sua largura
		// encaixe exatamente na largura do campo de visao da camera
		float k = (tutWidth / 2.0f) / rayTopRight.direction.x;
		float tutPosZ = k * rayTopRight.direction.z;

		maxTutY = (tutHeight / 2.0f) - k * rayTopRight.direction.y;
		minTutY = -maxTutY;

		tutStartPos = new Vector3 (0.0f, minTutY, tutPosZ);
		sceneStarting = true;
		sceneEnding = false;

		scrollRateY = (2.0f * k * rayTopRight.direction.y) / (screenRect.yMax - screenRect.yMin);

		scrollTouchId = INVALID_TOUCH_ID;
		scrollSpeed = 0.0f;

		// Lista de IDs de touches ignorados
		ignoreTouchCount = 0;
		ignoreTouchId = new int[MAX_IGNORE_TOUCHES];
	}

	// Update is called once per frame
	void Update () {
		int i;
		Touch t;
		bool endScroll = false;

		if (sceneStarting) {
			if ((tutStartPos - tutorial.transform.position).magnitude < 0.01f) {
				tutorial.transform.position = tutStartPos;
				sceneStarting = false;
			}
			else
				tutorial.transform.position = (1.0f - ENTRY_RATE) * tutorial.transform.position + ENTRY_RATE * tutStartPos;
		}

		if (Input.GetKey(KeyCode.Escape))
			sceneEnding = true;

		if (sceneEnding) {
			if (tutorial.transform.position.z < 1000.0f) {
				tutorial.transform.position = new Vector3(tutorial.transform.position.x * 0.9f,
				                                          tutorial.transform.position.y * 0.9f,
				                                          tutorial.transform.position.z + 1.0f + tutorial.transform.position.z / 20.0f);
				tutorial.transform.Rotate(new Vector3(0.0f, 0.0f, 10.0f));
			}
			else
				Application.LoadLevel("MenuScene");
		}

		for (i = 0; i < Input.touchCount; i++) {
			t = Input.touches[i];
			if (IsIgnoreTouchId(t.fingerId)) {
				if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
					RemoveIgnoreTouchId(t.fingerId);
			}
			else if (scrollTouchId != INVALID_TOUCH_ID) {
				if (t.fingerId == scrollTouchId) {
					touchY = (int) t.position.y;
					if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
						endScroll = true;
				}
				else
					AddIgnoreTouchId(t.fingerId);
			}
			else {
				scrollTouchId = t.fingerId;
				touchY = touchPreviousY = (int) t.position.y;
			}
		}

		float deltaTutY = 0.0f;
		if (scrollTouchId != INVALID_TOUCH_ID) {
			deltaTutY = (float) (touchY - touchPreviousY) * scrollRateY;
			touchPreviousY = touchY;
			scrollSpeed = 0.5f * scrollSpeed + 0.5f * deltaTutY;

			if (endScroll)
				scrollTouchId = INVALID_TOUCH_ID;
		}
		else {
			deltaTutY = scrollSpeed;
			scrollSpeed *= SCROLL_SPEED_KEEP_RATE;
			if (scrollSpeed < 0.01f && scrollSpeed > -0.01f)
				scrollSpeed = 0.0f;
		}

		if (deltaTutY != 0.0f) {
			Vector3 newTutPos = tutorial.transform.position;
			newTutPos.y += deltaTutY;
			if (newTutPos.y < minTutY)
				newTutPos.y = minTutY;
			else if (newTutPos.y > maxTutY)
				newTutPos.y = maxTutY;
			tutorial.transform.position = newTutPos;
		}
	}

	void AddIgnoreTouchId (int touchId) {
		if (ignoreTouchCount < MAX_IGNORE_TOUCHES) {
			ignoreTouchId[ignoreTouchCount] = touchId;
			ignoreTouchCount++;
		}
	}

	void RemoveIgnoreTouchId (int touchId) {
		int i = 0;
		while (i < ignoreTouchCount) {
			if (ignoreTouchId[i] == touchId) {
				ignoreTouchCount--;
				while (i < ignoreTouchCount) {
					ignoreTouchId[i] = ignoreTouchId[i + 1];
					i++;
				}
			}
			i++;
		}
	}

	bool IsIgnoreTouchId (int touchId) {
		int i;
		for (i = 0; i < ignoreTouchCount; i++)
			if (ignoreTouchId[i] == touchId)
				return true;
		return false;
	}
}
