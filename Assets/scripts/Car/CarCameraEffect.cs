using UnityEngine;
using Cinemachine;
using System.Collections;

public class CarCameraEffect : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;
    public float impactForwardOffset = -1f;
    public float impactMoveSpeed = 10f;
    public float returnMoveSpeed = 5f;

    private CinemachineTransposer transposer;
    private Vector3 defaultOffset;
    private bool isImpacting = false;

    void Start()
    {
        if (virtualCam == null) virtualCam = GetComponent<CinemachineVirtualCamera>();
        transposer = virtualCam.GetCinemachineComponent<CinemachineTransposer>();
        defaultOffset = transposer.m_FollowOffset;
    }

    public void TriggerCameraEffect()
    {
        if (!isImpacting) StartCoroutine(ImpactEffect());
    }

    IEnumerator ImpactEffect()
    {
        isImpacting = true;
        Vector3 impactOffset = defaultOffset + new Vector3(0, 0, impactForwardOffset);
        while (Vector3.Distance(transposer.m_FollowOffset, impactOffset) > 0.01f)
        {
            transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, impactOffset, Time.deltaTime * impactMoveSpeed);
            yield return null;
        }
        while (Vector3.Distance(transposer.m_FollowOffset, defaultOffset) > 0.01f)
        {
            transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, defaultOffset, Time.deltaTime * returnMoveSpeed);
            yield return null;
        }
        transposer.m_FollowOffset = defaultOffset;
        isImpacting = false;
    }
}