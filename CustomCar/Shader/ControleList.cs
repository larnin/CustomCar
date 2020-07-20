using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCar
{
    public class ControleList<T>
    {
        T m_defaultValue = default(T);
        List<T> m_list = new List<T>();
        int m_index = 0;

        public ControleList(T defaultValue = default(T))
        {
            m_defaultValue = defaultValue;
        }

        public int GetNextIndex()
        {
            while (m_index >= m_list.Count)
                m_list.Add(m_defaultValue);
            return m_index++;
        }

        public T GetItem(int index)
        {
            if (index < 0 || index >= m_list.Count)
                return m_defaultValue;
            return m_list[index];
        }

        public void SetItem(int index, T value)
        {
            if (index < 0 || index >= m_list.Count)
                return;
            m_list[index] = value;
        }

        public void ResetIndex()
        {
            m_index = 0;
        }

        public void Reset(T defaultValue)
        {
            m_list.Clear();
            m_index = 0;
            m_defaultValue = defaultValue;
        }

    }
}
