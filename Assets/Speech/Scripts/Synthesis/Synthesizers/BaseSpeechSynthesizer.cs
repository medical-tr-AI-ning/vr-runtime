using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Speech.Scripts.Utilities;
using UnityEngine;

namespace Speech.Scripts.Synthesis
{
    public abstract class BaseSpeechSynthesizer : MonoBehaviour
    {
        #region variables
        private static readonly string SUFFIX = ".mp3";

        public event SpeechSynthesisEventHandler SpeechSynthesisChunkCompleted;
        private List<string> _cachedItemsList;

        protected abstract string CacheDirName { get; }
        #endregion

        #region unity_functions
        protected virtual void Start()
        {
            PersistentSpeechFileManager.SetupPersistentDataStoragePath();
            initCachedItems();
        }
        #endregion

        #region public_functions
        public async Task Synthesize(Utterance utterance)
        {
            if (string.IsNullOrWhiteSpace(utterance?.Text))
            {
                throw new ArgumentException(nameof(Utterance.Text));
            }

            await SynthesizeImpl(utterance);
        }
        #endregion

        #region helper_functions
        protected abstract Task SynthesizeImpl(Utterance utterance);

        protected void HandleSpeechSynthesisChunk(byte[] contents, Utterance utterance)
        {
            string hash = createHashForUtterance(utterance);
            string path = PersistentSpeechFileManager.GetFilePath(hash + SUFFIX, CacheDirName);
            StorePersistentSpeechFile(contents, path);
            _cachedItemsList.Add(hash);
            SendSpeechSynthesisChunkCompleted(this, new SpeechSynthesisEventArgs { UtteranceText = utterance.Text, FilePath = path, Voice = utterance.VoiceType });
        }

        protected void HandleSpeechSynthesisCached(Utterance utterance)
        {
            string hash = createHashForUtterance(utterance);
            string path = PersistentSpeechFileManager.GetFilePath(hash + SUFFIX, CacheDirName);
            SendSpeechSynthesisChunkCompleted(this, new SpeechSynthesisEventArgs { UtteranceText = utterance.Text, FilePath = path, Voice = utterance.VoiceType });
        }

        protected bool isCached(Utterance utterance)
        {
            if (_cachedItemsList.Contains(createHashForUtterance(utterance)))
            {
                return true;
            }

            return false;
        }

        private string createHashForUtterance(Utterance utterance)
        {
            Hash128 hash = new Hash128();
            hash.Append(utterance.Text);
            hash.Append(utterance.VoiceID.ToString());

            return hash.ToString();
        }

        private void initCachedItems()
        {
            _cachedItemsList = new(PersistentSpeechFileManager.GetFileNamesInFolder(CacheDirName, true));
            foreach(string item in _cachedItemsList)
            {
                if (!Hash128.Parse(item).isValid)
                {
                    _cachedItemsList.Remove(item);
                    PersistentSpeechFileManager.DeleteSingleFile(item + SUFFIX, CacheDirName);
                }
            }
        }

        private void SendSpeechSynthesisChunkCompleted(object sender, SpeechSynthesisEventArgs args)
        {
            SpeechSynthesisChunkCompleted?.Invoke(sender, args);
        }

        private void StorePersistentSpeechFile(byte[] contents, string path)
        {            
            File.WriteAllBytes(path, contents);            
        }
        #endregion
    }

    public delegate void SpeechSynthesisEventHandler(object sender, SpeechSynthesisEventArgs args);
}