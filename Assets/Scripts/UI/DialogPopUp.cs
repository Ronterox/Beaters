using UnityEngine;

namespace UI
{
    public class DialogPopUp : MonoBehaviour
    {
        public string playerPrefKey;

        private void Start()
        {
            if (PlayerPrefs.HasKey(playerPrefKey) && PlayerPrefs.GetInt(playerPrefKey) == 1) Destroy(gameObject);
        }

        public void CloseAndSave()
        {
            PlayerPrefs.SetInt(playerPrefKey, 1);
            PlayerPrefs.Save();

            gameObject.SetActive(false);
        }
    }
}
