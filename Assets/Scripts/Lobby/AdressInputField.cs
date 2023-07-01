using UnityEngine;
using UnityEngine.UI;

namespace MFPS
{
    [RequireComponent(typeof(InputField))]
    public class AdressInputField : MonoBehaviour
    {
        const string AdressPrefKey = "ServerIPAdress";

        void Start()
        {
            string defaultName = "127.0.0.1";
            InputField _inputField = this.GetComponent<InputField>();

            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(AdressPrefKey))
                {
                    defaultName = PlayerPrefs.GetString(AdressPrefKey);
                    _inputField.text = defaultName;
                }
            }
        }

        public void SetIPAdress(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("ip is null or empty");
                return;
            }

            PlayerPrefs.SetString(AdressPrefKey, value);
        }
    }
}