using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPos : MonoBehaviour
{
    private Transform playerTransform;
    private float maxDashLength;
    private float minDashLength;
    private int PlayerSwag;
    private float minSwagCost;
    private RectTransform pointerRectTransform;
    public float objectHeight = 30f;
    private SpriteRenderer _spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        maxDashLength = GameObject.FindWithTag("Player").GetComponent<PlayerMovements>().maxDashRange;
        minDashLength = GameObject.FindWithTag("Player").GetComponent<PlayerMovements>().minDashRange;
        minSwagCost = GameObject.FindWithTag("Player").GetComponent<PlayerStateManager>().dashCost;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public static Vector3 ClampMagnitude(Vector3 v, float max, float min)
    {
        double sm = v.sqrMagnitude;
        if (sm > (double)max * (double)max) return v.normalized * max;
        else if (sm < (double)min * (double)min) return v.normalized * min;
        return v;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos1 = new Vector3(0f, 0f, 0f);
        Vector3 pos2 = playerTransform.position;
        Vector3 lTemp = transform.localScale;
        int swag = GameObject.FindWithTag("Player").GetComponent<PlayerStateManager>().getSwag();

        if (swag < minSwagCost)
        {
            _spriteRenderer.enabled = false;
            return;
        }
        _spriteRenderer.enabled = true;
        pos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition); /*new Vector3(Input.mousePosition.x, Input.mousePosition.y, (float)Camera.main.nearClipPlane + 0.5f);
        pos1 = Camera.main.ScreenToWorldPoint(pos1);*/
        pos1.z = 0f;
        pos2.z = 0f;
        Vector3 diff = pos1 - pos2;
        float distanceSq = diff.magnitude;
        var dashVector = diff / distanceSq;
        Vector3 dashDestination = new Vector3(0f, 0f, 0f);
        if (distanceSq == 0)
        {
            return;
        }
        else if (minDashLength <= distanceSq && distanceSq <= maxDashLength)
        {
            dashDestination = pos1;
        }
        else
        {
            dashDestination = pos2 + ClampMagnitude(diff, maxDashLength, minDashLength);
        }
        pos1 = dashDestination;//Camera.main.ScreenToWorldPoint(dashDestination);
        pos1.z = 1f;
        var v3 = pos1 - pos2;
        transform.position = pos2 + (v3 / 2.0f);
        Debug.Log(v3.magnitude);
        lTemp.x = v3.magnitude / objectHeight;
        float angle = (Mathf.Atan2(v3.y, v3.x) * Mathf.Rad2Deg) % 360;
        transform.localScale = lTemp;
        transform.rotation = Quaternion.Euler(Vector3.forward * angle);
    }
}
