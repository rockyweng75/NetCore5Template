using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetCore5_Domain
{
    public static class ModelUtils
    {
        public static string ToCamelCase(string EntityName)
        {

            string result = "";
            IList<char> tempList = EntityName.ToList<char>();
            int index = 0;
            bool toUpper = false;
            foreach (char c in tempList)
            {
                string str = "";
                if (index++ > 0)
                {
                    if (!char.Parse("_").Equals(c) && !toUpper)
                    {
                        str = c.ToString().ToLower();
                    }
                    else if (toUpper)
                    {
                        str = c.ToString();
                        toUpper = false;
                    }
                    else
                    {
                        toUpper = true;
                    }
                }
                else
                {
                    str = c.ToString();
                }
                result += str;
            }
            return result;
        }

        public static string ToColumnName(string VMName)
        {

            string result = "";
            IList<char> tempList = VMName.ToList<char>();
            int index = 0;
            foreach (char c in tempList)
            {
                string str = "";
                if (index++ > 0)
                {
                    if (char.IsLower(c))
                    {
                        str = char.ToUpper(c).ToString();
                    }
                    else
                    {
                        str = "_" + c.ToString();
                    }
                }
                else
                {
                    str = c.ToString();
                }
                result += str;
            }
            return result;
        }

        public static void VM2Entity(object VM, object result)
        {
            if (VM == null || result == null)
            {
                return;
            }

            PropertyInfo[] infos = VM.GetType().GetProperties();
            PropertyInfo[] rInfos = result.GetType().GetProperties();

            foreach (PropertyInfo info in infos)
            {
                PropertyInfo rp = rInfos.Where((o) => o.Name.Equals(ToColumnName(info.Name))).FirstOrDefault();
                if (rp != null)
                {
                    try
                    {
                        rp.SetValue(result, info.GetValue(VM));
                    }
                    catch
                    {
                        // TODO fix oracle entity framework bug
                    }
                }
            }
        }

        public static void VM2EntityPKey(object VM, object result)
        {
            if (VM == null || result == null)
            {
                return;
            }

            PropertyInfo[] infos = VM.GetType().GetProperties();
            PropertyInfo[] rInfos = result.GetType().GetProperties();

            foreach (PropertyInfo info in infos)
            {

                PropertyInfo rp = rInfos.Where((o) => o.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)
                                                && o.Name.Equals(ToColumnName(info.Name))).FirstOrDefault();
                if (rp != null)
                {
                    try
                    {
                        rp.SetValue(result, info.GetValue(VM));
                    }
                    catch
                    {
                        // TODO fix oracle entity framework bug
                    }
                }
            }
        }

        public static void Entity2VM(object entity, object result)
        {

            if (entity == null || result == null)
            {
                return;
            }
            PropertyInfo[] infos = entity.GetType().GetProperties();
            PropertyInfo[] rInfos = result.GetType().GetProperties();

            foreach (PropertyInfo info in infos)
            {
                PropertyInfo rp = rInfos.Where((o) => o.Name.Equals(ToCamelCase(info.Name))).FirstOrDefault();
                if (rp != null)
                {
                    try
                    {
                        rp.SetValue(result, info.GetValue(entity));
                    }
                    catch
                    {
                        // TODO fix oracle entity framework bug
                    }
                }
            }
        }

        public static void Clone(object from, object to)
        {

            if (from == null || from == null)
            {
                return;
            }
            PropertyInfo[] infos = from.GetType().GetProperties();
            PropertyInfo[] rInfos = to.GetType().GetProperties();

            foreach (PropertyInfo info in infos)
            {
                PropertyInfo rp = rInfos.Where((o) => o.Name.Equals(info.Name)).FirstOrDefault();
                if (rp != null)
                {
                    try
                    {
                        rp.SetValue(to, info.GetValue(from));
                    }
                    catch
                    {
                        // TODO fix oracle entity framework bug
                    }
                }
            }
        }

        public static bool EqualsByPKey(object entity1, object entity2)
        {
            Type ths = entity1.GetType();
            Type objType = entity2.GetType();

            if (ths != objType)
            {
                return false;
            }


            foreach (System.Reflection.PropertyInfo pi in ths.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {

                if (pi.CustomAttributes.Where(o => typeof(KeyAttribute).Name.Equals(o.AttributeType.Name)).Any())
                {
                    object selfValue = ths.GetProperty(pi.Name).GetValue(entity1, null);
                    object toValue = ths.GetProperty(pi.Name).GetValue(entity2, null);

                    if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
