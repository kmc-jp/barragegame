using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class Skill
    {
        public int coolDown=0;
        public string skillName;

        public Skill(string _skillName)
        {
            skillName = _skillName;
        }

        public void update(bool update=true)
        {
            if (update)
            {
                coolDown--;
            }
        }

        public void used(int i = 0)
        {
            if (i == 0)
            {
                coolDown = DataBase.getSkillData(skillName).cooldownFps;
            }
        }
    }
}
