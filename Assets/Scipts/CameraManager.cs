using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [Header("Camera")]
    public Camera mainCamera;

    [Header("Positions")]
    public Transform marcusView;
    public Transform demonstrationView;

    [Header("Settings")]
    public float transitionSpeed = 2f;

    private Coroutine activeCameraCoroutine;

    void Awake()
    {
        Instance = this;
    }

    // ── Call this when Marcus is teaching ─────
    public void MoveTOMarcusView()
    {
        if (activeCameraCoroutine != null)
            StopCoroutine(activeCameraCoroutine);

        activeCameraCoroutine = StartCoroutine(
            MoveCamera(
                marcusView.position,
                marcusView.rotation,
                transitionSpeed
            )
        );
    }

    // ── Call this when demonstration starts ───
    public void MoveToDemonstrationView()
    {
        if (activeCameraCoroutine != null)
            StopCoroutine(activeCameraCoroutine);

        activeCameraCoroutine = StartCoroutine(
            MoveCamera(
                demonstrationView.position,
                demonstrationView.rotation,
                transitionSpeed
            )
        );
    }

    // ── Follow a specific character ───────────
    public void FocusOnCharacter(Transform character)
    {
        if (activeCameraCoroutine != null)
            StopCoroutine(activeCameraCoroutine);

        activeCameraCoroutine = StartCoroutine(
            FollowCharacter(character)
        );
    }

    // ── Smooth camera move to position ────────
    IEnumerator MoveCamera(
        Vector3 targetPos,
        Quaternion targetRot,
        float duration)
    {
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Smooth ease
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            mainCamera.transform.position = Vector3.Lerp(
                startPos, targetPos, smoothT);
            mainCamera.transform.rotation = Quaternion.Lerp(
                startRot, targetRot, smoothT);

            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;
    }

    // ── Keeps camera looking at character ─────
    IEnumerator FollowCharacter(Transform target)
    {
        while (target != null)
        {
            Vector3 direction =
                target.position - mainCamera.transform.position;

            Quaternion targetRot =
                Quaternion.LookRotation(direction);

            mainCamera.transform.rotation = Quaternion.Lerp(
                mainCamera.transform.rotation,
                targetRot,
                Time.deltaTime * transitionSpeed
            );

            yield return null;
        }
    }
}