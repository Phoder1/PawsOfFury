using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private enum PressState { DefaultState, Pressed, Left }
    private PressState pressState;
    [SerializeField] GameObject minion;
    [SerializeField] GameObject draggedObject;
    [SerializeField] GameObject SpawnPoint;
    [SerializeField] LayerMask pathLayer;

    Camera mainCam;
    LevelManager levelManager;
    SpriteRenderer spawnPointSprite;
    bool positionValid;
    private void Start()
    {
        levelManager = LevelManager._instance;
        mainCam = Camera.main;
        spawnPointSprite = SpawnPoint.GetComponent<SpriteRenderer>();
        positionValid = false;
    }

    private void Update()
    {
        if (pressState == PressState.Left)
        {
            draggedObject.transform.position = InputManager.RayToPlanePosition(mainCam.ScreenPointToRay(Input.mousePosition));
            if (Physics.Raycast(draggedObject.transform.position, Vector3.down, out RaycastHit floorHit, draggedObject.transform.position.y + 0.5f, pathLayer))
            {
                SpawnPoint.transform.position = new Vector3(draggedObject.transform.position.x, floorHit.point.y + 0.5f, draggedObject.transform.position.z);
                spawnPointSprite.color = Color.green;
                positionValid = true;
            }
            else
            {
                SpawnPoint.transform.position = new Vector3(draggedObject.transform.position.x, SpawnPoint.transform.position.y, draggedObject.transform.position.z);
                spawnPointSprite.color = Color.red;
                positionValid = false;
            }

        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (pressState == PressState.DefaultState)
            pressState = PressState.Pressed;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (pressState == PressState.Pressed)
        {
            pressState = PressState.Left;
            draggedObject.SetActive(true);
            SpawnPoint.SetActive(true);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (pressState == PressState.Left && positionValid)
        {
            //Spawn minion
            Instantiate(minion, draggedObject.transform.position - Vector3.up * InputManager.dragHeight, Quaternion.identity);
            //Todo: MONEY MONEY MONEY, take money
        }
        pressState = PressState.DefaultState;
        draggedObject.SetActive(false);
        SpawnPoint.SetActive(false);
        positionValid = false;
    }
}
