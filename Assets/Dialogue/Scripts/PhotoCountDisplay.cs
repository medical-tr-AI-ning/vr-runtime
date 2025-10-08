using System;
using MedicalTraining.Configuration;
using TMPro;
using UnityEngine;

namespace MedicalTraining.Dialogue
{
    /// <summary>
    /// Controls the photo count display on the "Aufnahme Speichern" button.
    /// </summary>
    public class PhotoCountDisplay : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        private bool _photoLimitEnabled;
        private int _photoLimit;
        [SerializeField, ReadOnly] private TakePhoto _takePhoto;

        public void SetupPhotoCount(TakePhoto takePhoto)
        {
            _takePhoto = takePhoto;

            _text = GetComponent<TextMeshProUGUI>();
            if (_takePhoto is null)
            {
                Debug.LogWarning("Photo Count Display is missing TakePhoto reference!");
                return;
            }

            _takePhoto.PhotoCountUpdated += updateDisplay;
            updateDisplay();
        }

        private void OnStart()
        {
            updateDisplay();
        }

        private void updateDisplay()
        {
            int maxPictures = _takePhoto.PhotoLimit;
            int takenPictures = _takePhoto.GetPhotosTaken();
            string displayText = $"{takenPictures}{(_takePhoto.PhotoLimitEnabled ? "/"+maxPictures : string.Empty)}";
            Debug.Log(displayText);
            _text.text = displayText;
        }
    }
}