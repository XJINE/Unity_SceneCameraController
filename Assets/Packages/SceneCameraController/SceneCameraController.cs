using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;
#endif

public class SceneCameraController : MonoBehaviour
{
    #region Class

    [CustomEditor(typeof(SceneCameraController))]
    public class SceneCameraControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Reset"))
            {
                SceneCameraController sceneCameraController = target as SceneCameraController;
                sceneCameraController.Reset();
            }

            base.OnInspectorGUI();
        }
    }

    #endregion Class

    #region Enum

    public enum MouseButton
    {
        Left   = 0,
        Right  = 1,
        Middle = 2
    }

    public enum MouseMove
    {
        X           = 0,
        Y           = 1,
        ScrollWheel = 2
    }

    #endregion Enum

    #region Field

    private Vector3 moveTarget   = Vector3.zero;
    private Vector3 rotateTarget = new Vector3(0, 0, 1);

    private static readonly string[] MouseMoveString = new string[]
    {
        "Mouse X",
        "Mouse Y",
        "Mouse ScrollWheel"
    };

    public Vector3 resetCameraPosition = Vector3.zero;
    public Vector3 resetCameraRotation = Vector3.zero;

    // Move Settings

    public MouseMove moveTrigger = MouseMove.ScrollWheel;
    public bool enableMove = true;
    public bool invertMoveDirection = false;
    public float moveSpeed = 6f;
    public bool limitMoveX = false;
    public bool limitMoveY = false;
    public bool limitMoveZ = false;
    public bool smoothMove = true;
    public float smoothMoveSpeed = 10f;

    // Rotate Settings

    public MouseButton rotateTrigger = MouseButton.Right;
    public bool enableRotate = true;
    public bool invertRotateDirection = false;
    public float rotateSpeed = 3f;
    public bool limitRotateX = false;
    public bool limitRotateY = false;
    public bool smoothRotate = true;
    public float smoothRotateSpeed = 10f;

    // Drag Settings

    public MouseButton dragTrigger = MouseButton.Middle;
    public bool enableDrag = true;
    public bool invertDragDirection = false;
    public float dragSpeed = 3f;
    public bool limitDragX = false;
    public bool limitDragY = false;
    public bool smoothDrag = true;
    public float smoothDragSpeed = 10f;

    #endregion Field

    #region Method

    protected virtual void Start()
    {
        this.moveTarget   = this.transform.position;
        this.rotateTarget = this.transform.forward;
    }

    protected virtual void Update()
    {
        Move();
        Rotate();
        Drag();
    }

    private void Move()
    {
        if (!this.enableMove)
        {
            return;
        }

        float moveAmount = Input.GetAxis(SceneCameraController.MouseMoveString[(int)this.moveTrigger]);

        if (moveAmount != 0)
        {
            float direction = this.invertMoveDirection ? -1 : 1;

            this.moveTarget  = this.transform.forward;
            this.moveTarget *= this.moveSpeed * moveAmount * direction;
            this.moveTarget += this.transform.position;

            if (this.limitMoveX)
            {
                this.moveTarget.x = this.transform.position.x;
            }

            if (this.limitMoveY)
            {
                this.moveTarget.y = this.transform.position.y;
            }

            if (this.limitMoveZ)
            {
                this.moveTarget.z = this.transform.position.z;
            }
        }

        if (this.smoothMove)
        {
            if (this.moveTarget == this.transform.position)
            {
                this.moveTarget = this.transform.position;
            }

            this.transform.position = Vector3.Lerp(this.transform.position,
                                                   this.moveTarget,
                                                   this.smoothMoveSpeed * Time.deltaTime);
        }
        else
        {
            this.transform.position = moveTarget;
        }
    }

    private void Rotate()
    {
        if (!this.enableRotate)
        {
            return;
        }

        float direction = this.invertRotateDirection ? -1 : 1;

        float mouseX = Input.GetAxis(SceneCameraController.MouseMoveString[(int)MouseMove.X]) * direction;
        float mouseY = Input.GetAxis(SceneCameraController.MouseMoveString[(int)MouseMove.Y]) * direction;

        if (Input.GetMouseButton((int)this.rotateTrigger))
        {
            if (!this.limitRotateX)
            {
                this.rotateTarget = Quaternion.Euler(0, mouseX * this.rotateSpeed, 0) * this.rotateTarget;
            }

            if (!this.limitRotateY)
            {
                this.rotateTarget = Quaternion.AngleAxis(mouseY * this.rotateSpeed,
                                                         Vector3.Cross(this.transform.forward, Vector3.up)) * this.rotateTarget;
            }
        }

        if (this.smoothRotate)
        {
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                       Quaternion.LookRotation(this.rotateTarget),
                                                       this.smoothRotateSpeed * Time.deltaTime);
        }
        else
        {
            this.transform.rotation = Quaternion.LookRotation(this.rotateTarget);
        }
    }

    private void Drag()
    {
        if (!this.enableDrag)
        {
            return;
        }

        // CAUTION:
        // direction is difference from Move and Rotate.

        float direction = this.invertDragDirection ? 1 : -1;
        float mouseX = Input.GetAxis(SceneCameraController.MouseMoveString[(int)MouseMove.X]) * direction;
        float mouseY = Input.GetAxis(SceneCameraController.MouseMoveString[(int)MouseMove.Y]) * direction;

        if (Input.GetMouseButton((int)this.dragTrigger))
        {
            this.moveTarget = this.transform.position;

            if (!this.limitDragX)
            {
                this.moveTarget += this.transform.right * mouseX * dragSpeed;
            }

            if (!this.limitDragY)
            {
                this.moveTarget += Vector3.up * mouseY * dragSpeed;
            }
        }

        if (this.smoothDrag)
        {
            this.transform.position = Vector3.Lerp(this.transform.position,
                                                   this.moveTarget,
                                                   this.smoothDragSpeed * Time.deltaTime);
        }
        else
        {
            this.transform.position = this.moveTarget;
        }
    }

    public void Reset()
    {
        this.transform.position = this.resetCameraPosition;
        this.transform.rotation = Quaternion.Euler(this.resetCameraRotation);
        this.moveTarget         = this.transform.position;
        this.rotateTarget       = this.transform.forward;
    }

    #endregion Method
}