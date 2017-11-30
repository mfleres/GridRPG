using UnityEngine;
using System.Collections;

namespace GridRPG {
    /// <summary>
    /// Holds a message to be displayed.
    /// </summary>
    public class Message
    {
        public UI.ReadyEvent messageDisplayed; //Triggered when the message is displayed.
        public string text; //Message text.
        public Sprite icon; //Icon to be displayed along with the text.

        public Message()
        {
            this.text = "";
            this.icon = null;
        }

        public Message(string text)
        {
            this.text = text;
            this.icon = null;
        }

        public Message(string text, Sprite icon)
        {
            this.text = text;
            this.icon = icon;
        }
    }
}
