using UnityEngine;
using World;

namespace Player
{
    public class PlayerIO : MonoBehaviour
    {
        public float MaxInteractDistance = 8;
        public byte SelectedInventory;
        public bool ResetCamera;
        public Animator PlayerAnimator;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            var isWaling = Input.GetKey(KeyCode.W)
                           || Input.GetKey(KeyCode.A)
                           || Input.GetKey(KeyCode.S)
                           || Input.GetKey(KeyCode.D);
//            PlayerAnimator.SetBool("walking", isWaling);

            if (GameObject.FindWithTag("FPSController").transform.position.y < -20)
            {
                Debug.Log("Player fell through world, resetting!");
                GameObject.FindWithTag("FPSController").transform.position = new Vector3(
                    GameObject.FindWithTag("FPSController").transform.position.x, 60,
                    GameObject.FindWithTag("FPSController").transform.position.z);
            }

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) ||
                Input.GetKeyDown(KeyCode.C))
            {
                var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
                RaycastHit hit;
                var rayDistance = MaxInteractDistance;

                if (!ResetCamera)
                {
                    rayDistance *= 3.14159f;
                }

                if (Physics.Raycast(ray, out hit, rayDistance))
                {
                    var chunk = hit.transform.GetComponent<Chunk>();

                    if (chunk == null) return;

                    if (Input.GetMouseButtonDown(0))
                    {
                        var hitPoint = hit.point;
                        hitPoint -= hit.normal / 4;
                        chunk.SetBrick(0, hitPoint);
                    }

                    if (Input.GetMouseButtonDown(1))
                    {
                        var hitPoint = hit.point;
                        if (SelectedInventory != 0)
                        {
                            hitPoint += hit.normal / 4;
                            chunk.SetBrick(SelectedInventory, hitPoint);
                        }
                    }

                    if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.C))
                    {
                        var hitPoint = hit.point;
                        hitPoint -= hit.normal / 4;
                        SelectedInventory = chunk.GetByte(hitPoint);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (!ResetCamera)
                {
                    Camera.main.transform.localPosition -= Vector3.forward * 3.14159f;
                }
                else
                {
                    Camera.main.transform.position = transform.position;
                }

                ResetCamera = !ResetCamera;
            }

            if (Input.GetKey(KeyCode.Escape) && Input.GetKey(KeyCode.F1))
            {
                Application.Quit();
            }
        }
    }
}
