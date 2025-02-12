using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public Player player;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }
}