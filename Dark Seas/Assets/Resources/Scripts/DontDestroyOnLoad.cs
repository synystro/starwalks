using UnityEngine;

namespace DarkSeas
{
    public class DontDestroyOnLoad : MonoBehaviour
    {

        #region singleton

        private void Awake()
        {

            DontDestroyOnLoad(this);

        }

        #endregion

    }
}
