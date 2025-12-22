using UnityEngine;

namespace EarthUnderFreelancer.Player
{
    /// <summary>
    /// Camera controller for following and orbiting the player vehicle
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0, 5, -15);

        [Header("Follow Settings")]
        [SerializeField] private float followSpeed = 10f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float lookAheadFactor = 3f;

        [Header("Zoom Settings")]
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 30f;
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float currentZoom = 15f;

        [Header("Orbit Settings")]
        [SerializeField] private bool allowOrbit = true;
        [SerializeField] private float orbitSensitivity = 2f;
        [SerializeField] private float minVerticalAngle = -30f;
        [SerializeField] private float maxVerticalAngle = 60f;

        [Header("Camera Shake")]
        [SerializeField] private float shakeIntensity = 0.5f;
        [SerializeField] private float shakeDuration = 0.2f;

        private float currentOrbitX = 0f;
        private float currentOrbitY = 20f;
        private float shakeTimer = 0f;
        private Vector3 shakeOffset = Vector3.zero;
        private CameraMode currentMode = CameraMode.Follow;

        public enum CameraMode { Follow, Orbit, Cockpit, Free }

        private void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    target = player.transform;
            }

            if (target != null)
            {
                transform.position = target.position + target.TransformDirection(offset);
                transform.LookAt(target);
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            UpdateCameraShake();

            switch (currentMode)
            {
                case CameraMode.Follow:
                    UpdateFollowCamera();
                    break;
                case CameraMode.Orbit:
                    UpdateOrbitCamera();
                    break;
                case CameraMode.Cockpit:
                    UpdateCockpitCamera();
                    break;
                case CameraMode.Free:
                    UpdateFreeCamera();
                    break;
            }

            transform.position += shakeOffset;
        }

        private void UpdateFollowCamera()
        {
            Vector3 desiredPosition = target.position + target.TransformDirection(offset.normalized * currentZoom);
            desiredPosition.y = target.position.y + offset.y;

            Rigidbody targetRb = target.GetComponent<Rigidbody>();
            if (targetRb != null)
            {
                desiredPosition += targetRb.linearVelocity * lookAheadFactor * Time.deltaTime;
            }

            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

            Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        }

        private void UpdateOrbitCamera()
        {
            if (Input.GetMouseButton(1) && allowOrbit)
            {
                currentOrbitX += Input.GetAxis("Mouse X") * orbitSensitivity;
                currentOrbitY -= Input.GetAxis("Mouse Y") * orbitSensitivity;
                currentOrbitY = Mathf.Clamp(currentOrbitY, minVerticalAngle, maxVerticalAngle);
            }

            Quaternion rotation = Quaternion.Euler(currentOrbitY, currentOrbitX, 0);
            Vector3 position = target.position + rotation * new Vector3(0, 0, -currentZoom);

            transform.position = Vector3.Lerp(transform.position, position, followSpeed * Time.deltaTime);
            transform.LookAt(target);
        }

        private void UpdateCockpitCamera()
        {
            transform.position = target.position + target.TransformDirection(new Vector3(0, 1, 2));
            transform.rotation = target.rotation;
        }

        private void UpdateFreeCamera()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            float upDown = 0f;

            if (Input.GetKey(KeyCode.E)) upDown = 1f;
            if (Input.GetKey(KeyCode.Q)) upDown = -1f;

            Vector3 move = new Vector3(horizontal, upDown, vertical) * 20f * Time.deltaTime;
            transform.Translate(move, Space.Self);

            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X") * 2f;
                float mouseY = Input.GetAxis("Mouse Y") * 2f;
                transform.eulerAngles += new Vector3(-mouseY, mouseX, 0);
            }
        }

        private void UpdateCameraShake()
        {
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;
                shakeOffset = Random.insideUnitSphere * shakeIntensity * (shakeTimer / shakeDuration);
            }
            else
            {
                shakeOffset = Vector3.zero;
            }
        }

        public void ShakeCamera(float intensity, float duration)
        {
            shakeIntensity = intensity;
            shakeDuration = duration;
            shakeTimer = duration;
        }

        public void SetTarget(Transform newTarget) => target = newTarget;
        public void SetCameraMode(CameraMode mode) => currentMode = mode;

        public void SetZoom(float zoom)
        {
            currentZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        }

        public void ZoomIn() => currentZoom = Mathf.Max(currentZoom - zoomSpeed * Time.deltaTime, minZoom);
        public void ZoomOut() => currentZoom = Mathf.Min(currentZoom + zoomSpeed * Time.deltaTime, maxZoom);

        private void Update()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                currentZoom -= scroll * zoomSpeed * 10f;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                int nextMode = ((int)currentMode + 1) % System.Enum.GetValues(typeof(CameraMode)).Length;
                SetCameraMode((CameraMode)nextMode);
            }
        }
    }
}
