using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class SkillType {
        public string typename { get; private set; }
        public string label { get; private set; }

        public SkillType(string _typename, string _label) {
            typename = _typename;
            label = _label;
        }
    }
    class EffectType
    {
        public string typename { get; private set; }
        public string label { get; private set; }

        public EffectType(string _typename, string _label)
        {
            typename = _typename;
            label = _label;
        }
    }
    class Skill
    {
        public SkillType skilltype;
        public string name;
        public Skill(string _name, SkillType _skilltype) {
            name = _name;
            skilltype = _skilltype;
        }
    }
    class Effect
    {
        public EffectType effecttype;
        public string name;
        public int duration;
        public Effect(string _name, EffectType _effecttype, int _duration) {
            name = _name;
            effecttype = _effecttype;
            duration = _duration;
        }
    }
}
