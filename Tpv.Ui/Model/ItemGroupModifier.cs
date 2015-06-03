using System;

namespace Tpv.Ui.Model
{
    public class ItemGroupModifier
    {
        public string Name { get; set; }
        public string Minimum { get; set; }
        public string Maximum { get; set; }
        public int Id { get; set; }

        public int MinimumVal
        {
            get
            {
                if (String.IsNullOrEmpty(Minimum))
                    return 0;

                int val;
                return int.TryParse(Minimum, out val) ? val : SharedConstants.ZERO_VALUE;
            }
        }

        public int MaximumVal
        {
            get
            {
                if (String.IsNullOrEmpty(Maximum))
                    return 0;

                int val;
                return int.TryParse(Maximum, out val) ? val : SharedConstants.ZERO_VALUE;
            }
        }
    }
}