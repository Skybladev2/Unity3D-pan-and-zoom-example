using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    // двигаем камеру
    private bool drag = false;
    // масштабируем
    private bool zoom = false;

    // экранные координаты начальной точки касани€
    private Vector3 initialTouchPosition;
    // мировые координаты камеры при инициировании
    // перемещени€/масштабировани€
    private Vector3 initialCameraPosition;

    // экранные координаты начальной точки первого касани€
    private Vector3 initialTouch0Position;
    // экранные координаты начальной точки второго касани€
    private Vector3 initialTouch1Position;
    // средн€€ точка между начальными координатами касаний
    private Vector3 initialMidPointScreen;
    // ортогональный размер камеры на момент начала масштабировани€
    private float initialOrthographicSize;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1)
        {
            zoom = false;
            Touch touch0 = Input.GetTouch(0);

            if (IsTouching(touch0))
            {
                if (!drag)
                {
                    initialTouchPosition = touch0.position;
                    initialCameraPosition = this.transform.position;

                    drag = true;
                }
                else
                {
                    Vector2 delta = camera.ScreenToWorldPoint(touch0.position) -
                                    camera.ScreenToWorldPoint(initialTouchPosition);

                    Vector3 newPos = initialCameraPosition;
                    newPos.x -= delta.x;
                    newPos.y -= delta.y;

                    this.transform.position = newPos;
                }
            }
            else
            {
                drag = false;
            }
        }
        else
        {
            drag = false;
        }

        if (Input.touchCount == 2)
        {
            drag = false;

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (!zoom)
            {
                initialTouch0Position = touch0.position;
                initialTouch1Position = touch1.position;
                initialCameraPosition = this.transform.position;
                initialOrthographicSize = Camera.main.orthographicSize;
                initialMidPointScreen = (touch0.position + touch1.position) / 2;

                zoom = true;
            }
            else
            {
                this.transform.position = initialCameraPosition;
                camera.orthographicSize = initialOrthographicSize;

                float scaleFactor = GetScaleFactor(touch0.position,
                                                   touch1.position,
                                                   initialTouch0Position,
                                                   initialTouch1Position);

                Vector2 currentMidPoint = (touch0.position + touch1.position) / 2;
                Vector3 initialMidPointWorldBeforeZoom = camera.ScreenToWorldPoint(initialMidPointScreen);

                Camera.main.orthographicSize = initialOrthographicSize / scaleFactor;

                Vector3 initialMidPointWorldAfterZoom = camera.ScreenToWorldPoint(initialMidPointScreen);
                Vector2 initialMidPointDelta = initialMidPointWorldBeforeZoom - initialMidPointWorldAfterZoom;

                Vector2 oldAndNewMidPointDelta =
                    camera.ScreenToWorldPoint(currentMidPoint) -
                    camera.ScreenToWorldPoint(initialMidPointScreen);

                Vector3 newPos = initialCameraPosition;
                newPos.x -= oldAndNewMidPointDelta.x - initialMidPointDelta.x;
                newPos.y -= oldAndNewMidPointDelta.y - initialMidPointDelta.y;

                this.transform.position = newPos;
            }
        }
        else
        {
            zoom = false;
        }
    }

    static bool IsTouching(Touch touch)
    {
        return touch.phase == TouchPhase.Began ||
                touch.phase == TouchPhase.Moved ||
                touch.phase == TouchPhase.Stationary;
    }

    public static float GetScaleFactor(Vector2 position1, Vector2 position2, Vector2 oldPosition1, Vector2 oldPosition2)
    {
        float distance = Vector2.Distance(position1, position2);
        float oldDistance = Vector2.Distance(oldPosition1, oldPosition2);

        if (oldDistance == 0 || distance == 0)
        {
            return 1.0f;
        }

        return distance / oldDistance;
    }
}
