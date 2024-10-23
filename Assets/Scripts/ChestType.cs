using UnityEngine;

[CreateAssetMenu(fileName = "NewChest", menuName = "Chests/ChestType")]
public class ChestType : ScriptableObject
{
    public string chestName;
    public int unlockTimeInMinutes;
    public int gemsPer10Minutes;
    public int coinReward;
    public int gemReward;
}
