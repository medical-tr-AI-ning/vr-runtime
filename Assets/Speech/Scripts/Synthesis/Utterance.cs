using System;
using System.Xml;

namespace Speech.Scripts.Synthesis
{
    public enum Voice
    {
        patient,
        doctor
    }

    public class Utterance
    {
        #region variables
        public string ID { get; private set; }

        public string Text { get; set; }

        public Voice VoiceType { get; set; } = Voice.patient;

        public string VoiceID { get; set; }

        public float Rate { get; set; } = 1f;

        #endregion

        #region public_functions
        public Utterance()
        {
            ID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Creates Speech Synthesis Markup Language (SSML) conform string for speech synthesis
        /// </summary>
        /// <param name="voice">Voice enum element</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string ToSSML(string voice)
        {
            if (string.IsNullOrWhiteSpace(voice))
            {
                throw new ArgumentException(nameof(voice));
            }

            if (string.IsNullOrWhiteSpace(Text))
            {
                throw new ArgumentException(nameof(Text));
            }

            var doc = new XmlDocument();
            doc.LoadXml("<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"de-DE\"></speak>");

            var voiceElement = doc.CreateElement("voice");
            voiceElement.SetAttribute("name", voice);

            var prosodyElement = doc.CreateElement("prosody");
            prosodyElement.SetAttribute("rate", Rate.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
            prosodyElement.InnerText = Text;

            voiceElement.AppendChild(prosodyElement);

            doc.DocumentElement?.AppendChild(voiceElement);

            return doc.OuterXml;
        }
        #endregion
    }
}