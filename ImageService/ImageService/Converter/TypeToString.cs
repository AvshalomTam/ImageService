using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Converter
{
    class TypeToString
    {
        public static Dictionary<MessageTypeEnum, string> typeConverter = new Dictionary<MessageTypeEnum, string> {
            { MessageTypeEnum.INFO, "INFO" }, { MessageTypeEnum.FAIL, "ERROR" }, { MessageTypeEnum.WARNING, "WARNING" } };

        public static string getType(MessageTypeEnum type)
        {
            try
            {
                if (typeConverter.TryGetValue(type, out string strType))
                    return strType;
                else
                    return "ERROR";
            }
            catch
            {
                return "ERROR";
            }
        }
    }
}
