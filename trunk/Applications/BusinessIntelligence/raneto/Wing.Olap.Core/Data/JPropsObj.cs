using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ranet.Olap.Core.Data
{
    public abstract class JPropsObj
    {
        public List<PropertyData> props;
        public JPropsObj(List<PropertyData> props) {
            this.props = props;
        }
    }

    public class CellPropsObj : JPropsObj
    {
        int hashCode = 0;
        public CellPropsObj(List<PropertyData> props)
            : base (props)
        {
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
            var ps = obj as JPropsObj;
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
                            if (!p.Equals(ps.props[i]))
                                return false;
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

    public class MemberPropsObj : JPropsObj
    {
        int hashCode = 0;
        public MemberPropsObj(List<PropertyData> props)
            : base(props)
        {
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
            var ps = obj as JPropsObj;
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
                            if (!p.Equals(ps.props[i]))
                                return false;
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

    public class CustomMemberPropsObj : JPropsObj
    {
        int hashCode = 0;
        public CustomMemberPropsObj(List<PropertyData> props)
            : base(props)
        {
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
            var ps = obj as JPropsObj;
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
                            if (!p.Equals(ps.props[i]))
                                return false;
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
