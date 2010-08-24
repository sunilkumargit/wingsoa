using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AnalysisServices.AdomdClient;

namespace Ranet.Olap.Core.Data
{
    class AdomdCellPropsObj
    {
        int hashCode = 0;
        public CellPropertyCollection props;

        public AdomdCellPropsObj(CellPropertyCollection properties)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            props = properties;

            foreach (var p in props)
            {
                switch (p.Name)
                {
                    case "CellOrdinal":
                    case "VALUE":
                    case "FORMATTED_VALUE":
                        continue;
                    default:
                        unchecked
                        {
                            hashCode *= 3;
                            hashCode ^= p.GetHashCode();
                            break;
                        }
                }
            }
        }

        public override bool Equals(object obj)
        {
            var ps = obj as AdomdCellPropsObj;
            if (ps == null)
                return false;

            if (ps.props.Count != props.Count)
                return false;

            for (int i = 0; i < props.Count; i++)
            {
                var p = props[i];
                switch (p.Name)
                {
                    case "CellOrdinal":
                    case "VALUE":
                    case "FORMATTED_VALUE":
                        continue;
                    default:
                        unchecked
                        {
                            if(p.Name != ps.props[i].Name)
                                return false;
                            if (p.Value == null)
                            {
                                if (ps.props[i].Value != null)
                                   return false;
                            }
                            else
                            {
                                if (!p.Value.Equals(ps.props[i].Value))
                                    return false;
                            }
                            break;
                        }
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }

    class AdomdMemberPropsObj
    {
        int hashCode = 0;
        public PropertyCollection props;

        public AdomdMemberPropsObj(PropertyCollection properties)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            props = properties;

            foreach (var p in props)
            {
                switch (p.Name)
                {
                    case "CellOrdinal":
                    case "VALUE":
                    case "FORMATTED_VALUE":
                        continue;
                    default:
                        unchecked
                        {
                            hashCode *= 3;
                            hashCode ^= p.GetHashCode();
                            break;
                        }
                }
            }
        }

        public override bool Equals(object obj)
        {
            var ps = obj as AdomdMemberPropsObj;
            if (ps == null)
                return false;

            if (ps.props.Count != props.Count)
                return false;

            for (int i = 0; i < props.Count; i++)
            {
                var p = props[i];
                switch (p.Name)
                {
                    default:
                        unchecked
                        {
                            if (p.Name != ps.props[i].Name)
                                return false;
                            if (p.Value == null)
                            {
                                if (ps.props[i].Value != null)
                                    return false;
                            }
                            else
                            {
                                if (!p.Value.Equals(ps.props[i].Value))
                                    return false;
                            }
                            break;
                        }
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }

    class AdomdCustomMemberPropsObj
    {
        int hashCode = 0;

        public MemberPropertyCollection props;
        public AdomdCustomMemberPropsObj(MemberPropertyCollection properties)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            props = properties;

            foreach (var p in props)
            {
                switch (p.Name)
                {
                    case "CUBE_NAME":
                    case "DIMENSION_UNIQUE_NAME":
                    case "HIERARCHY_UNIQUE_NAME":
                    case "LEVEL_UNIQUE_NAME":
                    case "LEVEL_NUMBER":
                    case "PARENT_COUNT":
                    case "PARENT_LEVEL":
                    case "PARENT_UNIQUE_NAME":
                    case "SKIPPED_LEVELS":
                        unchecked
                        {
                            hashCode *= 3;
                            hashCode ^= p.GetHashCode();
                            break;
                        }
                    default:
                        continue;
                }
            }
        }

        public override bool Equals(object obj)
        {
            var ps = obj as AdomdCustomMemberPropsObj;
            if (ps == null)
                return false;

            if (ps.props.Count != props.Count)
                return false;

            for (int i = 0; i < props.Count; i++)
            {
                var p = props[i];
                switch (p.Name)
                {
                    case "CUBE_NAME":
                    case "DIMENSION_UNIQUE_NAME":
                    case "HIERARCHY_UNIQUE_NAME":
                    case "LEVEL_UNIQUE_NAME":
                    case "LEVEL_NUMBER":
                    case "PARENT_COUNT":
                    case "PARENT_LEVEL":
                    case "PARENT_UNIQUE_NAME":
                    case "SKIPPED_LEVELS":
                        unchecked
                        {
                            if (p.Name != ps.props[i].Name)
                                return false;
                            if (p.Value == null)
                            {
                                if (ps.props[i].Value != null)
                                    return false;
                            }
                            else
                            {
                                if (!p.Value.Equals(ps.props[i].Value))
                                    return false;
                            }
                            break; 
                        }
                    default:
                        continue;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }

}
