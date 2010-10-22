using System.Collections.Generic;

namespace Wing.Olap.Controls.General
{
    public class SessionHolder
    {
        // Ключ - ID cоедиенения, значение - ID сессии
        Dictionary<string, string> m_Sessions = new Dictionary<string, string>();
        /// <summary>
        /// Возвращает ID сессии по ID соединения
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public string this[string connectionId]
        {
            get
            {
                lock (this)
                {
                    if (m_Sessions.ContainsKey(connectionId))
                    {
                        return m_Sessions[connectionId];
                    }
                }

                return null;
            }
            set
            {
                lock (this)
                {
                    m_Sessions[connectionId] = value;
                }
            }
        }
    }
}
