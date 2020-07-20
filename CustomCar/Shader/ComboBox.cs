
using UnityEngine;

namespace CustomCar
{
    public class ComboBox
    {
        private static bool m_forceToUnShow = false;
        private static int m_useControlID = -1;
        private bool m_isClickedComboButton = false;
        private int m_selectedItemIndex = 0;

        private GUIContent m_buttonContent;
        private GUIContent[] m_listContent;
        private string m_buttonStyle;
        private string m_boxStyle;
        private GUIStyle m_listStyle;

        private Rect m_rect;
        private bool m_done;

        public ComboBox(GUIContent buttonContent, GUIContent[] listContent, GUIStyle listStyle)
        {
            m_buttonContent = buttonContent;
            m_listContent = listContent;
            m_buttonStyle = "button";
            m_boxStyle = "box";
            m_listStyle = listStyle;
        }

        public ComboBox(GUIContent buttonContent, GUIContent[] listContent, string buttonStyle, string boxStyle, GUIStyle listStyle)
        {
            m_buttonContent = buttonContent;
            m_listContent = listContent;
            m_buttonStyle = buttonStyle;
            m_boxStyle = boxStyle;
            m_listStyle = listStyle;
        }

        public ComboBox(GUIContent[] listContent)
        {
            m_listStyle = new GUIStyle();
            m_listStyle.normal.textColor = Color.white;
            m_listStyle.onHover.background =
            m_listStyle.hover.background = new Texture2D(2, 2);
            m_listStyle.padding.left =
            m_listStyle.padding.right =
            m_listStyle.padding.top =
            m_listStyle.padding.bottom = 4;

            if (listContent.Length > 0)
                m_buttonContent = listContent[0];
            else m_buttonContent = new GUIContent("Null");
            m_listContent = listContent;
            m_buttonStyle = "button";
            m_boxStyle = "box";
        }

        public void SetContent(GUIContent[] listContent)
        {
            if (listContent.Length == 0)
                return;

            if (listContent.Length <= m_selectedItemIndex)
                m_selectedItemIndex = 0;

            m_listContent = listContent;

            m_buttonContent = m_listContent[m_selectedItemIndex];
        }

        public int Show()
        {
            if (m_forceToUnShow)
            {
                m_forceToUnShow = false;
                m_isClickedComboButton = false;
            }

            m_done = false;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.mouseUp:
                    {
                        if (m_isClickedComboButton)
                        {
                            m_done = true;
                        }
                    }
                    break;
            }

            if (GUILayout.Button(m_buttonContent, m_buttonStyle))
            {
                if (m_useControlID == -1)
                    m_useControlID = controlID;

                if (m_useControlID != controlID)
                {
                    m_forceToUnShow = true;
                    m_useControlID = controlID;
                }
                m_isClickedComboButton = true;
            }

            if (m_isClickedComboButton)
                m_rect = GUILayoutUtility.GetLastRect();
            
            return m_selectedItemIndex;
        }

        public void DisplayList()
        {
            if (m_isClickedComboButton)
            {

                Rect listRect = new Rect(m_rect.x, m_rect.y + m_listStyle.CalcHeight(m_listContent[0], 1.0f),
                            m_rect.width, m_listStyle.CalcHeight(m_listContent[0], 1.0f) * m_listContent.Length);

                GUI.Box(listRect, "", m_boxStyle);
                int newSelectedItemIndex = GUI.SelectionGrid(listRect, m_selectedItemIndex, m_listContent, 1, m_listStyle);
                if (newSelectedItemIndex != m_selectedItemIndex)
                {
                    m_selectedItemIndex = newSelectedItemIndex;
                    m_buttonContent = m_listContent[m_selectedItemIndex];
                }
            }

            if (m_done)
                m_isClickedComboButton = false;
        }

        public int SelectedItemIndex
        {
            get
            {
                return m_selectedItemIndex;
            }
            set
            {
                m_selectedItemIndex = value;
            }
        }
    }
}