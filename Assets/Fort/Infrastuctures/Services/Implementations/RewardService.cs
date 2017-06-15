using UnityEngine;
using System.Collections;

namespace Fort
{
    [Service(ServiceType = typeof(IRewardService))]
    public class RewardService : MonoBehaviour,IRewardService
    {
    }
}