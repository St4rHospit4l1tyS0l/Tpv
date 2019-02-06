using System;

namespace Tpv.Printer.Model.Shared
{
    public class MasterPropertyModel
    {
        public Action<string, string, Func<int, int>> Transform { get; }
        public string Field { get; }
        public bool IsRequired { get; }
        public string FieldMsg { get; }
        public Func<int, int> ExtraTransform { get; }

        public MasterPropertyModel(Action<string, string, Func<int, int>> transform, string field,
            bool isRequired, string fieldMsg, Func<int, int> extraTransform = null)
        {
            Transform = transform;
            Field = field;
            IsRequired = isRequired;
            FieldMsg = fieldMsg;
            ExtraTransform = extraTransform;
        }

    }
}
