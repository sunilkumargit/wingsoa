/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Olap.Core.Storage
{
    public class StorageActionArgs
    {
        public StorageActionArgs()
        { 
        
        }

        StorageActionTypes m_ActionType = StorageActionTypes.None;
        public StorageActionTypes ActionType
        {
            get { return m_ActionType; }
            set { m_ActionType = value; }
        }
        
        StorageContentTypes m_ContentType = StorageContentTypes.None;
        public StorageContentTypes ContentType
        {
            get { return m_ContentType; }
            set { m_ContentType = value; }
        }

        String m_Content = string.Empty;
        public String Content
        {
            get { return m_Content; }
            set { m_Content = value; }
        }

        ObjectStorageFileDescription m_FileDescription = null;
        public ObjectStorageFileDescription FileDescription
        {
            get {
                if (m_FileDescription == null)
                    m_FileDescription = new ObjectStorageFileDescription();
                return m_FileDescription;
            }
            set { m_FileDescription = value; }
        }
    }

    public enum StorageContentTypes
    { 
        MdxDesignerLayout,
        ValueCopySettings,
        CustomCellStyles,
        None
    }

    public enum StorageActionTypes
    {
        Save,
        Load,
        GetList,
        None,
        Clear,
        Delete
    }

}
