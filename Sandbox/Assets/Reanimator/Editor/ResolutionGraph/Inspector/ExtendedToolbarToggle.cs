using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation {

    public class ToggleButton {
        public string text;
        public event Action enabled;
        public event Action disabled;

        public ToggleButton(string text)
        {
            this.text = text;
        }
        public ToggleButton() {}
        
        public void Enabled()
        {
            enabled?.Invoke();
        }
        public void Disabled()
        {
            disabled?.Invoke();
        }
    }
    public sealed class ExtendedToolbarToggle : ToolbarToggle {
        public new class UxmlFactory : UxmlFactory<ExtendedToolbarToggle, UxmlTraits> { }

        private ToggleButton m_ToggleToggleButton;

        public ExtendedToolbarToggle()
        {
            m_ToggleToggleButton = new ToggleButton();
            AddToClassList("extendedToolbarToggle");
        }

        public string buttonText {
            get => this.text;
            set => this.text = value;
        }

        public event Action enabled
        {
            add {
                if (m_ToggleToggleButton == null) return;
                m_ToggleToggleButton.enabled += value;
            }
            remove
            {
                if (m_ToggleToggleButton == null) return;
                m_ToggleToggleButton.enabled -= value;
            }
        }
        public event Action disabled
        {
            add {
                if (m_ToggleToggleButton == null) return;
                m_ToggleToggleButton.disabled += value;
            }
            remove
            {
                if (m_ToggleToggleButton == null) return;
                m_ToggleToggleButton.disabled -= value;
            }
        }

        protected override void ToggleValue()
        {
            base.ToggleValue();
            switch (value) {
                case true:
                    RemoveFromClassList("un-selected");
                    AddToClassList("selected");
                    
                    m_ToggleToggleButton.Enabled();
                    break;
                case false:
                    RemoveFromClassList("selected");
                    AddToClassList("un-selected");
                    
                    m_ToggleToggleButton.Disabled();
                    break;
            }
        }
    }
}