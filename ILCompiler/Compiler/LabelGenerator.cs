using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler.Compiler
{
    public enum LabelType
    {
        BasicBlock,
        String,
    }

    public static class LabelGenerator
    {
        private static Dictionary<LabelType, string> _labelFormatByType = new Dictionary<LabelType, string>()
        {
            { LabelType.BasicBlock, "bb{0}" },
            { LabelType.String, "s{0}" },
        };


        private static Dictionary<LabelType, int> _nextIdByType = new Dictionary<LabelType, int>();

        public static string GetLabel(LabelType labelType)
        {
            _nextIdByType.TryGetValue(labelType, out int id);
            _nextIdByType[labelType] = id + 1;

            var format = _labelFormatByType[labelType];

            return String.Format(format, id);
        }
    }
}
