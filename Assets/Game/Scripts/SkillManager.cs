using CustomInspector;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    [field: SerializeField] [field: SelfFill(true, mode = OwnerMode.DirectChildren)]
    public DashSkill Dash { get; private set; }

    [field: SerializeField] [field: SelfFill(true, mode = OwnerMode.DirectChildren)]
    public CloneSkill Clone { get; private set; }

    [field: SerializeField] [field: SelfFill(true, mode = OwnerMode.DirectChildren)]
    public SwordSkill Sword { get; private set; }


    [field: SerializeField] [field: SelfFill(true, mode = OwnerMode.DirectChildren)]
    public BlackholeSkill Blackhole { get; private set; }


    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }
}