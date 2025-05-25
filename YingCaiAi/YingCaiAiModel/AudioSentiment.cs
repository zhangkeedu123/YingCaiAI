using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{
    public class AudioSentiment
    {
        public LabelModel sentiment { get; set; }
        public List<LabelModel> emotion { get; set; }

        public List<LabelModel> intent { get; set; }

        public List<LabelModel> violation { get; set; }

    }

    public class LabelModel
    {
        public string label { get; set; }
        public double score { get; set; }
    }
}
