using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Ranet.AgOlap.Controls.General
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
