using UnityEngine;

public class Shooting : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    public GameObject bulletPrefab;
    public Transform bulletTransform;
    public bool canFire;
    private float timer;
    public float timerBetweenFiring;

    void Start()
    {
        mainCam = Camera.main; // Optimized: Using Camera.main instead of FindGameObjectWithTag
    }

    void Update()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePos - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timerBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }

        if (Input.GetMouseButton(0) && canFire)
        {
            canFire = false;
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || bulletTransform == null) return;

        // records metric
        MetricsManager.instance.playerMetrics.RecordShot();

        // Instantiate the bullet
        GameObject bulletInstance = Instantiate(bulletPrefab, bulletTransform.position, Quaternion.identity);

        // Get the bullet script and set its direction
        bulletScript bulletScriptInstance = bulletInstance.GetComponent<bulletScript>();
        if (bulletScriptInstance != null)
        {
            Vector2 shootDirection = (mousePos - bulletTransform.position).normalized;
            bulletScriptInstance.SetDirection(shootDirection);
        }
    }
}
