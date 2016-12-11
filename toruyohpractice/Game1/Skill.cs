using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    enum Timing { moving, dying,  } //movingは普通の状態、dyingは死んだ瞬間を意味する
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

        public bool used( int _duration,int _loop_index=-1,int _hp = 0, int _maxHp=1)
        {
            bool succeed = Condition.skillCondition(DataBase.getSkillData(skillName).conditions,_loop_index,_duration,_hp,_hp*100/_maxHp);
            if (succeed) {
                coolDown = DataBase.getSkillData(skillName).cooldownFps;
            }
            return succeed;
        }
    }
}
