using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing
{
    public interface IPathMapper
    {
        String MapPath(String relativePath);
    }
}
