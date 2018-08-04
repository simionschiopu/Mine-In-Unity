using System.Linq;
using Repository;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using World;

namespace UI
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField]
        private Toggle[] worldTypeToggles;

        [SerializeField]
        private Button generateWorldButton;

        private ISettingsRepository settingsRepository;

        private void Awake()
        {
            settingsRepository = new SettingsRepository();

            var currentSelectedWorldType = settingsRepository.GetWorldType();
            for (var index = 0; index < worldTypeToggles.Length; index++)
            {
                worldTypeToggles[index].isOn = currentSelectedWorldType == (WorldType) index;
            }

            generateWorldButton.onClick.AddListener(() =>
            {
                var worldType = (WorldType) worldTypeToggles.ToList().FindIndex(x => x.isOn);
                settingsRepository.SaveWorldType(worldType);

                SceneManager.LoadScene(0);
            });
        }
    }
}
