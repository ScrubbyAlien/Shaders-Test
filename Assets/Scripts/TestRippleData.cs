using UnityEngine;

public class TestRippleData : MonoBehaviour
{
    [SerializeField]
    private Material rippleMaterial;

    private Camera mCamera;
    private Ray sptr;

    private void Start() {
        sptr = new();
        mCamera = Camera.main;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            sptr = mCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(sptr, out RaycastHit hit, 1000, LayerMask.GetMask("ground"))) {
                rippleMaterial.SetFloat("RippleStartTime", Time.time);
                rippleMaterial.SetVector("Origin", hit.point);
            }
        }
    }
}