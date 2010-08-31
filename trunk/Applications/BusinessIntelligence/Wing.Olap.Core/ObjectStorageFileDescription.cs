/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Olap.Core
{
    public class ObjectStorageFileDescription
    {
        public ObjectStorageFileDescription()
        { }

        public ObjectStorageFileDescription(ObjectDescription descr)
        {
            m_Description = descr;
        }

        ObjectDescription m_Description;
        public ObjectDescription Description
        {
            get {
                if (m_Description == null)
                    m_Description = new ObjectDescription();
                return m_Description; 
            }
            set { m_Description = value; }
        }


        String m_ContentFileName = String.Empty;
        public String ContentFileName
        {
            get { return m_ContentFileName; }
            set { m_ContentFileName = value; }
        }
    }
}
