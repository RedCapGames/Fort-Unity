using UnityEngine;
using System.Collections;

namespace Fort
{
    [Service(ServiceType = typeof(IAudioService))]
    public class AudioService : MonoBehaviour,IAudioService
    {
    }
}