namespace ILCompiler.Compiler
{
    public enum LabelType
    {
        BasicBlock,
        String,
        FieldRVAData
    }

    public static class LabelGenerator
    {
        private static readonly Dictionary<LabelType, string> _labelFormatByType = new Dictionary<LabelType, string>()
        {
            { LabelType.BasicBlock, "BB{0:00}" },
            { LabelType.String, "S{0}" },
            { LabelType.FieldRVAData, "FRVA{0}" }
        };


        private static readonly Dictionary<LabelType, int> _nextIdByType = new Dictionary<LabelType, int>();

        public static string GetLabel(LabelType labelType)
        {
            _nextIdByType.TryGetValue(labelType, out int id);
            _nextIdByType[labelType] = id + 1;

            var format = _labelFormatByType[labelType];

            return String.Format(format, id);
        }
    }
}
