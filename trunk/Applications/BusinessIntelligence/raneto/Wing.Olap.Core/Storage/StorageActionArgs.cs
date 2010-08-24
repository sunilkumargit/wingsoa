/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ranet.Olap.Core.Storage
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
