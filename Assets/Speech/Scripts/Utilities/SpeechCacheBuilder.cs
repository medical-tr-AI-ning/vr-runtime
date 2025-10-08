using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Speech.Scripts.Synthesis
{
    public class SpeechCacheBuilder : MonoBehaviour
    {
        [SerializeField]
        private SpeechSynthesizerController _speechSynthesizerController;

        [SerializeField]
        private string pathToDialogueOptions = "";

        [SerializeField]
        private bool Execute = false;
        
        private void OnValidate()
        {
            if (Execute && pathToDialogueOptions != "")
            {
                foreach(string response in ReadDialogueOptions())
                {
                    _speechSynthesizerController.Speak(response);
                }
                Execute = false;
            }
        }

        private void Awake()
        {
            _speechSynthesizerController = GetComponentInChildren<SpeechSynthesizerController>();
        }

        private List<string> ReadDialogueOptions()
        {
            JObject dialogueOptionsJson = JObject.Parse(ReadTextAssetFromStreamingAssets(pathToDialogueOptions));
            List<string> responses = new List<string>();

            foreach (KeyValuePair<string, JToken> item in dialogueOptionsJson)
            {
                JToken responsesJson = item.Value;
                if (responsesJson.Type == JTokenType.Array)
                {
                    var responsesArray = (JArray)responsesJson;
                    IEnumerator<JToken> responseEnumerator = responsesArray.GetEnumerator();
                    while (responseEnumerator.MoveNext())
                    {
                        responses.Add(responseEnumerator.Current.Value<string>());
                    }
                }
            }
            return responses;
        }

        private string ReadTextAssetFromStreamingAssets(string path)
        {
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, path);
            return System.IO.File.ReadAllText(filePath);
        }
    }
}
