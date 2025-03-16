using UnityEngine;

public class PlayerBulletManager : MonoBehaviour
{
    public static PlayerBulletManager Instance;
    private int activeBulletCount = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void RegisterBullet()
    {
        activeBulletCount++;
    }

    public void UnregisterBullet()
    {
        activeBulletCount--;
        Debug.Log("Bullet count: " + activeBulletCount);
    }

    public bool BulletCount()
    {
        return activeBulletCount > 0;
    }
}
