using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class Skill
    {
        SkillData skillData;
        public int coolDown=0;
        public string skillName;

        public Skill(string _skillName)
        {
            skillName = _skillName;
        }
        public void update()
        {
            coolDown--;
        }
    }
}
